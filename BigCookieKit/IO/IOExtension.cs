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

        public static byte[] Encode7Bit(long value, bool reverse = false)
        {
            byte[] stream = new byte[16];
            var count = 0;
            var num = (ulong)value;

            if (reverse)
            {
                do
                {
                    var c = num & 0x7F;
                    if ((ulong)value != num)
                    {
                        c |= 0x80;
                    }
                    stream[count++] = (byte)c;
                    num >>= 7;
                } while (num != 0);
            }
            else
            {
                while (num >= 0x80)
                {
                    stream[count++] = (byte)(num | 0x80);
                    num >>= 7;
                }
                stream[count++] = (byte)num;
            }

            return stream.AsSpan(0, count).ToArray();
        }

        public static long Dncode7Bit(byte[] value, bool reverse = false)
        {
            ulong rs = 0;
            byte n = 0;
            var count = 0;

            if (reverse)
            {
                count = value.Length - 1;
                n = (byte)(7 * count);
                while (true)
                {
                    var bt = value[count];
                    if (count > 0)
                    {
                        rs |= (ulong)(bt & 0x7f) << n;
                    }
                    else { rs |= bt; break; }
                    n -= 7;
                    count--;
                }
            }
            else
            {
                while (true)
                {
                    var bt = value[count++];
                    byte b = bt;
                    rs |= (ulong)(b & 0x7f) << n;
                    if ((b & 0x80) == 0) break;
                    n += 7;
                }
            }

            return (long)rs;
        }
    }
}
