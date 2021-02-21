using FlatBread.Buffer;
using FlatBread.Enum;
using FlatBread.Inherit;
using FlatBread.Log;
using FlatBread.Session;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlatBread.Tcp
{
    public class TcpClient
    {
        /// <summary>
        /// 用户的名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 服务地址
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 地址族
        /// <para>默认为IPV4</para>
        /// </summary>
        public AddressFamily AddressFamily { get; set; } = AddressFamily.InterNetwork;

        /// <summary>
        /// 缓冲位大小
        /// </summary>
        int BufferSize { get; set; } = 4096;

        /// <summary>
        /// 接收使用的缓冲位
        /// </summary>
        Memory<byte> ReceiveBuffer { get; set; }

        /// <summary>
        /// 握手使用的接套字
        /// </summary>
        ShakeHandEventArgs ShakeHandEvent { get; set; }

        /// <summary>
        /// 客户端Socket
        /// </summary>
        Socket Client { get; set; }

        public TcpClient(string host, int port)
        {
            this.Host = host;
            this.Port = port;

            UserTokenSession Session = new UserTokenSession();
            //初始化发送接套字
            SendEventArgs SendEvent = new SendEventArgs();
            SendEvent.Completed += AsyncDispatchCenter;
            SendEvent.UserToken = Session;
            SendEvent.SendAction = ProcessSend;

            //初始化接收接套字
            ReceiveBuffer = new byte[BufferSize];
            ReceiveEventArgs ReceiveEvent = new ReceiveEventArgs();
            ReceiveEvent.Completed += AsyncDispatchCenter;
            ReceiveEvent.UserToken = Session;
            ReceiveEvent.ReceiveAction = ProcessReceive;
            ReceiveEvent.SetBuffer(ReceiveBuffer);

            ShakeHandEvent = new ShakeHandEventArgs(ReceiveEvent, SendEvent);
            ShakeHandEvent.Completed += AsyncDispatchCenter;
            ShakeHandEvent.ConnectAction = StartConnect;
            Session.ShakeHandEvent = ShakeHandEvent;
            Session.Mode = SocketMode.Client;
            ShakeHandEvent.UserToken = Session;
        }

        public IPEndPoint GetEndPoint()
        {
            IPAddress[] address = Dns.GetHostAddresses(Host);
            if (address.Length == 0) throw new ArgumentNullException("Host to dns analytics is null!");
            return new IPEndPoint(address.FirstOrDefault(), Port);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void StartConnect()
        {
            Client = new Socket(AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //设置连接地址
            ShakeHandEvent.RemoteEndPoint = GetEndPoint();
            UserTokenSession Session = (UserTokenSession)ShakeHandEvent.UserToken;
            Console.WriteLine("进行连接....是否处于正在连接中:" + Session.Connecting);
            Session.Connecting = true;
            if (!Client.ConnectAsync(ShakeHandEvent))
            {
                ProcessConnect(ShakeHandEvent);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void AsyncDispatchCenter(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                //检测到连接行为
                case SocketAsyncOperation.Connect:
                    ProcessConnect(e);
                    break;
                //检测到接收行为
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ProcessConnect(SocketAsyncEventArgs e)
        {
            ShakeHandEventArgs eventArgs = (ShakeHandEventArgs)e;
            UserTokenSession Session = (UserTokenSession)e.UserToken;
            //结束连接中状态
            Session.Connecting = false;
            switch (eventArgs.SocketError)
            {
                case SocketError.Success:
                    Session.OperationTime = DateTime.Now;

                    //异步监听连接完成后的操作
                    ThreadPool.QueueUserWorkItem((e) => OnConnect?.Invoke(Session));

                    //接收服务端传来的流
                    if (!Session.Channel.ReceiveAsync(eventArgs.ReceiveEventArgs))
                    {
                        ProcessReceive(eventArgs.ReceiveEventArgs);
                    }

                    //如果存在发送失败的消息 在重连之后
                    while (Session.NoSuccessMessage.Count > 0)
                    {
                        if (Session.NoSuccessMessage.TryDequeue(out var message))
                            Session.Channel.Send(message);
                    }
                    break;
                case SocketError.ConnectionRefused:
                    LogHelper.LogError("服务器拒绝连接");
                    break;
                default:
                    LogHelper.LogWarn("发现未采集的接套字状态:" + eventArgs.SocketError);
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ProcessSend(SocketAsyncEventArgs e)
        {
            SendEventArgs eventArgs = (SendEventArgs)e;
            UserTokenSession UserToken = (UserTokenSession)eventArgs.UserToken;
            switch (eventArgs.SocketError)
            {
                //需要重新连接
                case SocketError.ConnectionReset:
                    UserToken.NoSuccessMessage.Enqueue(eventArgs.MemoryBuffer.ToArray());
                    StartConnect();
                    break;
                //成功就不需要任何操作了
                case SocketError.Success:
                    break;
                default:
                    LogHelper.LogWarn("发现未采集的接套字状态:" + eventArgs.SocketError);
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ProcessReceive(SocketAsyncEventArgs e)
        {
            ReceiveEventArgs eventArgs = (ReceiveEventArgs)e;
            UserTokenSession UserToken = (UserTokenSession)eventArgs.UserToken;
            if (eventArgs.SocketError == SocketError.Success && eventArgs.BytesTransferred > 0)
            {
                //解码回调
                eventArgs.Decode((bytes) => OnCallBack?.Invoke(UserToken, bytes));

                //继续接收消息
                if (UserToken.Channel != null
                    && !UserToken.Channel.ReceiveAsync(e))
                {
                    //此次接收没有接收完毕 递归接收
                    ProcessReceive(e);
                }
            }
        }

        /// <summary>
        /// 接收结果回调
        /// </summary>
        public Action<UserTokenSession, Packet> OnCallBack { get; set; }

        /// <summary>
        /// 连接成功回调
        /// </summary>
        public Action<UserTokenSession> OnConnect { get; set; }
    }
}
