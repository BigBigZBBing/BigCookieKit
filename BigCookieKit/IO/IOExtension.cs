using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BigCookieKit.IO
{
    public static class IOExtension
    {
        public static Encoding encode = Encoding.UTF8;

        public static Stream ToStream(this string text)
        {
            var stream = new MemoryStream();
            stream.Write(encode.GetBytes(text));
            return stream;
        }
    }
}
