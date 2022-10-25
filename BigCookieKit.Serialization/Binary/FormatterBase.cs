using System;
using System.Collections.Generic;
using System.Text;

namespace BigCookieKit.Serialization.Binary
{
    public interface FormatterBase
    {
        Binary Host { get; set; }

        int Priority { get; set; }

        bool Write(object value);

        object Read(Type type);
    }
}
