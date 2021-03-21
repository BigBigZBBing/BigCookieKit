using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Communication
{
    public abstract class Handle : XSocketAsyncEventArgs
    {
        public Handle()
        {
        }

        private byte[] _buffer;

        private EventHandler<SocketAsyncEventArgs> _callback;

        internal byte[] buffer { get { return _buffer; } set { Communication.Buffer.SetBuffer(this, value); } }

        internal EventHandler<SocketAsyncEventArgs> callback { get { return _callback; } set { Completed += value; } }

        internal Pipeline pipeline = new Pipeline();

        internal Handle New()
        {
            Handle handle = (Handle)Activator.CreateInstance(GetType());
            handle.pipeline = pipeline;
            return handle;
        }

        public void PipeStart(Action callback)
        {
            pipeline.Start(callback);
        }

        public abstract void Encode(byte[] bytes);

        public abstract void Decode(Action<byte[]> packet);
    }
}
