using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Office.Xlsx
{
    public class ColumnConfig
    {
        private string _Column;

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
                ColumnIndex = ExcelHelper.ColumnToIndex(value);
                _Column = value;
            }
        }

        /// <summary>
        /// 列的索引 由1开始
        /// </summary>
        public int? ColumnIndex { get; set; }

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
        public ColumnNormal NormalType { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// 是否可空
        /// </summary>
        public bool IsAllowNull { get; set; } = true;

    }
}
