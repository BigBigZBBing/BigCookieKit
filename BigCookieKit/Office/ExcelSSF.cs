using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Office
{
    public static class ExcelSSF
    {
        /// <summary>
        /// 固定的单元格样式
        /// </summary>
        internal static Dictionary<string, Type> FixedNumFmt => new Dictionary<string, Type>()
        {
            { "0" , typeof(string) },
            { "14" , typeof(DateTime) }
        };
    }
}
