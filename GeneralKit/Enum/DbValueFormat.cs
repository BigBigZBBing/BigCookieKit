using System;
using System.Collections.Generic;
using System.Text;

namespace GeneralKit
{
    /// <summary>
    /// DataTable取值格式化
    /// </summary>
    public enum DbValueFormat
    {
        None,
        /// <summary>
        /// 去除头尾空格
        /// </summary>
        DisTrim,
        /// <summary>
        /// 去除换行符
        /// </summary>
        DisBreak,
        /// <summary>
        /// 去除制表符
        /// </summary>
        DisTabs,
    }
}
