using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BigCookieKit.Communication
{
    public class NetworkClient : Network, ICilent
    {
        public NetworkClient(string Host, int Port) : base(Host, Port)
        {
        }

        internal XSocket Server { get; set; }

        public void DispatchCenter(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    ProcessConnect(e);
                    break;
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
            }
        }

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
            Server.Mode = ApplyMode.Client;
            ThreadPool.QueueUserWorkItem(e => Open());
        }

        public void Open()
        {
            Session session = new Session(DispatchCenter)
            {
                Server = Server,
                Mode = Server.Mode,
                Encoder = Encoder,
                RemoteEndPoint = GetAddress(),
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

            if (!Server.ConnectAsync(session))
            {
                ProcessConnect(session);
            }
        }

        public void ProcessConnect(SocketAsyncEventArgs e)
        {
            Session session = (Session)e.UserToken;
            if (session.SocketError == SocketError.Success)
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
                Console.WriteLine($"ProcessConnect:[{session.SocketError.ToString()}]");
            }
        }

        public void ProcessReceive(SocketAsyncEventArgs e)
        {
            Session session = (Session)e.UserToken;
            if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
            {
                session.ReceiveHandle
                    .Decode(packet =>
                    OnCallBack?.Invoke(session, packet));

                if (!session.m_Socket
                    .ReceiveAsync(e))
                    ProcessReceive(e);
            }
        }

        public void ProcessSend(SocketAsyncEventArgs e)
        {
        }

        public Action<Session, byte[]> OnCallBack { get; set; }

        public Action<Session> OnConnect { get; set; }
    }
}
