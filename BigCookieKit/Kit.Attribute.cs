using BigCookieKit.Attributes;
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

        public static string ToDisplay<TEnum>(this TEnum em) where TEnum : Enum
        {
            Type type = em.GetType();
            FieldInfo field = type.GetField(em.ToString());
            if (field != null)
            {
                var attrs = field.GetCustomAttributes(typeof(DisplayAttribute), true) as DisplayAttribute[];
                if (attrs != null && attrs.Length > 0)
                    return attrs[0].Value;
            }
            return "";
        }

        /// <summary>
        /// <para>验证模型</para>
        /// <para>(未设置错误消息不验证)</para>
        /// <para>{0}属性名称</para>
        /// <para>{1}特性备注</para>
        /// <para>{2}属性值</para>
        /// <para>{3}错误描述</para>
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="Entity">实体对象</param>
        /// <returns></returns>

        public static bool ModelValidation<TEntity>(this TEntity Entity)
        {
            StringBuilder strBuilder = new StringBuilder();
            try
            {
                PropertyInfo[] properties = Entity.GetType().GetProperties();
                if (properties != null)
                {
                    foreach (PropertyInfo propertie in properties)
                    {
                        if (propertie.GetCustomAttribute(typeof(RequiredRuleAttribute)).NotNull())
                        {
                            var attr = propertie.GetCustomAttribute(typeof(RequiredRuleAttribute)) as RequiredRuleAttribute;
                            if (!BasicValidation(new FastProperty(propertie, Entity), attr, strBuilder))
                                continue;
                        }

                        if (propertie.PropertyType == typeof(string)
                            && propertie.GetCustomAttribute(typeof(StringRuleAttribute)).NotNull())
                        {
                            var attr = propertie.GetCustomAttribute(typeof(StringRuleAttribute)) as StringRuleAttribute;
                            if (!BasicValidation(new FastProperty(propertie, Entity), attr, strBuilder))
                                continue;
                        }

                        if ((propertie.PropertyType == typeof(int) || propertie.PropertyType == typeof(long)
                            || propertie.PropertyType == typeof(int?) || propertie.PropertyType == typeof(long?))
                            && propertie.GetCustomAttribute(typeof(NumericRuleAttribute)).NotNull())
                        {
                            var attr = propertie.GetCustomAttribute(typeof(NumericRuleAttribute)) as NumericRuleAttribute;
                            if (!BasicValidation(new FastProperty(propertie, Entity), attr, strBuilder))
                                continue;
                        }

                        if ((propertie.PropertyType == typeof(float) || propertie.PropertyType == typeof(double) || propertie.PropertyType == typeof(decimal)
                            || propertie.PropertyType == typeof(float?) || propertie.PropertyType == typeof(double?) || propertie.PropertyType == typeof(decimal?))
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


        static bool BasicValidation<T>(FastProperty propertie, T attr, StringBuilder strBuilder) where T : BasicAttribute
        {
            if (attr.NotNull() && attr.Message.NotNull())
            {
                Type Type = propertie.PropertyType;
                string Name = propertie.PropertyName;
                object Value = propertie.Get();
                if (attr.Required == true && Value.IsNull())
                {
                    strBuilder.AppendLine(string.Format(attr.Message, Name, attr.Name, Value ?? "NULL", "必须赋值"));
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


        static bool StringValidation(FastProperty propertie, StringRuleAttribute attr, StringBuilder strBuilder)
        {
            string Name = propertie.PropertyName;
            object Value = propertie.Get();
            if (attr.MinLength.NotNull() && Value.NotNull() && Value.ToString().Length < (int?)attr.MinLength)
            {
                strBuilder.AppendLine(string.Format(attr.Message, Name, attr.Name, Value ?? "NULL", $"不能少于{attr.MinLength}位"));
                return false;
            }
            if (attr.MaxLength.NotNull() && Value.NotNull() && Value.ToString().Length > (int?)attr.MaxLength)
            {
                strBuilder.AppendLine(string.Format(attr.Message, Name, attr.Name, Value ?? "NULL", $"不能超过{attr.MaxLength}位"));
                return false;
            }
            if (attr.RegExp.NotNull() && Value.NotNull() && !Regex.IsMatch(Value.ToString(), attr.RegExp))
            {
                strBuilder.AppendLine(string.Format(attr.Message, Name, attr.Name, Value ?? "NULL", $"正则匹配失败"));
                return false;
            }
            return true;
        }


        static bool NumericValidation(FastProperty propertie, NumericRuleAttribute attr, StringBuilder strBuilder)
        {
            string Name = propertie.PropertyName;
            object Value = propertie.Get();
            if (attr.Greater.NotNull(false) && Value.NotNull(false) && Convert.ToDecimal(Value) > Convert.ToDecimal(attr.Greater))
            {
                strBuilder.AppendLine(string.Format(attr.Message, Name, attr.Name, Value ?? "NULL", $"不能大于{attr.Greater}"));
                return false;
            }
            if (attr.Less.NotNull(false) && Value.NotNull(false) && Convert.ToDecimal(Value) < Convert.ToDecimal(attr.Less))
            {
                strBuilder.AppendLine(string.Format(attr.Message, Name, attr.Name, Value ?? "NULL", $"不能小于{attr.Less}"));
                return false;
            }
            if (attr.Equal.NotNull(false) && Value.NotNull(false) && Convert.ToDecimal(Value) != Convert.ToDecimal(attr.Equal))
            {
                strBuilder.AppendLine(string.Format(attr.Message, Name, attr.Name, Value ?? "NULL", $"必须等于{attr.Equal}"));
                return false;
            }
            if (attr.NoEqual.NotNull(false) && Value.NotNull(false) && Convert.ToDecimal(Value) == Convert.ToDecimal(attr.NoEqual))
            {
                strBuilder.AppendLine(string.Format(attr.Message, Name, attr.Name, Value ?? "NULL", $"不能等于{attr.NoEqual}"));
                return false;
            }
            return true;
        }


        static bool DecimalValidation(FastProperty propertie, DecimalRuleAttribute attr, StringBuilder strBuilder)
        {
            string Name = propertie.PropertyName;
            object Value = propertie.Get();
            if (attr.Greater.NotNull(false) && Value.NotNull(false) && Convert.ToDecimal(Value) > Convert.ToDecimal(attr.Greater))
            {
                strBuilder.AppendLine(string.Format(attr.Message, Name, attr.Name, Value ?? "NULL", $"不能大于{attr.Greater}"));
                return false;
            }
            if (attr.Less.NotNull(false) && Value.NotNull(false) && Convert.ToDecimal(Value) < Convert.ToDecimal(attr.Less))
            {
                strBuilder.AppendLine(string.Format(attr.Message, Name, attr.Name, Value ?? "NULL", $"不能小于{attr.Less}"));
                return false;
            }
            if (attr.Equal.NotNull(false) && Value.NotNull(false) && Convert.ToDecimal(Value) == Convert.ToDecimal(attr.Equal))
            {
                strBuilder.AppendLine(string.Format(attr.Message, Name, attr.Name, Value ?? "NULL", $"不能等于{attr.Equal}"));
                return false;
            }
            if (attr.NoEqual.NotNull(false) && Value.NotNull(false) && Convert.ToDecimal(Value) != Convert.ToDecimal(attr.NoEqual))
            {
                strBuilder.AppendLine(string.Format(attr.Message, Name, attr.Name, Value ?? "NULL", $"必须等于{attr.NoEqual}"));
                return false;
            }
            if (attr.Precision.NotNull(false) && Value.NotNull(false) && Convert.ToDecimal(Value).GetPrecision() > (int?)attr.Precision)
            {
                strBuilder.AppendLine(string.Format(attr.Message, Name, attr.Name, Value ?? "NULL", $"精度不能超过{attr.Precision}"));
                return false;
            }
            return true;
        }

        #endregion
    }
}
