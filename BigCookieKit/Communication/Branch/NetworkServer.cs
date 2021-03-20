using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System.Net;
using System.Buffers;

namespace BigCookieKit.Communication
{
    /// <summary>
    /// 基础网络服务端
    /// </summary>
    public class NetworkServer : Network, IServer
    {
        public NetworkServer(int Port) : base("127.0.0.1", Port)
        {
        }

        public int MaxConnect { get; set; } = 200;

        public Action<Session> OnConnect { get; set; }

        public Action<Session, byte[]> OnReceive { get; set; }

        public Action<Session> OnExit { get; set; }

        internal XSocket Server { get; set; }

        internal ConcurrentQueue<Session> Pool = new ConcurrentQueue<Session>();

        public void Start()
        {
            switch (Protocol)
            {
                case NetworkProtocol.Tcp:
                case NetworkProtocol.Http1:
                case NetworkProtocol.Http2:
                    Server = new XSocket(AddressFamily, SocketType, ProtocolType.Tcp);
                    break;
                case NetworkProtocol.Udp:
                    Server = new XSocket(AddressFamily, SocketType, ProtocolType.Udp);
                    break;
            }
            Server.Mode = ApplyMode.Server;
            Server.Bind(GetAddress());
            Server.Listen(MaxConnect);

            ThreadPool.QueueUserWorkItem(e => Open());
        }

        public void Open()
        {
            Session session = Pool.TryDequeue(out Session oldSession)
                ? oldSession
                : new Session(DispatchCenter)
                {
                    Server = Server,
                    Mode = Server.Mode,
                    Encoder = Encoder,
                    ReceiveHandle = Handle.Define(BufferPool.Rent(BufferSize), DispatchCenter) ?? Protocol switch
                    {
                        NetworkProtocol.Tcp => new TcpHandle().Define(BufferPool.Rent(BufferSize), DispatchCenter),
                        NetworkProtocol.Udp => throw new NotImplementedException(),
                        NetworkProtocol.Http1 => throw new NotImplementedException(),
                        NetworkProtocol.Http2 => throw new NotImplementedException(),
                        _ => throw new NotImplementedException(),
                    },
                    SendHandle = Handle.Define(DispatchCenter) ?? Protocol switch
                    {
                        NetworkProtocol.Tcp => new TcpHandle().Define(DispatchCenter),
                        NetworkProtocol.Udp => throw new NotImplementedException(),
                        NetworkProtocol.Http1 => throw new NotImplementedException(),
                        NetworkProtocol.Http2 => throw new NotImplementedException(),
                        _ => throw new NotImplementedException(),
                    },
                };
            if (!Server.AcceptAsync(session))
            {
                ProcessAccept(session);
            }
        }

        public void ProcessAccept(SocketAsyncEventArgs e)
        {
            Session session = (Session)e.UserToken;
            if (e.LastOperation == SocketAsyncOperation.Accept
                && e.SocketError == SocketError.Success)
            {
                session.UserCode = Guid.NewGuid().ToString("D");
                var EndPoint = (IPEndPoint)session.m_Socket.RemoteEndPoint;
                var AllHost = Dns.GetHostEntry(EndPoint.Address).AddressList;
                session.UserHost = string.Join("|", AllHost.Select(x => x.ToString()).ToArray());
                session.UserPort = EndPoint.Port;
                session.OperationTime = DateTime.Now;
                session.ReceiveHandle.UserToken = session;

                OnConnect?.Invoke(session);

                if (!session.m_Socket
                    .ReceiveAsync(session.ReceiveHandle))
                    ProcessReceive(session.ReceiveHandle);
            }
            else
            {
                Pool.Enqueue(session);
            }
            ThreadPool.QueueUserWorkItem(e => Open());
        }

        public void ProcessReceive(SocketAsyncEventArgs e)
        {
            Session session = (Session)e.UserToken;
            if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
            {
                session.ReceiveHandle
                    .Decode(packet =>
                    OnReceive?.Invoke(session, packet));

                if (!session.m_Socket
                    .ReceiveAsync(e))
                    ProcessReceive(e);
            }
            else
            {
                OnExit?.Invoke(session);
                Pool.Enqueue(session);
            }
        }

        public void ProcessSend(SocketAsyncEventArgs e)
        {
        }

        public void DispatchCenter(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    ProcessAccept(e);
                    break;
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
            }
        }
    }
}
