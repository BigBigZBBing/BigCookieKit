using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit
{
    public class ExcelConfig
    {
        /// <summary>
        /// 列名索引行 不设置则自动列名 例:Colume1 Colume2
        /// </summary>
        public int? ColumnNameRow { get; set; }

        /// <summary>
        /// 开始的行 跟Excal左边对应
        /// </summary>
        public int? StartRow { get; set; } = 1;

        /// <summary>
        /// 结束的行
        /// </summary>
        public int? EndRow { get; set; }

        /// <summary>
        /// 开始的列 跟Excal上边对应
        /// </summary>
        public string StartColumn
        {
            get
            {
                return _StartColumn;
            }
            set
            {
                StartColumnIndex = ColumnToIndex(value);
                _StartColumn = value;
            }
        }

        /// <summary>
        /// 开始的列索引
        /// </summary>
        public int? StartColumnIndex { get; set; }

        /// <summary>
        /// 结束的列
        /// </summary>
        public string EndColumn
        {
            get
            {
                return _EndColumn;
            }
            set
            {
                EndColumnIndex = ColumnToIndex(value).GetValueOrDefault();
                _EndColumn = value;
            }
        }

        /// <summary>
        /// 结束的列索引
        /// </summary>
        public int? EndColumnIndex { get; set; }

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
            if (index <= 0) return null;
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

        public List<ColumnConfig> Columns { get; set; }

        string _StartColumn;
        string _EndColumn;
    }
}
