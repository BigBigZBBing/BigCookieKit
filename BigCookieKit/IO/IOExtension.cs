using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BigCookieKit.IO
{
    public static class IOExtension
    {
        public static Encoding encode = Encoding.UTF8;

        /// <summary>
        /// 字符串转成流对象
        /// </summary>
        /// <param name="text">字符串</param>
        /// <param name="begin">流首位</param>
        /// <returns></returns>
        public static Stream ToStream(this string text, bool begin = true)
        {
            var stream = new MemoryStream();
            stream.Write(encode.GetBytes(text));
            if (begin) stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// 获取流内的字节数组
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="close">是否自动关闭</param>
        /// <returns></returns>
        public static byte[] GetBytes(this Stream stream, bool close = false)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            if (close) stream.Close();
            return bytes;
        }

        /// <summary>
        /// 获取流内的字符串
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="close">是否自动关闭</param>
        /// <returns></returns>
        public static string GetString(this Stream stream, bool close = false)
        {
            var sr = new StreamReader(stream);
            string result = sr.ReadToEnd();
            if (close) stream.Close();
            return result;
        }

        /// <summary>
        /// 七位压缩法-压缩
        /// </summary>
        /// <param name="value">64位值</param>
        /// <param name="reverse">是否反转</param>
        /// <returns></returns>
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

        /// <summary>
        /// 七位压缩法-解压缩
        /// </summary>
        /// <param name="value">字节流</param>
        /// <param name="reverse">是否反转</param>
        /// <returns></returns>
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
