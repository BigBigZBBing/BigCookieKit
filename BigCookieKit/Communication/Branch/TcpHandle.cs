using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Communication
{
    public class TcpHandle : Handle
    {
        public TcpHandle() { }

        public override void Encode(byte[] bytes)
        {
            List<byte> buffer = new List<byte>();
            if (bytes.Length <= byte.MaxValue)
            {
                buffer.Add(01);
                buffer.Add((byte)bytes.Length);
            }
            else if (bytes.Length <= short.MaxValue)
            {
                buffer.Add(2);
                buffer.AddRange(BitConverter.GetBytes((short)bytes.Length));
            }
            else if (bytes.Length <= int.MaxValue)
            {
                buffer.Add(3);
                buffer.AddRange(BitConverter.GetBytes(bytes.Length));
            }
            buffer.AddRange(bytes);
            Communication.Buffer.SetBuffer(this, buffer.ToArray().AsMemory());
        }

        public override void Decode(Action<byte[]> packet)
        {
            Session session = (Session)UserToken;
        loop:
            if (session.ReceiveCapacity == 0)
            {
                switch (MemoryBuffer.Span[session.ReadOffset++])
                {
                    case 1: session.ReceiveCapacity = MemoryBuffer.Slice(session.ReadOffset, 1).Span[0]; session.ReadOffset += 1; break;
                    case 2: session.ReceiveCapacity = Kit.BitToInt16(MemoryBuffer.Slice(session.ReadOffset, 2).ToArray()); session.ReadOffset += 2; break;
                    case 3: session.ReceiveCapacity = Kit.BitToInt32(MemoryBuffer.Slice(session.ReadOffset, 4).ToArray()); session.ReadOffset += 4; break;
                }
                byte[] bytes = MemoryBuffer.Slice(session.ReadOffset,
                    session.ReceiveCapacity > BytesTransferred - session.ReadOffset
                    ? BytesTransferred - session.ReadOffset
                    : session.ReceiveCapacity).ToArray();
                session.BufferCache.AddRange(bytes);
                session.ReadOffset += bytes.Length;
            }
            else if (session.ReceiveCapacity > session.BufferCache.Count)
            {
                byte[] bytes = MemoryBuffer.Slice(session.ReadOffset,
                   session.ReceiveCapacity - session.BufferCache.Count > BytesTransferred - session.ReadOffset
                   ? BytesTransferred - session.ReadOffset
                   : session.ReceiveCapacity - session.BufferCache.Count).ToArray();
                session.BufferCache.AddRange(bytes);
                session.ReadOffset += bytes.Length;
            }

            if (session.ReceiveCapacity == session.BufferCache.Count)
            {
                packet.Invoke(session.BufferCache.ToArray());
                session.ReceiveCapacity = 0;
                session.BufferCache.Clear();
            }
            if (session.ReadOffset < BytesTransferred) goto loop;
            session.ReadOffset = 0;
        }
    }
}
