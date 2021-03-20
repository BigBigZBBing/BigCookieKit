using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Communication
{
    internal static class HandleTools
    {
        internal static Handle Define(this Handle handle, EventHandler<SocketAsyncEventArgs> _event)
        {
            if (handle.NotNull())
            {
                handle.callback = _event;
            }
            return handle;
        }

        internal static Handle Define(this Handle handle, byte[] buffer, EventHandler<SocketAsyncEventArgs> _event)
        {
            if (handle.NotNull())
            {
                handle.buffer = buffer;
                handle.callback = _event;
            }
            return handle;
        }
    }
}
