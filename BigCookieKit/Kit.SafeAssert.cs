using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

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
        /// <param name="def">是否包含默认值</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean IsNull(this Object obj, Boolean def = true)
        {
            if (obj == null)
                return true;
            else if (def && obj.GetType() == typeof(String))
            {
                return obj?.ToString() == "";
            }
            else if (obj.IsValue() && def && obj == Activator.CreateInstance(obj.GetType()))
            {
                return true;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean NotExist(this DataTable dt)
        {
            return !dt.Exist();
        }

        /// <summary>
        /// 保证字符串都拥有
        /// </summary>
        /// <param name="str"></param>
        /// <param name="parameters">参数顺序代表{0} {1}</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean IsClass(this Object obj)
        {
            if (obj.GetType().BaseType == typeof(Object))
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean IsValue(this Object obj)
        {
            if ((obj as ValueType) != null)
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean IsStruct(this Object obj)
        {
            if (obj.GetType().BaseType == typeof(ValueType))
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean IsEnum(this Object obj)
        {
            if (obj.GetType().BaseType == typeof(Enum))
            {
                return true;
            }
            return false;
        }
    }
}
