using System;
using System.Collections.Generic;
using System.Text;

namespace BigCookieKit.Algorithm
{
    public class BoyerMoore
    {
        /// <summary>
        /// 博伊尔-摩尔算法匹配单次
        /// </summary>
        /// <param name="source">内容</param>
        /// <param name="pattern">模式</param>
        /// <param name="offset">初始偏移量</param>
        /// <returns></returns>
        public static int BoyerMooreFirstMatch(byte[] source, byte[] pattern, int offset = 0)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            var bads = new int[256];
            var total = source.Length;
            var length = pattern.Length;
            var b_length = total - length;
            var last = length - 1;

            if (total == 0 || length == 0 || length > total) return -1;

            for (var i = 0; i < 256; i++)
            {
                bads[i] = length;
            }

            for (var i = last; i >= 0; i--)
            {
                var pet = pattern[i];
                for (int t = 0; t < i; t++)
                {
                    if (pet == pattern[t])
                    {
                        bads[pet] = length - t;
                        break;
                    }
                }
            }

            while (offset <= b_length)
            {
                int i;
                for (i = last; source[offset + i] == pattern[i]; i--)
                {
                    if (i == 0) return offset;
                }
                offset += bads[source[offset + i]];
            }

            return -1;
        }

        /// <summary>
        /// 博伊尔-摩尔算法匹配单次
        /// </summary>
        /// <param name="source">内容</param>
        /// <param name="pattern">模式</param>
        /// <param name="offset">初始偏移量</param>
        /// <returns></returns>
        public static int BoyerMooreFirstMatch(char[] source, char[] pattern, int offset = 0)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            var bads = new int[256];
            var total = source.Length;
            var length = pattern.Length;
            var b_length = total - length;
            var last = length - 1;

            if (total == 0 || length == 0 || length > total) return -1;

            for (var i = 0; i < 256; i++)
            {
                bads[i] = length;
            }

            for (var i = last; i >= 0; i--)
            {
                var pet = pattern[i];
                for (int t = 0; t < i; t++)
                {
                    if (pet == pattern[t])
                    {
                        bads[pet] = length - t;
                        break;
                    }
                }
            }

            while (offset <= b_length)
            {
                int i;
                for (i = last; source[offset + i] == pattern[i]; i--)
                {
                    if (i == 0) return offset;
                }
                offset += bads[source[offset + i]];
            }

            return -1;
        }

        /// <summary>
        /// 博伊尔-摩尔算法匹配所有
        /// </summary>
        /// <param name="source">内容</param>
        /// <param name="pattern">模式</param>
        /// <param name="offset">初始偏移量</param>
        /// <returns></returns>
        public static int[] BoyerMooreMatchAll(byte[] source, byte[] pattern, int offset = 0)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            List<int> res = new List<int>();
            var bads = new int[256];
            var total = source.Length;
            var length = pattern.Length;
            var b_length = total - length;
            var last = length - 1;

            if (total == 0 || length == 0 || length > total) return res.ToArray();

            for (var i = 0; i < 256; i++)
            {
                bads[i] = length;
            }

            for (var i = last; i >= 0; i--)
            {
                var pet = pattern[i];
                for (int t = 0; t < i; t++)
                {
                    if (pet == pattern[t])
                    {
                        bads[pet] = length - t;
                        break;
                    }
                }
            }

            while (offset <= b_length)
            {
                int i;
                for (i = last; source[offset + i] == pattern[i]; i--)
                {
                    if (i == 0)
                    {
                        res.Add(offset);
                        break;
                    }
                }
                offset += bads[source[offset + i]];
            }

            return res.ToArray();
        }

        /// <summary>
        /// 博伊尔-摩尔算法匹配所有
        /// </summary>
        /// <param name="source">内容</param>
        /// <param name="pattern">模式</param>
        /// <param name="offset">初始偏移量</param>
        /// <returns></returns>
        public static int[] BoyerMooreMatchAll(char[] source, char[] pattern, int offset = 0)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            List<int> res = new List<int>();
            var bads = new int[256];
            var total = source.Length;
            var length = pattern.Length;
            var b_length = total - length;
            var last = length - 1;

            if (total == 0 || length == 0 || length > total) return res.ToArray();

            for (var i = 0; i < 256; i++)
            {
                bads[i] = length;
            }

            for (var i = last; i >= 0; i--)
            {
                var pet = pattern[i];
                for (int t = 0; t < i; t++)
                {
                    if (pet == pattern[t])
                    {
                        bads[pet] = length - t;
                        break;
                    }
                }
            }

            while (offset <= b_length)
            {
                int i;
                for (i = last; source[offset + i] == pattern[i]; i--)
                {
                    if (i == 0)
                    {
                        res.Add(offset);
                        break;
                    }
                }
                offset += bads[source[offset + i]];
            }

            return res.ToArray();
        }
    }
}
