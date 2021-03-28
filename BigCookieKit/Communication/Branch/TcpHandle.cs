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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Encode(byte[] bytes)
        {
            byte[] stream = null;
            if (bytes.Length <= byte.MaxValue)
            {
                stream = new byte[bytes.Length + 2];
                stream[0] = 1;
                stream[1] = (byte)bytes.Length;
                bytes.CopyTo(stream, 2);
            }
            else if (bytes.Length <= short.MaxValue)
            {
                stream = new byte[bytes.Length + 3];
                stream[0] = 2;
                Kit.GetBytes((short)bytes.Length).CopyTo(stream, 1);
                bytes.CopyTo(stream, 3);
            }
            else if (bytes.Length <= int.MaxValue)
            {
                stream = new byte[bytes.Length + 5];
                stream[0] = 3;
                Kit.GetBytes(bytes.Length).CopyTo(stream, 1);
                bytes.CopyTo(stream, 5);
            }
            Communication.Buffer.SetBuffer(this, stream);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Decode(Action<byte[]> packet)
        {
            Session session = (Session)UserToken;
            EofStream eof = session.BufferHead;
        loop:
            if (session.ReceiveType == 0)
            {
                session.ReceiveType = MemoryBuffer.Span[session.ReadOffset++];
                if (session.ReadOffset == MemoryBuffer.Length)
                {
                    session.ReadOffset = 0;
                    return;
                }
            }

            if (session.ReceiveCapacity == 0 && session.ReadOffset < MemoryBuffer.Length)
            {
                int pos;
                switch (session.ReceiveType)
                {
                    case 1:
                        if (!EnsureOffSet(1)) return;
                        session.ReceiveCapacity = MemoryBuffer.Slice(session.ReadOffset, 1).Span[0];
                        session.ReadOffset += 1;
                        break;
                    case 2:
                        if (!EnsureOffSet(2)) return;
                        pos = 2 - eof.Count;
                        eof.AddRange(MemoryBuffer.Slice(session.ReadOffset, pos).ToArray());
                        session.ReadOffset += pos;
                        session.ReceiveCapacity = Kit.BitToInt16(eof.ToArray());
                        break;
                    case 3:
                        if (!EnsureOffSet(4)) return;
                        pos = 4 - eof.Count;
                        eof.AddRange(MemoryBuffer.Slice(session.ReadOffset, pos).ToArray());
                        session.ReadOffset += pos;
                        session.ReceiveCapacity = Kit.BitToInt32(eof.ToArray());
                        break;
                    case 255: session.Client?.Shutdown(SocketShutdown.Both); session.ReceiveType = 0; return;
                }
                int remaining = BytesTransferred - session.ReadOffset;
                byte[] bytes = MemoryBuffer.Slice(session.ReadOffset,
                    session.ReceiveCapacity > remaining
                    ? remaining
                    : session.ReceiveCapacity).ToArray();
                session.BufferBody.AddRange(bytes);
                session.ReadOffset += bytes.Length;
            }
            else if (session.ReceiveCapacity > session.BufferBody.Count)
            {
                int due = session.ReceiveCapacity - session.BufferBody.Count;
                int remaining = BytesTransferred - session.ReadOffset;
                byte[] bytes = MemoryBuffer.Slice(session.ReadOffset,
                   due > remaining
                   ? remaining
                   : due).ToArray();
                session.BufferBody.AddRange(bytes);
                session.ReadOffset += bytes.Length;
            }

            if (session.ReceiveCapacity == session.BufferBody.Count)
            {
                packet.Invoke(session.BufferBody.ToArray());
                session.ReceiveType = 0;
                session.ReceiveCapacity = 0;
                session.BufferBody.Clear();
                if (eof.Count > 0) eof.Clear();
            }
            if (session.ReadOffset < BytesTransferred) goto loop;
            session.ReadOffset = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool EnsureOffSet(int bits)
        {
            Session session = (Session)UserToken;
            EofStream eof = session.BufferHead;
            int target = session.ReadOffset + bits;
            if (target > MemoryBuffer.Length)
            {
                if (bits > 1)
                {
                    int remain = MemoryBuffer.Length - session.ReadOffset;
                    eof.AddRange(MemoryBuffer.Slice(session.ReadOffset, remain).ToArray());
                }
                session.ReadOffset = 0;
                return false;
            }
            return true;
        }
    }
}
