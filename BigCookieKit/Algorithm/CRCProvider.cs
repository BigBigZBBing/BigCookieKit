using System;
using System.Collections.Generic;
using System.Text;

namespace BigCookieKit.Algorithm
{
    public static class CRCProvider
    {
        private static ulong[] crc_table => InitCrc32Table();

        private static ulong[] InitCrc32Table()
        {
            ulong[] table = new ulong[256];
            for (int i = 0; i < 256; i++)//用++i以提高效率
            {
                ulong c = (ulong)i;
                for (int j = 8; j > 0; j--)
                {
                    if (((uint)c & 1) == 1)// LSM为1
                        c = (c >> 1) ^ 0xEDB88320;//采取反向校验
                    else //0xEDB88320就是CRC-32多项表达式的reversed值
                        c >>= 1;
                }
                table[i] = c;
            }
            return table;
        }

        public static ulong Crc32Verify(byte[] bytes)
        {
            ulong[] T = crc_table;
            ulong C = 0xFFFFFFFF;
            int L = bytes.Length;
            for (int i = 0; i < L; i++)
            {
                C = (C >> 8) ^ T[(C & 0xFF) ^ bytes[i]];
            }
            return C ^ 0xFFFFFFFF;
        }

        private static int[] signed_crc_table()
        {
            int[] table = new int[256];
            for (int n = 0; n != 256; ++n)
            {
                int c = n;
                c = (int)((c & 1) > 0 ? (-306674912 ^ ((uint)c >> 1)) : ((uint)c >> 1));
                c = (int)((c & 1) > 0 ? (-306674912 ^ ((uint)c >> 1)) : ((uint)c >> 1));
                c = (int)((c & 1) > 0 ? (-306674912 ^ ((uint)c >> 1)) : ((uint)c >> 1));
                c = (int)((c & 1) > 0 ? (-306674912 ^ ((uint)c >> 1)) : ((uint)c >> 1));
                c = (int)((c & 1) > 0 ? (-306674912 ^ ((uint)c >> 1)) : ((uint)c >> 1));
                c = (int)((c & 1) > 0 ? (-306674912 ^ ((uint)c >> 1)) : ((uint)c >> 1));
                c = (int)((c & 1) > 0 ? (-306674912 ^ ((uint)c >> 1)) : ((uint)c >> 1));
                c = (int)((c & 1) > 0 ? (-306674912 ^ ((uint)c >> 1)) : ((uint)c >> 1));
                table[n] = c;
            }
            return table;
        }

        public static int crc32_buf(char[] buf, int seed)
        {
            int[] T = signed_crc_table();
            if (buf.Length > 10000) return crc32_buf_8(buf, seed);
            int C = seed ^ -1;
            int L = buf.Length - 3;
            int i;
            for (i = 0; i < L;)
            {
                C = (int)((uint)C >> 8) ^ T[(C ^ (buf[i] > 47 && buf[i] < 58 ? (buf[i] - 48) : 0)) & 0xFF]; i++;
                C = (int)((uint)C >> 8) ^ T[(C ^ (buf[i] > 47 && buf[i] < 58 ? (buf[i] - 48) : 0)) & 0xFF]; i++;
                C = (int)((uint)C >> 8) ^ T[(C ^ (buf[i] > 47 && buf[i] < 58 ? (buf[i] - 48) : 0)) & 0xFF]; i++;
                C = (int)((uint)C >> 8) ^ T[(C ^ (buf[i] > 47 && buf[i] < 58 ? (buf[i] - 48) : 0)) & 0xFF]; i++;
            }
            while (i < L + 3) { C = (int)((uint)C >> 8) ^ T[C & 0xFF]; i++; }
            return C ^ -1;
        }

        private static int crc32_buf_8(char[] buf, int seed)
        {
            int[] T = signed_crc_table();
            int C = seed ^ -1;
            int L = buf.Length - 7;
            int i;
            for (i = 0; i < L;)
            {
                C = (int)((uint)C >> 8) ^ T[(C ^ (buf[i] > 47 && buf[i] < 58 ? (buf[i] - 48) : 0)) & 0xFF]; i++;
                C = (int)((uint)C >> 8) ^ T[(C ^ (buf[i] > 47 && buf[i] < 58 ? (buf[i] - 48) : 0)) & 0xFF]; i++;
                C = (int)((uint)C >> 8) ^ T[(C ^ (buf[i] > 47 && buf[i] < 58 ? (buf[i] - 48) : 0)) & 0xFF]; i++;
                C = (int)((uint)C >> 8) ^ T[(C ^ (buf[i] > 47 && buf[i] < 58 ? (buf[i] - 48) : 0)) & 0xFF]; i++;
                C = (int)((uint)C >> 8) ^ T[(C ^ (buf[i] > 47 && buf[i] < 58 ? (buf[i] - 48) : 0)) & 0xFF]; i++;
                C = (int)((uint)C >> 8) ^ T[(C ^ (buf[i] > 47 && buf[i] < 58 ? (buf[i] - 48) : 0)) & 0xFF]; i++;
                C = (int)((uint)C >> 8) ^ T[(C ^ (buf[i] > 47 && buf[i] < 58 ? (buf[i] - 48) : 0)) & 0xFF]; i++;
                C = (int)((uint)C >> 8) ^ T[(C ^ (buf[i] > 47 && buf[i] < 58 ? (buf[i] - 48) : 0)) & 0xFF]; i++;
            }
            while (i < L + 7) C = (C >> 8) ^ T[(C ^ buf[i++]) & 0xFF];
            return C ^ -1;
        }
    }
}
