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

        internal ConcurrentQueue<Session> Pool = new ConcurrentQueue<Session>();

        public void Start()
        {
            switch (Protocol)
            {
                case NetworkProtocol.Tcp:
                case NetworkProtocol.Http1:
                case NetworkProtocol.Http2:
                    CurrSocket = new XSocket(AddressFamily, SocketType, ProtocolType.Tcp);
                    break;
                case NetworkProtocol.Udp:
                    CurrSocket = new XSocket(AddressFamily, SocketType, ProtocolType.Udp);
                    break;
            }

            CurrSocket.Mode = ApplyMode.Server;
            CurrSocket.Bind(GetAddress());
            CurrSocket.Listen(MaxConnect);

            ((IServer)this).Open();
        }

        void IServer.Open()
        {
            bool isOld = Pool.TryDequeue(out Session oldSession);
            Session session = isOld ? oldSession
                : new Session(((IServer)this).DispatchCenter)
                {
                    Server = CurrSocket,
                    Mode = CurrSocket.Mode,
                    Encoder = Encoder,
                    RecHandle = Handle?.New().Define(BufferPool.Rent(BufferSize), ((IServer)this).DispatchCenter) ?? Protocol switch
                    {
                        NetworkProtocol.Tcp => new TcpHandle().Define(BufferPool.Rent(BufferSize), ((IServer)this).DispatchCenter),
                        NetworkProtocol.Udp => new NoneHandle().Define(BufferPool.Rent(BufferSize), ((IServer)this).DispatchCenter),
                        NetworkProtocol.Http1 => new NoneHandle().Define(BufferPool.Rent(BufferSize), ((IServer)this).DispatchCenter),
                        NetworkProtocol.Http2 => new NoneHandle().Define(BufferPool.Rent(BufferSize), ((IServer)this).DispatchCenter),
                        _ => throw new NotImplementedException(),
                    },
                    SendHandle = Handle?.New().Define(((IServer)this).DispatchCenter) ?? Protocol switch
                    {
                        NetworkProtocol.Tcp => new TcpHandle().Define(((IServer)this).DispatchCenter),
                        NetworkProtocol.Udp => new NoneHandle().Define(BufferPool.Rent(BufferSize), ((IServer)this).DispatchCenter),
                        NetworkProtocol.Http1 => new NoneHandle().Define(BufferPool.Rent(BufferSize), ((IServer)this).DispatchCenter),
                        NetworkProtocol.Http2 => new NoneHandle().Define(BufferPool.Rent(BufferSize), ((IServer)this).DispatchCenter),
                        _ => throw new NotImplementedException(),
                    },
                };
            if (!CurrSocket.AcceptAsync(session))
            {
                ((IServer)this).ProcessAccept(session);
            }
        }

        void IServer.ProcessAccept(SocketAsyncEventArgs e)
        {
            Session session = (Session)e.UserToken;
            if (e.LastOperation == SocketAsyncOperation.Accept
                && e.SocketError == SocketError.Success)
            {
                session.UserCode = Guid.NewGuid().ToString("D");
                var EndPoint = (IPEndPoint)session.Client.RemoteEndPoint;
                var AllHost = Dns.GetHostEntry(EndPoint.Address).AddressList;
                session.UserHost = string.Join("|", AllHost.Select(x => x.ToString()).ToArray());
                session.UserPort = EndPoint.Port;
                session.OperationTime = DateTime.Now;
                session.RecHandle.UserToken = session;

                OnConnect?.Invoke(session);

                if (!session.Client
                        .ReceiveAsync(session.RecHandle))
                    ((IServer)this).ProcessReceive(session.RecHandle);
            }
            else
            {
                Pool.Enqueue(session);
            }
            ThreadPool.QueueUserWorkItem(e => ((IServer)this).Open());
        }

        void IServer.ProcessReceive(SocketAsyncEventArgs e)
        {
            Session session = (Session)e.UserToken;
            if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
            {
                session.RecHandle
                    .Decode(packet =>
                    {
                        session.RecHandle.PipeStart(session, delegate
                        {
                            OnReceive?.Invoke(session, packet);
                        });
                    });

                if (!session.Client
                        .ReceiveAsync(e))
                    ((IServer)this).ProcessReceive(e);
            }
            else
            {
                OnExit?.Invoke(session);
                session.Disconnect();
                Pool.Enqueue(session);
            }
        }

        void IServer.ProcessSend(SocketAsyncEventArgs e)
        {
        }

        void IServer.DispatchCenter(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    ((IServer)this).ProcessAccept(e);
                    break;
                case SocketAsyncOperation.Receive:
                    ((IServer)this).ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ((IServer)this).ProcessSend(e);
                    break;
            }
        }
    }
}
