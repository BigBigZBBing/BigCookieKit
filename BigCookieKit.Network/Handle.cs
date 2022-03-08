using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Network
{
    public abstract class Handle : XSocketAsyncEventArgs
    {
        public Handle()
        {
        }

        private byte[] _buffer;

        private EventHandler<SocketAsyncEventArgs> _callback;

        internal byte[] buffer { get { return _buffer; } set { BigCookieKit.Network.Buffer.SetBuffer(this, value); } }

        internal EventHandler<SocketAsyncEventArgs> callback { get { return _callback; } set { Completed += value; } }

        internal Pipeline pipeline = new Pipeline();

        internal Handle New()
        {
            Handle handle = (Handle)Activator.CreateInstance(GetType());
            handle.pipeline = pipeline;
            return handle;
        }

        public void PipeStart(Session session, Action callback)
        {
            pipeline.Start(session, callback);
        }

        public abstract void Encode(byte[] bytes);

        public abstract void Decode(Action<byte[]> packet);
    }
}
