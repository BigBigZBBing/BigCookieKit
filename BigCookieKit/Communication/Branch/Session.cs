using BigCookieKit.Reflect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BigCookieKit.Communication
{
    public class Session : XSocketAsyncEventArgs
    {
        public Encoding Encoder { get; set; }

        public string UserCode { get; set; }

        public string UserHost { get; set; }

        public int? UserPort { get; set; }

        public DateTime? OperationTime { get; set; }

        public XSocket Server { get; set; }

        public Handle ReceiveHandle { get; set; }

        public Handle SendHandle { get; set; }

        internal EofStream BufferBody;

        internal EofStream BufferHead;

        internal int ReceiveType;

        internal int ReceiveCapacity;

        internal int ReadOffset;

        internal static Func<SocketAsyncEventArgs, bool> EnsureFree =
            SmartBuilder.DynamicMethod<Func<SocketAsyncEventArgs, bool>>("EnsureFree", il =>
            {
                var saea = il.NewObject(il.ArgumentRef<SocketAsyncEventArgs>(0));
                var isFree = il.NewInt32(saea.GetField(
#if NET452
            "m_Operating"
#else
            "_operating"
#endif
                    )) == 0;
                isFree.Output();
                il.Return();
            });

        public Session()
        {
            this.UserToken = this;
            BufferBody = new EofStream();
            BufferHead = new EofStream();
        }

        public Session(EventHandler<SocketAsyncEventArgs> _event) : this()
        {
            Completed += _event;
        }

        public bool SendMessage(string message)
        {
            byte[] bytes = Encoder.GetBytes(message);
            return SendMessage(bytes);
        }

        public bool SendMessage(byte[] message)
        {
            if (!EnsureSafe()) return false;
            SendHandle.Encode(message);
            Client.SendAsync(SendHandle);
            return ValidationState();
        }

        public bool Disconnect()
        {
            Client.Shutdown(SocketShutdown.Both);
            return true;
            //if (!EnsureSafe()) return false;
            //Communication.Buffer.SetBuffer(SendHandle, new byte[] { 255, 0 });
            //Client.SendAsync(SendHandle);
            //return ValidationState();
        }

        private bool ValidationState()
        {
            switch (SendHandle.SocketError)
            {
                case SocketError.Success:
                    return true;
                default: return false;
            }
        }

        private bool EnsureSafe()
        {
            if (!Client.Connected) return false;
            SpinWait sw = default;
            while (!EnsureFree(SendHandle))
                sw.SpinOnce();
            return true;
        }
    }
}
