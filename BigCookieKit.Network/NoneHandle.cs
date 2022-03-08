using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Network
{
    public class NoneHandle : Handle
    {
        public override void Encode(byte[] bytes)
        {
            BigCookieKit.Network.Buffer.SetBuffer(this, bytes);
        }

        public override void Decode(Action<byte[]> packet)
        {
            packet(MemoryBuffer.ToArray());
        }
    }
}
