using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Office
{
    public class ExcelConfig
    {
        /// <summary>
        /// 工作簿索引
        /// </summary>
        public int SheetIndex { get; set; }

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
                StartColumnIndex = ExcelHelper.ColumnToIndex(value);
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
                EndColumnIndex = ExcelHelper.ColumnToIndex(value).GetValueOrDefault();
                _EndColumn = value;
            }
        }

        /// <summary>
        /// 结束的列索引
        /// </summary>
        public int? EndColumnIndex { get; set; }

        /// <summary>
        /// 列的配置
        /// </summary>
        public ColumnConfig[] ColumnSetting { get; set; }

        string _StartColumn;
        string _EndColumn;
    }
}
