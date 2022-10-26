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
            var ret = -1;
            BoyerMooreMatch(source, pattern, offset, value => { ret = value; return false; });
            return ret;
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
            var ret = -1;
            BoyerMooreMatch(Encoding.Default.GetBytes(source), Encoding.Default.GetBytes(pattern), offset, value => { ret = value; return false; });
            return ret;
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
            List<int> res = new List<int>();
            BoyerMooreMatch(source, pattern, offset, value => { res.Add(value); return true; });
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
            List<int> res = new List<int>();
            BoyerMooreMatch(source, pattern, offset, value => { res.Add(value); return true; });
            return res.ToArray();
        }

        /// <summary>
        /// 博伊尔-摩尔算法匹配单次
        /// </summary>
        /// <param name="source">内容</param>
        /// <param name="pattern">模式</param>
        /// <param name="offset">初始偏移量</param>
        /// <returns></returns>
        public static int BoyerMooreFirstMatch(string source, string pattern, int offset = 0)
        {
            var s1 = Encoding.UTF8.GetBytes(source);
            var p1 = Encoding.UTF8.GetBytes(pattern);
            int pos = BoyerMooreFirstMatch(s1, p1, 0);
            s1 = s1[pos..];
            return source.Length - Encoding.UTF8.GetString(s1).Length;
        }

        /// <summary>
        /// 博伊尔-摩尔算法匹配所有
        /// </summary>
        /// <param name="source">内容</param>
        /// <param name="pattern">模式</param>
        /// <param name="offset">初始偏移量</param>
        /// <returns></returns>
        public static int[] BoyerMooreMatchAll(string source, string pattern, int offset = 0)
        {
            var s1 = Encoding.UTF8.GetBytes(source);
            var p1 = Encoding.UTF8.GetBytes(pattern);
            List<int> res = new List<int>();
            int pos;
            do
            {
                pos = BoyerMooreFirstMatch(s1, p1, 0);
                if (pos < 0) break;
                s1 = s1[pos..];
                res.Add(source.Length - Encoding.UTF8.GetString(s1).Length);
                if (s1.Length <= pattern.Length) break;
                s1 = s1[pattern.Length..];
                if (s1.Length < pattern.Length) break;
            } while (pos > -1);
            return res.ToArray();
        }

        private static void BoyerMooreMatch(char[] source, char[] pattern, int offset, Func<int, bool> callback)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            var map = new bool[256];
            var total = source.Length;
            var length = pattern.Length;
            var b_length = total - length;
            var last = length - 1;

            if (total == 0 || length == 0 || length > total) return;

            //初始化字符是否需要匹配
            for (int i = 0; i < length; i++)
                map[pattern[i]] = true;

            while (offset <= b_length)
            {
                int start = 0;
                int j = offset + last;

                //判断字符是否在查询集合内 不存在则直接跳跃
                if (map[source[j]])
                {
                    bool ismath = false;
                    int end;
                    for (end = last; source[j] == pattern[end]; end--, j--)
                    {
                        //如果全部相等则匹配成功
                        if (end == 0)
                        {
                            ismath = true;
                            break;
                        }
                    }
                    if (ismath)
                    {
                        if (!callback(offset))
                            return;
                        offset += last;
                    }

                    //首位开始比较
                    bool isbad = false;
                    while (start < end)
                    {
                        if (source[j] == pattern[start])
                        {
                            offset += end - start;
                            isbad = true;
                            break;
                        }
                        start++;
                    }
                    if (!isbad) offset += ++end;
                }
                else
                {
                    offset += length;
                }
            }

            return;
        }

        private static void BoyerMooreMatch(byte[] source, byte[] pattern, int offset, Func<int, bool> callback)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            var map = new bool[256];
            var total = source.Length;
            var length = pattern.Length;
            var b_length = total - length;
            var last = length - 1;

            if (total == 0 || length == 0 || length > total) return;

            //初始化字符是否需要匹配
            for (int i = 0; i < length; i++)
                map[pattern[i]] = true;

            while (offset <= b_length)
            {
                int start = 0;
                int end = last;
                int j = offset + last;

                //判断字符是否在查询集合内 不存在则直接跳跃
                if (map[source[j]])
                {
                    bool ismath = false;
                    for (end = last; source[j] == pattern[end]; end--, j--)
                    {
                        //如果全部相等则匹配成功
                        if (end == 0)
                        {
                            ismath = true;
                            break;
                        }
                    }
                    if (ismath)
                    {
                        if (!callback(offset))
                            return;
                        offset += length;
                    }

                    //首位开始比较
                    bool isbad = false;
                    while (start < end)
                    {
                        if (source[j] == pattern[start])
                        {
                            offset += last - start;
                            isbad = true;
                            break;
                        }
                        start++;
                    }
                    if (!isbad) offset += ++end;
                }
                else
                {
                    offset += length;
                }
            }

            return;
        }
    }
}
