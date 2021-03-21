using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Communication
{
    public class XSocket : Socket
    {
        public XSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType) : base(addressFamily, socketType, protocolType)
        {
        }

        public ApplyMode Mode { get; set; }

        internal static XSocket Create(Socket socket)
        {
            return new XSocket(socket.AddressFamily, socket.SocketType, socket.ProtocolType);
        }

        public bool Start(SocketAsyncEventArgs e)
        {
            var xe = e as XSocketAsyncEventArgs;
            xe.Mode = Mode;
            switch (Mode)
            {
                case ApplyMode.Server:
                    return AcceptAsync(xe);
                case ApplyMode.Client:
                    return ConnectAsync(xe);
                default: throw new SocketException();
            }
        }
    }
}
