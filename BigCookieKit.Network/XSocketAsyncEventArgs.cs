using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Network
{
    public class XSocketAsyncEventArgs : SocketAsyncEventArgs
    {
#if NET452
        public Memory<byte> MemoryBuffer { get { return this.Buffer; } }
#endif
        internal ApplyMode Mode { get; set; }

        public Socket Client
        {
            get => Mode switch
            {
                ApplyMode.Client => ConnectSocket,
                ApplyMode.Server => AcceptSocket,
                _ => null
            };
            internal set
            {
                switch (Mode)
                {
                    case ApplyMode.Client:
                        break;
                    case ApplyMode.Server:
                        AcceptSocket = value;
                        break;
                }
            }
        }
    }
}
