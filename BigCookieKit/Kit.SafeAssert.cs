using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BigCookieKit
{
    public static partial class Kit
    {
        /// <summary>
        /// 判断对象为NULL或者默认值
        /// <para/>class判断NULL
        /// <para/>struct判断是否为默认值
        /// <para/>String判断IsNullOrEmpty
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="def">是否判断默认值</param>
        /// <returns></returns>
        public static Boolean IsNull(this Object obj, Boolean def = true)
        {
            if (obj == null) return true;
            else if (def && obj.GetType() == typeof(string))
            {
                return (string)obj == "";
            }
            else if (def && obj.IsValue())
            {
                if (Equals(obj, Activator.CreateInstance(obj.GetType())))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断对象不为NULL或者默认值
        /// <para/>直接反向 <seealso cref="IsNull"/> 结果
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="def">是否包含默认值</param>
        /// <returns></returns>
        public static Boolean NotNull(this Object obj, Boolean def = true)
        {
            return !obj.IsNull(def);
        }

        /// <summary>
        /// 集合是存在内容
        /// <para/>判断 IEnumerable&lt;T&gt; 是否为NULL
        /// <para/>判断 IEnumerable&lt;T&gt;.Count() 是否为0
        /// </summary>
        /// <param name="collection">集合</param>
        /// <returns></returns>
        public static Boolean Exist<T>(this IEnumerable<T> collection)
        {
            if (collection.IsNull() || collection.Count().IsNull())
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 集合是不存在内容
        /// <para/>直接反向 <seealso cref="Exist&lt;T&gt;"/> 结果
        /// </summary>
        /// <param name="collection">集合</param>
        /// <returns></returns>
        public static Boolean NotExist<T>(this IEnumerable<T> collection)
        {
            return !collection.Exist();
        }

        /// <summary>
        /// 判断DataSet是否为NULL
        /// <para/>判断 DataSet 是否为NULL
        /// <para/>判断 DataSet.Tables 是否为NULL
        /// <para/>判断 DataSet.Tables.Count 是否为0
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static Boolean Exist(this DataSet ds)
        {
            if (ds.IsNull() || ds.Tables.IsNull() || ds.Tables.Count.IsNull())
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 判断DataSet是否不为NULL
        /// <para/>直接反向 <seealso cref="Exist"/> 结果
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static Boolean NotExist(this DataSet ds)
        {
            return !ds.Exist();
        }

        /// <summary>
        /// 判断DataSet是否为NULL 并且索引下的Table存在行数据
        /// <para/>判断 DataSet 是否为NULLs
        /// <para/>判断 DataSet.Tables 是否为NULL
        /// <para/>判断 DataSet.Tables.Count 是否为0
        /// <para/>判断 <seealso cref="NotExist"/> 结果
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="index">Tables的索引</param>
        /// <returns></returns>
        public static Boolean Exist(this DataSet ds, Int32 index)
        {
            if (ds.IsNull() || ds.Tables.IsNull() || ds.Tables.Count.IsNull() || ds.Tables[index].NotExist())
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 判断DataSet是否不为NULL 并且索引下的Table不存在行数据
        /// <para/>直接反向 <seealso cref="Exist"/> 结果
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="index">Tables的索引</param>
        /// <returns></returns>
        public static Boolean NotExist(this DataSet ds, Int32 index)
        {
            return !ds.Exist(index);
        }

        /// <summary>
        /// 判断DataTable存在行数据
        /// <para/>判断 DataTable 是否为NULL
        /// <para/>判断 DataTable.Rows 是否为NULL
        /// <para/>判断 DataTable.Rows.Count 是否为0
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static Boolean Exist(this DataTable dt)
        {
            if (dt.IsNull() || dt.Rows.IsNull() || dt.Rows.Count.IsNull())
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 判断DataTable不存在行数据
        /// <para/>直接反向 <seealso cref="Exist(DataSet,Int32)"/> 结果
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static Boolean NotExist(this DataTable dt)
        {
            return !dt.Exist();
        }

        /// <summary>
        /// 判断字符片段在目标字符串中都存在
        /// </summary>
        /// <param name="str"></param>
        /// <param name="parameters">参数顺序代表{0} {1}</param>
        /// <returns></returns>
        public static Boolean AllOwn(this String str, params String[] parameters)
        {
            foreach (var item in parameters)
            {
                if (str.IndexOf(item, StringComparison.Ordinal) == -1)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 判断是否为引用类型
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static Boolean IsClass(this Object obj)
        {
            if (obj.GetType().IsClass)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断是否为值类型
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static Boolean IsValue(this Object obj)
        {
            Type type = obj.GetType();
            if (type.IsValueType)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断是否为结构体
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static Boolean IsStruct(this Object obj)
        {
            Type type = obj.GetType();
            if (!type.IsPrimitive && !type.IsEnum && type.IsValueType)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否为枚举
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static Boolean IsEnum(this Object obj)
        {
            if (obj.GetType().IsEnum)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否为数组
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Boolean IsArray(this Object obj)
        {
            if (obj.GetType().IsArray)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 转成字符串(默认为NULL)
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static String ToStr(this Object obj)
        {
            if (obj.NotNull())
            {
                return obj.ToString();
            }
            return null;
        }

        /// <summary>
        /// 转成字符串(默认为空字符串)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static String ToStrEmpty(this Object obj)
        {
            if (obj.NotNull())
            {
                return obj.ToString();
            }
            return "";
        }

        public static bool Exist(this Enum source, params Enum[] range)
        {
            if (range.Length == 0) return false;
            else if (range.Length == 1)
            {
                return source == range[0];
            }
            int r = Convert.ToInt32(range[0]);
            for (int i = 1; i < range.Length; i++)
            {
                r |= Convert.ToInt32(range[i]);
            }
            return !((Convert.ToInt32(source) & r) == 0);//也可写作return (Convert.ToInt32(source) & r) == Convert.ToInt32(source);
        }
    }
}
