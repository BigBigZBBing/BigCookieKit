using System;
using System.Collections.Generic;
using System.Data;
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
        /// <param name="filed"></param>
        /// <param name="value"></param>
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
    }
}
