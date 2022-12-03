using System;
using System.Collections.Generic;
using System.Text;

namespace BigCookieKit.Algorithm
{
    /// <summary>
    /// LZW压缩算法
    /// </summary>
    public static class LZW
    {
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="uncompressed">字符串</param>
        /// <returns>字节流</returns>
        public static List<int> Compress(string uncompressed)
        {
            //初始化ASCII码表
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            for (int i = 0; i < 256; i++)
                dictionary.Add(((char)i).ToString(), i);

            string w = string.Empty;
            List<int> compressed = new List<int>();

            foreach (char c in uncompressed)
            {
                string wc = w + c;
                if (dictionary.ContainsKey(wc))
                {
                    w = wc;
                }
                else
                {
                    compressed.Add(dictionary[w]);
                    dictionary.Add(wc, dictionary.Count);
                    w = c.ToString();
                }
            }

            if (!string.IsNullOrEmpty(w))
                compressed.Add(dictionary[w]);

            return compressed;
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="compressed">字节流</param>
        /// <returns>字符串</returns>
        public static string Decompress(List<int> compressed)
        {
            //初始化ASCII码表
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            for (int i = 0; i < 256; i++)
                dictionary.Add(i, ((char)i).ToString());

            string w = dictionary[compressed[0]];
            compressed.RemoveAt(0);
            StringBuilder decompressed = new StringBuilder(w);

            foreach (int k in compressed)
            {
                string entry = null;
                if (dictionary.ContainsKey(k))
                    entry = dictionary[k];
                else if (k == dictionary.Count)
                    entry = w + w[0];

                decompressed.Append(entry);

                dictionary.Add(dictionary.Count, w + entry[0]);

                w = entry;
            }

            return decompressed.ToString();
        }
    }
}
