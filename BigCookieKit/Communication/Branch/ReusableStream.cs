using BigCookieKit.Reflect;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Communication
{
    public class ReusableStream : MemoryStream
    {
        public int Count { get => (int)Length; }

        private Action<MemoryStream> _flush;

        public override void Flush()
        {
            if (_flush == null)
            {
                _flush =
                SmartBuilder.DynamicMethod<Action<MemoryStream>>("flush", il =>
                {
                    var stream = il.NewObject(il.ArgumentRef<MemoryStream>(0));
                    stream.SetField("_buffer", il.NewArray<byte>());
                    stream.SetField("_position", 0);
                    stream.SetField("_capacity", 0);
                    stream.SetField("_length", 0);
                    il.Return();
                });
            }
            _flush.Invoke(this);
        }
    }
}
