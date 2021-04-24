using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Communication
{
    public class TcpHandle : Handle
    {
        public override void Encode(byte[] bytes)
        {
            byte[] stream = null;
            if (bytes.Length <= short.MaxValue)
            {
                stream = new byte[bytes.Length + 3];
                stream[0] = (int)MessageMode.MessageShort;
                Kit.GetBytes((short)bytes.Length).CopyTo(stream, 1);
                bytes.CopyTo(stream, 3);
            }
            else if (bytes.Length <= int.MaxValue)
            {
                stream = new byte[bytes.Length + 5];
                stream[0] = (int)MessageMode.MessageInt;
                Kit.GetBytes(bytes.Length).CopyTo(stream, 1);
                bytes.CopyTo(stream, 5);
            }
            Communication.Buffer.SetBuffer(this, stream);
        }

        public override void Decode(Action<byte[]> packet)
        {
            Session session = (Session)UserToken;
            EofStream eof = session.BufferHead;
        loop:
            if (session.ReceiveType == 0)
            {
                session.ReceiveType = MemoryBuffer.Span[session.ReceiveOffset++];
                if (session.ReceiveOffset == MemoryBuffer.Length)
                {
                    session.ReceiveOffset = 0;
                    return;
                }
            }

            if (session.ReceiveCapacity == 0 && session.ReceiveOffset < MemoryBuffer.Length)
            {
                int pos;
                switch (session.ReceiveType)
                {
                    case (int)MessageMode.MessageShort:
                        if (!EnsureOffSet(2)) return;
                        pos = 2 - eof.Count;
                        eof.AddRange(MemoryBuffer.Slice(session.ReceiveOffset, pos).ToArray());
                        session.ReceiveOffset += pos;
                        session.ReceiveCapacity = Kit.BitToInt16(eof.ToArray());
                        break;
                    case (int)MessageMode.MessageInt:
                        if (!EnsureOffSet(4)) return;
                        pos = 4 - eof.Count;
                        eof.AddRange(MemoryBuffer.Slice(session.ReceiveOffset, pos).ToArray());
                        session.ReceiveOffset += pos;
                        session.ReceiveCapacity = Kit.BitToInt32(eof.ToArray());
                        break;
                    case 255: session.Client?.Shutdown(SocketShutdown.Both); session.ReceiveType = 0; return;
                }
                int remaining = BytesTransferred - session.ReceiveOffset;
                byte[] bytes = MemoryBuffer.Slice(session.ReceiveOffset,
                    session.ReceiveCapacity > remaining
                    ? remaining
                    : session.ReceiveCapacity).ToArray();
                session.BufferBody.AddRange(bytes);
                session.ReceiveOffset += bytes.Length;
            }
            else if (session.ReceiveCapacity > session.BufferBody.Count)
            {
                int due = session.ReceiveCapacity - session.BufferBody.Count;
                int remaining = BytesTransferred - session.ReceiveOffset;
                byte[] bytes = MemoryBuffer.Slice(session.ReceiveOffset,
                   due > remaining
                   ? remaining
                   : due).ToArray();
                session.BufferBody.AddRange(bytes);
                session.ReceiveOffset += bytes.Length;
            }

            if (session.ReceiveCapacity == session.BufferBody.Count)
            {
                packet.Invoke(session.BufferBody.ToArray());
                session.ReceiveType = 0;
                session.ReceiveCapacity = 0;
                session.BufferBody.Clear();
                if (eof.Count > 0) eof.Clear();
            }
            if (session.ReceiveOffset < BytesTransferred) goto loop;
            session.ReceiveOffset = 0;
        }

        
        private bool EnsureOffSet(int bits)
        {
            Session session = (Session)UserToken;
            EofStream eof = session.BufferHead;
            int target = session.ReceiveOffset + bits;
            if (target > MemoryBuffer.Length)
            {
                if (bits > 1)
                {
                    int remain = MemoryBuffer.Length - session.ReceiveOffset;
                    eof.AddRange(MemoryBuffer.Slice(session.ReceiveOffset, remain).ToArray());
                }
                session.ReceiveOffset = 0;
                return false;
            }
            return true;
        }
    }
}
