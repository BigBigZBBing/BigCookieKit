using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace BigCookieKit
{
    public static partial class Kit
    {
        /// <summary>
        /// 尝试转换类型
        /// </summary>
        /// <param name="obj">转换的对象</param>
        /// <param name="type">需要转换的类型</param>
        /// <param name="value">回调的参数</param>
        /// <returns></returns>
        
        public static bool TryParse(this object obj, Type type, out object value)
        {
            value = null;
            try
            {
                string temp = obj.ToString();
                MethodInfo methodInfo = type.GetMethod("TryParse", new[] { typeof(string), type.MakeByRefType() });
                var parameters = new object[] { temp, null };
                if (methodInfo.NotNull() && (bool)methodInfo.Invoke(type, parameters))
                {
                    value = parameters[1];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch { }
            return false;
        }

        /// <summary>
        /// 尝试转换类型
        /// </summary>
        /// <param name="obj">转换的对象</param>
        /// <param name="type">需要转换的类型</param>
        /// <returns></returns>
        
        public static bool TryParse(this object obj, Type type)
        {
            return obj.TryParse(type, out object temp);
        }

        /// <summary>
        /// 尝试转换类型
        /// </summary>
        /// <typeparam name="T">需要转换的类型</typeparam>
        /// <param name="obj">转换的对象</param>
        /// <param name="value">回调的参数</param>
        /// <returns></returns>
        
        public static bool TryParse<T>(this object obj, out object value)
        {
            Type type = typeof(T);
            return obj.TryParse(type, out value);
        }

        /// <summary>
        /// 尝试转换类型
        /// </summary>
        /// <typeparam name="T">需要转换的类型</typeparam>
        /// <param name="obj">转换的对象</param>
        /// <returns></returns>
        
        public static bool TryParse<T>(this object obj)
        {
            return obj.TryParse<T>(out object temp);
        }

        /// <summary>
        /// 安全转换对象
        /// <para/>如果是NULL会返回默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        
        public static T SafeParse<T>(this Object obj)
        {
            if (obj.TryParse<T>(out var value))
            {
                return (T)value;
            }
            return default(T);
        }
    }
}
