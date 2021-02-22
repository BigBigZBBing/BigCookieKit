﻿using BigCookieKit.Attributes;
using BigCookieKit.Reflect;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace BigCookieKit
{
    public static partial class Kit
    {
        /// <summary>
        /// 获取枚举的Remark值
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="em">枚举对象</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static String Remark<TEnum>(this TEnum em) where TEnum : Enum
        {
            Type type = em.GetType();
            FieldInfo field = type.GetField(em.ToString());
            if (field != null)
            {
                var attrs = field.GetCustomAttributes(typeof(RemarkAttribute), true) as RemarkAttribute[];
                if (attrs != null && attrs.Length > 0)
                    return attrs[0].Remark;
            }
            return "";
        }

        /// <summary>
        /// <para>验证模型</para>
        /// <para>(未设置错误消息不验证)</para>
        /// <para>{0}属性名称</para>
        /// <para>{1}属性值</para>
        /// <para>{2}错误描述</para>
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="Entity">实体对象</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean ModelValidation<TEntity>(this TEntity Entity)
        {
            StringBuilder strBuilder = new StringBuilder();
            try
            {
                PropertyInfo[] properties = Entity.GetType().GetProperties();
                if (properties != null)
                {
                    foreach (PropertyInfo propertie in properties)
                    {
                        if (propertie.PropertyType == typeof(string)
                            && propertie.GetCustomAttribute(typeof(StringRuleAttribute)).NotNull())
                        {
                            var attr = propertie.GetCustomAttribute(typeof(StringRuleAttribute)) as StringRuleAttribute;
                            if (!BasicValidation(new FastProperty(propertie, Entity), attr, strBuilder))
                                continue;
                        }

                        if ((propertie.PropertyType == typeof(int) || propertie.PropertyType == typeof(long))
                            && propertie.GetCustomAttribute(typeof(NumericRuleAttribute)).NotNull())
                        {
                            var attr = propertie.GetCustomAttribute(typeof(NumericRuleAttribute)) as NumericRuleAttribute;
                            if (!BasicValidation(new FastProperty(propertie, Entity), attr, strBuilder))
                                continue;
                        }

                        if ((propertie.PropertyType == typeof(float) || propertie.PropertyType == typeof(double) || propertie.PropertyType == typeof(decimal))
                            && propertie.GetCustomAttribute(typeof(DecimalRuleAttribute)).NotNull())
                        {
                            var attr = propertie.GetCustomAttribute(typeof(DecimalRuleAttribute)) as DecimalRuleAttribute;
                            if (!BasicValidation(new FastProperty(propertie, Entity), attr, strBuilder))
                                continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            if (strBuilder.Length > 0)
                throw new Exception(strBuilder.ToString());
            return true;
        }

        #region 私有

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool BasicValidation<T>(FastProperty propertie, T attr, StringBuilder strBuilder) where T : BasicAttribute
        {
            if (attr.NotNull() && attr.Message.NotNull())
            {
                Type Type = propertie.PropertyType;
                string Name = propertie.PropertyName;
                object Value = propertie.Get();
                if (attr.Required.NotNull() && attr.Required.Value == true && Value.IsNull())
                {
                    strBuilder.AppendLine(string.Format(attr.Message, Name, Value ?? "NULL", nameof(attr.Required)));
                    return false;
                }
                if (attr is StringRuleAttribute)
                {
                    if (!StringValidation(propertie, attr as StringRuleAttribute, strBuilder))
                        return false;
                }
                if (attr is NumericRuleAttribute)
                {
                    if (!NumericValidation(propertie, attr as NumericRuleAttribute, strBuilder))
                        return false;
                }
                if (attr is DecimalRuleAttribute)
                {
                    if (!DecimalValidation(propertie, attr as DecimalRuleAttribute, strBuilder))
                        return false;
                }
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool StringValidation(FastProperty propertie, StringRuleAttribute attr, StringBuilder strBuilder)
        {
            string Name = propertie.PropertyName;
            object Value = propertie.Get();
            if (attr.MinLength.NotNull() && Value.NotNull() && Value.ToString().Length < attr.MinLength)
            {
                strBuilder.AppendLine(string.Format(attr.Message, Name, Value ?? "NULL", nameof(attr.MinLength)));
                return false;
            }
            if (attr.MaxLength.NotNull() && Value.NotNull() && Value.ToString().Length > attr.MaxLength)
            {
                strBuilder.AppendLine(string.Format(attr.Message, Name, Value ?? "NULL", nameof(attr.MaxLength)));
                return false;
            }
            if (attr.RegExp.NotNull() && Value.NotNull() && !Regex.IsMatch(Value.ToString(), attr.RegExp))
            {
                strBuilder.AppendLine(string.Format(attr.Message, Name, Value ?? "NULL", nameof(attr.RegExp)));
                return false;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool NumericValidation(FastProperty propertie, NumericRuleAttribute attr, StringBuilder strBuilder)
        {
            string Name = propertie.PropertyName;
            object Value = propertie.Get();
            if (attr.Greater.NotNull() && Value.NotNull() && Convert.ToInt64(Value) > attr.Greater)
            {
                strBuilder.AppendLine(string.Format(attr.Message, Name, Value ?? "NULL", nameof(attr.Greater)));
                return false;
            }
            if (attr.Less.NotNull() && Value.NotNull() && Convert.ToInt64(Value) < attr.Less)
            {
                strBuilder.AppendLine(string.Format(attr.Message, Name, Value ?? "NULL", nameof(attr.Less)));
                return false;
            }
            if (attr.Equal.NotNull() && Value.NotNull() && Convert.ToInt64(Value) == attr.Equal)
            {
                strBuilder.AppendLine(string.Format(attr.Message, Name, Value ?? "NULL", nameof(attr.Equal)));
                return false;
            }
            if (attr.NoEqual.NotNull() && Value.NotNull() && Convert.ToInt64(Value) != attr.NoEqual)
            {
                strBuilder.AppendLine(string.Format(attr.Message, Name, Value ?? "NULL", nameof(attr.NoEqual)));
                return false;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool DecimalValidation(FastProperty propertie, DecimalRuleAttribute attr, StringBuilder strBuilder)
        {
            string Name = propertie.PropertyName;
            object Value = propertie.Get();
            if (attr.Greater.NotNull() && Value.NotNull() && Convert.ToDecimal(Value) > attr.Greater)
            {
                strBuilder.AppendLine(string.Format(attr.Message, Name, Value ?? "NULL", nameof(attr.Greater)));
                return false;
            }
            if (attr.Less.NotNull() && Value.NotNull() && Convert.ToDecimal(Value) < attr.Less)
            {
                strBuilder.AppendLine(string.Format(attr.Message, Name, Value ?? "NULL", nameof(attr.Less)));
                return false;
            }
            if (attr.Equal.NotNull() && Value.NotNull() && Convert.ToDecimal(Value) == attr.Equal)
            {
                strBuilder.AppendLine(string.Format(attr.Message, Name, Value ?? "NULL", nameof(attr.Equal)));
                return false;
            }
            if (attr.NoEqual.NotNull() && Value.NotNull() && Convert.ToDecimal(Value) != attr.NoEqual)
            {
                strBuilder.AppendLine(string.Format(attr.Message, Name, Value ?? "NULL", nameof(attr.NoEqual)));
                return false;
            }
            if (attr.Precision.NotNull() && Value.NotNull() && Convert.ToDecimal(Value).GetPrecision() > attr.Precision)
            {
                strBuilder.AppendLine(string.Format(attr.Message, Name, Value ?? "NULL", nameof(attr.Precision)));
                return false;
            }
            return true;
        }

        #endregion
    }
}
