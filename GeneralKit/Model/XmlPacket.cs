using System;
using System.Collections.Generic;
using System.Text;

namespace BigCookieKit.Model
{
    public class XmlPacket
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public XmlAttr[] Attributes { get; set; }
    }

    public class XmlAttr
    {
        public string Name { get; set; }

        public string Text { get; set; }
    }
}
