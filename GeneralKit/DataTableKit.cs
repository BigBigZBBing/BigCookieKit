using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;

namespace GeneralKit
{
    /// <summary>
    /// DataTable工具箱
    /// </summary>
    public static class DataTableKit
    {
        /// <summary>
        /// 给DataTable单元格赋值
        /// <para/>解决数值类型空值问题
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="filed">列名</param>
        /// <param name="value">值</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CellSetValue(this DataRow dr, string filed, object value)
        {
            Type filedType = dr.Table.Columns[filed].DataType;
            if (value == DBNull.Value || value.IsNull())
            {
                dr[filed] = DBNull.Value;
            }
            else
            {
                if (value.GetType() != filedType
                    && value.TryParse(filedType, out object temp))
                {
                    dr[filed] = temp;
                }
                else
                {
                    dr[filed] = value;
                }
            }
        }

        /// <summary>
        /// 安全获取单元格值
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="filed"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CellGetValue<T>(this DataRow dr, string filed, DbValueFormat format = DbValueFormat.None)
        {
            Type filedType = dr.Table.Columns[filed].DataType;
            if (typeof(T) != filedType || dr[filed] == DBNull.Value || dr[filed].IsNull(false))
            {
                return (T)Activator.CreateInstance(filedType);
            }
            else if (filedType == typeof(String))
            {
                string temp = dr[filed].SafeParse<String>();
                switch (format)
                {
                    case DbValueFormat.DisTrim:
                        return (T)(object)temp.Trim();
                    case DbValueFormat.DisBreak:
                        return (T)(object)temp.Replace("\n", "").Replace("\r", "");
                    case DbValueFormat.DisTabs:
                        return (T)(object)temp.Replace("\t", "");
                }
            }
            return (T)dr[filed];
        }
    }
}
