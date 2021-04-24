using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Communication
{
    public class EasyHandle : Handle
    {
        public override void Encode(byte[] bytes)
        {
            byte[] stream = new byte[bytes.Length + 2];
            Kit.GetBytes((short)bytes.Length).CopyTo(stream, 0);
            bytes.CopyTo(stream, 2);
            Communication.Buffer.SetBuffer(this, stream);
        }

        public override void Decode(Action<byte[]> packet)
        {
            Session session = (Session)UserToken;
            EofStream eofhead = session.BufferHead;
            EofStream eofbody = session.BufferBody;
        loop:
            if (EnsureHeadComplete())
            {
                if (session.ReceiveCapacity == 0)
                    session.ReceiveCapacity = Kit.BitToInt16(eofhead.ToArray());
                if (EnsureBodyComplete())
                {
                    packet(eofbody.ToArray());
                    session.ReceiveCapacity = 0;
                    eofhead.Clear();
                    eofbody.Clear();
                }
            }
            if (session.ReceiveOffset < BytesTransferred) goto loop;
            session.ReceiveOffset = 0;
        }

        private bool EnsureHeadComplete()
        {
            Session session = (Session)UserToken;
            EofStream eofhead = session.BufferHead;
            if (eofhead.Count < 2)
            {
                int just = 2 - eofhead.Count;
                if (session.ReceiveOffset + just > BytesTransferred)
                {
                    int remain = BytesTransferred - session.ReceiveOffset;
                    eofhead.AddRange(MemoryBuffer.Slice(session.ReceiveOffset, remain));
                    session.ReceiveOffset += remain;
                    return false;
                }
                else
                {
                    eofhead.AddRange(MemoryBuffer.Slice(session.ReceiveOffset, just));
                    session.ReceiveOffset += 2;
                    return true;
                }
            }
            return true;
        }

        private bool EnsureBodyComplete()
        {
            Session session = (Session)UserToken;
            EofStream eofbody = session.BufferBody;
            if (eofbody.Count < session.ReceiveCapacity)
            {
                int just = session.ReceiveCapacity - eofbody.Count;
                if (session.ReceiveOffset + just > BytesTransferred)
                {
                    int remain = BytesTransferred - session.ReceiveOffset;
                    eofbody.AddRange(MemoryBuffer.Slice(session.ReceiveOffset, remain));
                    session.ReceiveOffset += remain;
                    return false;
                }
                else
                {
                    eofbody.AddRange(MemoryBuffer.Slice(session.ReceiveOffset, just));
                    session.ReceiveOffset += just;
                    return true;
                }
            }
            return true;
        }
    }
}
