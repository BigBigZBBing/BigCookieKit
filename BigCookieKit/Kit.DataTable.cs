using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;

namespace BigCookieKit
{
    /// <summary>
    /// DataTable工具箱
    /// </summary>
    public static class DataTableKit
    {
        /// <summary>
        /// 给单元格赋值
        /// <para/>解决数值类型空值问题
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="filed">列名</param>
        /// <param name="value">值</param>
        public static void CellSetValue(this DataRow dr, string filed, object value)
        {
            Type filedType = dr.Table.Columns[filed].DataType;
            if (value == DBNull.Value || value.IsNull(false))
            {
                dr[filed] = DBNull.Value;
            }
            else
            {
                if (value.GetType() != filedType)
                {
                    if (value.TryParse(filedType, out object temp))
                    {
                        dr[filed] = temp;
                    }
                    else
                    {
                        dr[filed] = DBNull.Value;
                    }
                }
                else
                {
                    dr[filed] = value;
                }
            }
        }

        /// <summary>
        /// 获取单元格值
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static T CellGetValue<T>(this DataRow dr, string field, DbValueFormat format = DbValueFormat.None)
        {
            Type filedType = dr.Table.Columns[field].DataType;
            if (dr[field] == DBNull.Value
                || dr[field].IsNull(false)
                || !dr[field].TryParse<T>(out var item))
            {
                return default(T);
            }
            else
            {
                if (typeof(T) == typeof(String))
                {
                    if (format.HasFlag(DbValueFormat.DisTrim))
                    {
                        item = ((string)item).Trim();
                    }
                    if (format.HasFlag(DbValueFormat.DisBreak))
                    {
                        item = ((string)item).Replace("\n", "").Replace("\r", "");
                    }
                    if (format.HasFlag(DbValueFormat.DisTabs))
                    {
                        item = ((string)item).Replace("\t", "");
                    }
                }
                return (T)item;
            }
        }
    }
}
