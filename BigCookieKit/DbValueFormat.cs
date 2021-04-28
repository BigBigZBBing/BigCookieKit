using System;
using System.Collections.Generic;
using System.Text;

namespace BigCookieKit
{
    /// <summary>
    /// DataTable取值格式化
    /// </summary>
    [Flags]
    public enum DbValueFormat
    {
        None = 0,
        /// <summary>
        /// 去除头尾空格
        /// </summary>
        DisTrim = 1,
        /// <summary>
        /// 去除换行符
        /// </summary>
        DisBreak = 1 << 1,
        /// <summary>
        /// 去除制表符
        /// </summary>
        DisTabs = 1 << 2,
    }
}
