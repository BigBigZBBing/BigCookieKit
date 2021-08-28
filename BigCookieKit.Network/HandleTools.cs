using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Network
{
    public static class HandleTools
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

        public static void AddPipe<T>(this Handle handle) where T : IPipe
        {
            var midType = typeof(T);
            handle.pipeline.Add((IPipe)Activator.CreateInstance(midType));
        }

        public static void AddPipe(this Handle handle, Type midType)
        {
            if (!(midType.BaseType is IPipe)) throw new ArrayTypeMismatchException();
            handle.pipeline.Add((IPipe)Activator.CreateInstance(midType));
        }
    }
}
