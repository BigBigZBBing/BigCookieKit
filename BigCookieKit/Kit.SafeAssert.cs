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
        /// <code>
        /// 默认值判断:
        /// class判断NULL
        /// struct判断是否为默认值
        /// string判断IsNullOrEmpty
        /// </code>
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="def">是否判断默认值</param>
        /// <returns></returns>
        //public static bool IsNull(this Object obj, bool def = true)
        //{
        //    if (obj == null) return true;
        //    Type type = obj.GetType();
        //    if (def && type == typeof(string))
        //    {
        //        return (string)obj == "";
        //    }
        //    else if (def && obj.IsValue())
        //    {
        //        if (Equals(obj, Activator.CreateInstance(type)))
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        /// <summary>
        /// 判断对象不为NULL或者默认值
        /// <para/>直接反向 <seealso cref="IsNull"/> 结果
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="def">是否包含默认值</param>
        /// <returns></returns>
        //public static bool NotNull(this Object obj, bool def = true)
        //{
        //    return !obj.IsNull(def);
        //}

        /// <summary>
        /// 集合是存在内容
        /// <para/>判断 IEnumerable&lt;T&gt; 是否为NULL
        /// <para/>判断 IEnumerable&lt;T&gt;.Count() 是否为0
        /// </summary>
        /// <param name="collection">集合</param>
        /// <returns></returns>
        public static bool Exist<T>(this IEnumerable<T> collection)
        {
            if (collection == null || collection.Count() == 0)
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
        public static bool NotExist<T>(this IEnumerable<T> collection)
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
        public static bool Exist(this DataSet ds)
        {
            if (ds == null || ds.Tables == null || ds.Tables.Count == 0)
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
        public static bool NotExist(this DataSet ds)
        {
            return !ds.Exist();
        }

        /// <summary>
        /// 判断DataSet是否为NULL 并且索引下的Table存在行数据
        /// <para/>判断 DataSet 是否为NULLs
        /// <para/>判断 DataSet.Tables 是否为NULL
        /// <para/>判断 DataSet.Tables.Count 是否为0
        /// <para/>判断 DataSet.Tables[<paramref name="name"/>] 是否存在
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="index">Tables的索引</param>
        /// <returns></returns>
        public static bool ExistName(this DataSet ds, string name = null)
        {
            if (ds == null || ds.Tables == null || ds.Tables.Count == 0 || (name != null && !ds.Tables.Contains(name)))
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
        public static bool NotExistName(this DataSet ds, string name = null)
        {
            return !ds.ExistName(name);
        }

        /// <summary>
        /// 判断DataTable存在行数据
        /// <para/>判断 DataTable 是否为NULL
        /// <para/>判断 DataTable.Rows 是否为NULL
        /// <para/>判断 DataTable.Rows.Count 是否为0
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool Exist(this DataTable dt)
        {
            if (dt == null || dt.Rows == null || dt.Rows.Count == 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 判断DataTable不存在行数据
        /// <para/>直接反向 <seealso cref="Exist(DataSet,int)"/> 结果
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool NotExist(this DataTable dt)
        {
            return !dt.Exist();
        }

        /// <summary>
        /// 判断字符片段在目标字符串中都存在
        /// </summary>
        /// <param name="str"></param>
        /// <param name="parameters">参数顺序代表{0} {1}</param>
        /// <returns></returns>
        public static bool AllOwn(this string str, params string[] parameters)
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
        public static bool IsClass(this Object obj)
        {
            if (obj.GetType().IsClass)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断是否为自定义引用类型
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static bool IsCustomClass(this Object obj)
        {
            Type type = obj.GetType();
            // 不是原始类型 && 并且是引用类型 && 并且不是数组 && 并且不是通用类型 && 不是字符串
            if (!type.IsPrimitive && type.IsClass && !type.IsArray && !type.IsGenericType && type != typeof(string))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断是否为自定义引用类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsCustomClass(this Type type)
        {
            // 不是原始类型 && 并且是引用类型 && 并且不是数组 && 并且不是通用类型 && 不是字符串
            if (!type.IsPrimitive && type.IsClass && !type.IsArray && !type.IsGenericType && type != typeof(string))
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
        public static bool IsValue(this Object obj)
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
        public static bool IsStruct(this Object obj)
        {
            Type type = obj.GetType();
            if (!type.IsEnum && type.IsValueType)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断是否为结构体
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsStruct(this Type type)
        {
            if (!type.IsEnum && type.IsValueType)
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
        public static bool IsEnum(this Object obj)
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
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static bool IsArray(this Object obj)
        {
            if (obj.GetType().IsArray)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否为可空类型
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static bool IsNullable(this Object obj)
        {
            Type type = obj.GetType();
            return (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
        }

        /// <summary>
        /// 是否为可空类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsNullable(this Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
        }
    }
}
