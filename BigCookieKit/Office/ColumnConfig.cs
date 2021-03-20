using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Office
{
    /// <summary>
    /// 列的默认类型
    /// </summary>
    public enum ColumnNormal
    {
        Guid,
        NowDate,
        DBNull
    }

    public class ColumnConfig
    {
        /// <summary>
        /// 列的序号
        /// </summary>
        public string Column
        {
            get
            {
                return _Column;
            }
            set
            {
                StartColumnIndex = ExcelHelper.ColumnToIndex(value);
                _Column = value;
            }
        }

        /// <summary>
        /// 列的索引 由1开始
        /// </summary>
        public int? StartColumnIndex { get; set; }

        /// <summary>
        /// 列的类型
        /// </summary>
        public Type ColumnType { get; set; }

        /// <summary>
        /// 列的名称
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public ColumnNormal Normal { get; set; }

        string _Column;
    }
}
