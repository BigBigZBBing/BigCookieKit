using System.Runtime.InteropServices;

namespace BigCookieKit.XML
{
    /// <summary>
    /// Xml的信息数据
    /// <code>
    /// **用结构体来降低堆的利用
    /// </code>
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct XmlInfo
    {
        public bool HasValue;
        public string Name;
        public string Text;
        public XmlAttribute[] Attributes;
    }
}
