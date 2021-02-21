using FlatBread.Inherit;
using FlatBread.Log;
using System.Net;
using System.Net.Sockets;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using FlatBread.Session;
using FlatBread.Enum;
using FlatBread.Buffer;

namespace FlatBread.Tcp
{
    /// <summary>
    /// TCP通信方案
    /// </summary>
    public class TcpServer
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 服务器端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 最大连接数
        /// </summary>
        public int MaxConnect { get; set; } = 1000;

        /// <summary>
        /// 最大等待队列数
        /// </summary>
        public int MaxWaitQueue { get; set; } = 100;

        /// <summary>
        /// 缓冲位大小
        /// </summary>
        public int BufferSize { get; set; } = 4096;

        /// <summary>
        /// 缓冲池单元
        /// </summary>
        public int BufferPoolUnit { get; set; }

        /// <summary>
        /// 缓冲池最大容量
        /// </summary>
        public int BufferPoolSize { get { return BufferSize * BufferPoolUnit; } }

        /// <summary>
        /// 地址族
        /// <para>默认为IPV4</para>
        /// </summary>
        public AddressFamily AddressFamily { get; set; } = AddressFamily.InterNetwork;

        /// <summary>
        /// 构造函数
        /// <para>地址默认为当前计算机地址</para>
        /// </summary>
        public TcpServer(int port)
        {
            this.Port = port;
        }

        /// <summary>
        /// 服务端Socket
        /// </summary>
        internal Socket ServerSocket { get; set; }

        /// <summary>
        /// 用户端接套字容器池
        /// </summary>
        ShakeHandEventArgsPool ShakeHandEventPool { get; set; }

        /// <summary>
        /// 开启服务
        /// </summary>
        public void StartServer()
        {
            //1.服务端绑定、监听
            {
                switch (AddressFamily)
                {
                    case AddressFamily.InterNetwork:
                        ServerSocket = new Socket(AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        ServerSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
                        break;
                    case AddressFamily.InterNetworkV6:
                        ServerSocket = new Socket(AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        ServerSocket.Bind(new IPEndPoint(IPAddress.IPv6Any, Port));
                        break;
                }
                ServerSocket.Listen(65535);
            }
            LogHelper.LogInfo("Tcp服务已经开始监听~");

            //2.初始化用户端接套字容器池、缓存池
            {
                ShakeHandEventPool = new ShakeHandEventArgsPool(BufferSize, MaxConnect, AsyncDispatchCenter);
            }
            LogHelper.LogInfo("容器池已加载完毕~");

            //3.开始调用用户端接套字容器池监听
            {
                ShakeHandAsync();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ShakeHandAsync()
        {
            var evetArgs = ShakeHandEventPool.Pop();
            //异步接收连接
            if (!ServerSocket.AcceptAsync(evetArgs))
            {
                LogHelper.LogWarn("异步接收用户回调失败 同步进入回调函数~");

                //如果异步接收失败则同步接收
                ProcessAccept(evetArgs);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ProcessAccept(SocketAsyncEventArgs e)
        {
            ShakeHandEventArgs eventArgs = (ShakeHandEventArgs)e;

            //接收用户成功
            if (eventArgs.LastOperation == SocketAsyncOperation.Accept && eventArgs.SocketError == SocketError.Success)
            {
                //创建会话信息(当前会话)
                UserTokenSession UserToken = eventArgs.UserToken as UserTokenSession;
                var EndPoint = (IPEndPoint)eventArgs.AcceptSocket.RemoteEndPoint;
                var AllHost = Dns.GetHostEntry(EndPoint.Address).AddressList;
                UserToken.UserHost = string.Join('|', AllHost.Select(x => x.ToString()).ToArray());
                UserToken.UserPort = ((IPEndPoint)(eventArgs.AcceptSocket.RemoteEndPoint)).Port;
                UserToken.Mode = SocketMode.Server;
                UserToken.OperationTime = DateTime.Now;
                eventArgs.SendEventArgs.SendAction = ProcessSend;
                eventArgs.ReceiveEventArgs.ReceiveAction = ProcessReceive;
                UserToken.ShakeHandEvent = eventArgs;
                OnConnect?.Invoke(UserToken);

                //异步接收客户端行为
                //异步接收客户端消息
                if (!UserToken.Channel.ReceiveAsync(UserToken.ShakeHandEvent.ReceiveEventArgs))
                {
                    ProcessReceive(UserToken.ShakeHandEvent.ReceiveEventArgs);
                }
            }
            else
            {
                LogHelper.LogError($"连接失败~ 收到的状态:SocketError.{eventArgs.SocketError}");
                //接收用户失败就清理后送回池
                eventArgs.Clear();
                ShakeHandEventPool.Push(eventArgs);
            }
            //继续接收下一个
            ShakeHandAsync();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AsyncDispatchCenter(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                //检测到迎接行为
                case SocketAsyncOperation.Accept:
                    ProcessAccept(e);
                    break;
                //检测到输入行为
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ProcessSend(SocketAsyncEventArgs e)
        {
            SendEventArgs eventArgs = (SendEventArgs)e;
            UserTokenSession UserToken = (UserTokenSession)eventArgs.UserToken;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ProcessReceive(SocketAsyncEventArgs e)
        {
            ReceiveEventArgs eventArgs = (ReceiveEventArgs)e;
            UserTokenSession UserToken = (UserTokenSession)eventArgs.UserToken;
            if (eventArgs.SocketError == SocketError.Success && eventArgs.BytesTransferred > 0)
            {
                //解码回调
                eventArgs.Decode((packet) =>
                {
                    if (packet.Mode == MessageMode.MessageByte
                    || packet.Mode == MessageMode.MessageShort
                    || packet.Mode == MessageMode.MessageInt)
                        OnReceive?.Invoke(UserToken, packet);
                });

                //释放行为接套字的连接(此步骤无意义,只是以防万一)
                eventArgs.AcceptSocket = null;

                //继续接收消息
                if (!UserToken.Channel.ReceiveAsync(e))
                {
                    //此次接收没有接收完毕 递归接收
                    ProcessReceive(e);
                }
            }
            else
            {
                LogHelper.LogWarn("接收到客户端未采集的状态:" + eventArgs.SocketError);
                //客户端正常走这步
                OnExit?.Invoke(UserToken);
                //清理连接接套字
                UserToken.Clear();
                //推回接套字池
                ShakeHandEventPool.Push(UserToken.ShakeHandEvent);
            }
        }

        public Action<UserTokenSession> OnConnect { get; set; }
        public Action<UserTokenSession, Packet> OnReceive { get; set; }
        public Action<UserTokenSession> OnExit { get; set; }
    }
}
