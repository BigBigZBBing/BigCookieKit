using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BigCookieKit.XML
{
    /// <summary>
    /// 重构DataTable适应于BigCookie的框架数据调度
    /// </summary>
    public class CookieTable
    {
        public List<CookieColumn> Columns { get; set; }

        public List<CookieRow> Rows { get; set; }

        public CookieTable()
        {
            Columns = new List<CookieColumn>();
            Rows = new List<CookieRow>();
        }

        public void AddColumn(string ColumnName, Type ColumnType, string Formula = null)
        {
            Columns.Add(new CookieColumn()
            {
                ColumnName = ColumnName,
                ColumnType = ColumnType,
                Formula = Formula,
            });
        }

        public void AddRow(params object[] value)
        {
            if (value.Length > Columns.Count)
                throw new IndexOutOfRangeException();

            CookieRow ncr = new CookieRow();
            ncr.Cells = new CookieCell[Columns.Count];
            for (int i = 0; i < value.Length; i++)
            {
                ncr.Cells[i].Column = Columns[i];
                ncr.Cells[i].Value = value[i];
            }
            Rows.Add(ncr);
        }
    }

    public class CookieColumn
    {
        public string Formula { get; set; }

        public Type ColumnType { get; set; }

        public string ColumnName { get; set; }
    }

    public class CookieRow
    {
        public CookieCell[] Cells { get; set; }
    }

    public struct CookieCell
    {
        public CookieColumn Column { get; set; }

        public string Formula { get; set; }

        public object Value { get; set; }
    }
}
