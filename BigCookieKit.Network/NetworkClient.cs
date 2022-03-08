using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BigCookieKit.Network
{
    public class NetworkClient : Network, ICilent
    {
        public NetworkClient(string Host, int Port) : base(Host, Port)
        {
        }

        void ICilent.DispatchCenter(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    ((ICilent)this).ProcessConnect(e);
                    break;
                case SocketAsyncOperation.Receive:
                    ((ICilent)this).ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ((ICilent)this).ProcessSend(e);
                    break;
            }
        }

        public void Start()
        {
            if (CurrSocket == null)
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
            CurrSocket.Mode = ApplyMode.Client;
            ((ICilent)this).Open();
        }

        public void Close()
        {
            CurrSocket.Shutdown(SocketShutdown.Both);
        }

        void ICilent.Open()
        {
            Session session = new Session(((ICilent)this).DispatchCenter)
            {
                Server = CurrSocket,
                Mode = CurrSocket.Mode,
                Encoder = Encoder,
                RemoteEndPoint = GetAddress(),
                RecHandle = Handle?.New().Define(BufferPool.Rent(BufferSize), ((ICilent)this).DispatchCenter) ?? Protocol switch
                {
                    NetworkProtocol.Tcp => new TcpHandle().Define(BufferPool.Rent(BufferSize), ((ICilent)this).DispatchCenter),
                    NetworkProtocol.Udp => new NoneHandle().Define(BufferPool.Rent(BufferSize), ((ICilent)this).DispatchCenter),
                    NetworkProtocol.Http1 => throw new NotImplementedException(),
                    NetworkProtocol.Http2 => throw new NotImplementedException(),
                    NetworkProtocol.None => new NoneHandle().Define(BufferPool.Rent(BufferSize), ((ICilent)this).DispatchCenter),
                    _ => throw new NotImplementedException(),
                },
                SendHandle = Handle?.New().Define(((ICilent)this).DispatchCenter) ?? Protocol switch
                {
                    NetworkProtocol.Tcp => new TcpHandle().Define(((ICilent)this).DispatchCenter),
                    NetworkProtocol.Udp => new NoneHandle().Define(BufferPool.Rent(BufferSize), ((ICilent)this).DispatchCenter),
                    NetworkProtocol.Http1 => throw new NotImplementedException(),
                    NetworkProtocol.Http2 => throw new NotImplementedException(),
                    NetworkProtocol.None => new NoneHandle().Define(BufferPool.Rent(BufferSize), ((ICilent)this).DispatchCenter),
                    _ => throw new NotImplementedException(),
                },
            };

            if (!CurrSocket.ConnectAsync(session))
            {
                ((ICilent)this).ProcessConnect(session);
            }
        }

        void ICilent.ProcessConnect(SocketAsyncEventArgs e)
        {
            Session session = (Session)e.UserToken;
            if (session.SocketError == SocketError.Success)
            {
                session.UserCode = Guid.NewGuid().ToString("D");
                var EndPoint = (IPEndPoint)session.Client.RemoteEndPoint;
                var AllHost = Dns.GetHostEntry(EndPoint.Address).AddressList;
                session.UserHost = string.Join("|", AllHost.Select(x => x.ToString()).ToArray());
                session.UserPort = EndPoint.Port;
                session.OperationTime = DateTime.Now;
                session.RecHandle.UserToken = session;

                ThreadPool.QueueUserWorkItem(e => OnConnect?.Invoke(session));

                if (Certificate != null)
                {
                    session.SendMessage(Certificate.GetRawCertData());
                }

                if (!session.Client
                        .ReceiveAsync(session.RecHandle))
                    ((ICilent)this).ProcessReceive(session.RecHandle);
            }
            else
            {
                Console.WriteLine($"ProcessConnect:[{session.SocketError.ToString()}]");
            }
        }

        void ICilent.ProcessReceive(SocketAsyncEventArgs e)
        {
            Session session = (Session)e.UserToken;
            if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
            {
                session.RecHandle
                .Decode(packet =>
                {
                    session.RecHandle.PipeStart(session, delegate
                    {
                        OnCallBack?.Invoke(session, packet);
                    });
                });

                if (!session.Client
                        .ReceiveAsync(e))
                    ((ICilent)this).ProcessReceive(e);
            }
        }

        void ICilent.ProcessSend(SocketAsyncEventArgs e)
        {
        }

        public Action<Session, byte[]> OnCallBack { get; set; }

        public Action<Session> OnConnect { get; set; }
    }
}
