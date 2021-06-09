using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Office
{
    public static class ExcelHelper
    {
        /// <summary>
        /// 获取Excal 列坐标的对应的索引
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public static int? ColumnToIndex(string column)
        {
            if (string.IsNullOrEmpty(column)) return null;
            int index = 0, pos = 0;
            for (int i = column.Length - 1; i >= 0; i--, pos++)
            {
                index += ((column[i] - 64) * (int)(Math.Pow(26, pos)));
            }
            return index - 1;
        }

        /// <summary>
        /// 根据索引获取Excal 列坐标
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string IndexToColumn(int index)
        {
            if (index < 0) return null;
            string column = "";
            do
            {
                if (column.Length > 0)
                    index--;
                column = ((char)(index % 26 + (int)'A')).ToString() + column;
                index = (int)((index - index % 26) / 26);
            } while (index > 0);
            return column;
        }
    }
}
