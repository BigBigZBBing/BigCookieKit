using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace BigCookieKit
{
    public static partial class Kit
    {
        /// <summary>
        /// 获取浮点数的精度
        /// </summary>
        /// <param name="m_float"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetPrecision(this float m_float)
        {
            return GetPrecision((decimal)m_float);
        }

        /// <summary>
        /// 获取浮点数的精度
        /// </summary>
        /// <param name="m_float"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetPrecision(this double m_float)
        {
            return GetPrecision((decimal)m_float);
        }

        /// <summary>
        /// 获取浮点数的精度
        /// </summary>
        /// <param name="m_float"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetPrecision(this decimal m_float)
        {
            var str = m_float.ToString();
            var index = str.IndexOf(".", StringComparison.OrdinalIgnoreCase);
            if (index > -1)
            {
                return str.Substring(index + 1).TrimEnd('0').Length;
            }
            else
                return 0;
        }
    }
}
