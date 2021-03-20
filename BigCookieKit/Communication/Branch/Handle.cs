﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Communication
{
    public class Handle : XSocketAsyncEventArgs
    {
        public Handle()
        {
        }

        private byte[] _buffer;
        private EventHandler<SocketAsyncEventArgs> _callback;
        internal byte[] buffer { get { return _buffer; } set { Communication.Buffer.SetBuffer(this, value); } }

        internal EventHandler<SocketAsyncEventArgs> callback { get { return _callback; } set { Completed += value; } }

        public virtual void Encode(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public virtual void Decode(Action<byte[]> packet)
        {
            throw new NotImplementedException();
        }
    }
}
