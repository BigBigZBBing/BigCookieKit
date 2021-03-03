using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Serialization
{
    /// <summary>
    /// 基础序列化器
    /// </summary>
    public class BasicSerializer : ISerializer
    {
        public void StringEncode(String source)
        {
            byte[] bits = Kit.GetBytes(source.ToCharArray());
        }
    }
}
