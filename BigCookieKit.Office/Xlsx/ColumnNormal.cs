using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Office.Xlsx
{
    /// <summary>
    /// 列的默认类型
    /// </summary>
    public enum ColumnNormal
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default,
        /// <summary>
        /// Guid
        /// </summary>
        Guid,
        /// <summary>
        /// 当前时间
        /// </summary>
        NowDate,
        /// <summary>
        /// 自增
        /// </summary>
        Increment,
    }
}
