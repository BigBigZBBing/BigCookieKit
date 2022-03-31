using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace BigCookieKit.XML
{
    /// <summary>
    /// 解析的XML的实体
    /// </summary>
    public class XmlPacket
    {
        #region 内部参数

        internal XmlPacket Parent { get; set; }

        internal PacketState State { get; set; }

        #endregion

        public XmlInfo Info;

        public IList<XmlPacket> Node { get; set; }

        public XmlAttribute GetAttr(string key)
        {
            var attrs = Info.Attributes;
            if (attrs.NotExist()) return default;
            for (int i = 0; i < attrs.Length; i++)
            {
                var attr = attrs[i];
                if (attr.Name.Equals(key, StringComparison.OrdinalIgnoreCase))
                    return attr;
            }
            return default;
        }
    }
}
