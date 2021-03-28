using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Communication
{
    public class NoneHandle : Handle
    {
        public override void Encode(byte[] bytes)
        {
            Communication.Buffer.SetBuffer(this, bytes);
        }

        public override void Decode(Action<byte[]> packet)
        {
            packet(MemoryBuffer.ToArray());
        }
    }
}
