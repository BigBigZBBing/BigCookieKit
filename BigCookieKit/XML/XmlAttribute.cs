using System.Runtime.InteropServices;

namespace BigCookieKit.XML
{
    /// <summary>
    /// XML标签的属性
    /// <code>
    /// **用结构体来降低堆的利用
    /// </code>
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct XmlAttribute
    {
        public string Name;

        public string Text;
    }
}
