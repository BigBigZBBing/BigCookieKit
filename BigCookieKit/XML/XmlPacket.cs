using System;
using System.Collections;
using System.Collections.Generic;
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
        public bool HasValue { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public XmlAttr[] Attributes { get; set; }
        public IList<XmlPacket> Node { get; set; }
    }

    internal enum PacketState
    {
        None,
        Start,
        End
    }

    /// <summary>
    /// XML标签的属性
    /// </summary>
    public class XmlAttr
    {
        public string Name { get; set; }

        public string Text { get; set; }
    }
}
