using GeneralKit.Attributes;
using ILWheatBread;
using ILWheatBread.SmartEmit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace GeneralKit
{
    /// <summary>
    /// 通用工具箱
    /// </summary>
    public static class Kit
    {
        /// <summary>
        /// 判断对象为NULL或者默认值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static Boolean IsNull(this object obj)
        {
            if (obj == null)
            {
                return true;
            }
            else if (obj.GetType() == typeof(string))
            {
                return string.IsNullOrEmpty(obj.ToString());
            }
            else if (obj.IsValue() && obj == Activator.CreateInstance(obj.GetType()))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断对象不为NULL或者默认值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static Boolean NotNull(this object obj)
        {
            return !obj.IsNull();
        }

        /// <summary>
        /// 集合是存在内容
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
        /// </summary>
        /// <param name="collection">集合</param>
        /// <returns></returns>
        public static Boolean NotExist<T>(this IEnumerable<T> collection)
        {
            return !collection.Exist();
        }

        /// <summary>
        /// 获取枚举的Remark值
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="em">枚举对象</param>
        /// <returns></returns>
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

        /// <summary>
        /// 金钱转大写
        /// <para>(保留两位小数)</para>
        /// </summary>
        /// <param name="Num">数字</param>
        /// <returns></returns>
        public static String MoneyUpper(this decimal num)
        {
            string str1 = "零壹贰叁肆伍陆柒捌玖";            //0-9所对应的汉字 
            string str2 = "万仟佰拾亿仟佰拾万仟佰拾元角分"; //数字位所对应的汉字 
            string str3 = "";    //从原num值中取出的值 
            string str4 = "";    //数字的字符串形式 
            string str5 = "";  //人民币大写金额形式 
            int i;    //循环变量 
            int j;    //num的值乘以100的字符串长度 
            string ch1 = "";    //数字的汉语读法 
            string ch2 = "";    //数字位的汉字读法 
            int nzero = 0;  //用来计算连续的零值是几个 
            int temp;            //从原num值中取出的值 

            num = Math.Round(Math.Abs(num), 2);    //将num取绝对值并四舍五入取2位小数 
            str4 = ((long)(num * 100)).ToString();        //将num乘100并转换成字符串形式 
            j = str4.Length;      //找出最高位 
            if (j > 15) { return "溢出"; }
            str2 = str2.Substring(15 - j);   //取出对应位数的str2的值。如：200.55,j为5所以str2=佰拾元角分 

            //循环取出每一位需要转换的值 
            for (i = 0; i < j; i++)
            {
                str3 = str4.Substring(i, 1);          //取出需转换的某一位的值 
                temp = Convert.ToInt32(str3);      //转换为数字 
                if (i != (j - 3) && i != (j - 7) && i != (j - 11) && i != (j - 15))
                {
                    //当所取位数不为元、万、亿、万亿上的数字时 
                    if (str3 == "0")
                    {
                        ch1 = "";
                        ch2 = "";
                        nzero = nzero + 1;
                    }
                    else
                    {
                        if (str3 != "0" && nzero != 0)
                        {
                            ch1 = "零" + str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            ch1 = str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                    }
                }
                else
                {
                    //该位是万亿，亿，万，元位等关键位 
                    if (str3 != "0" && nzero != 0)
                    {
                        ch1 = "零" + str1.Substring(temp * 1, 1);
                        ch2 = str2.Substring(i, 1);
                        nzero = 0;
                    }
                    else
                    {
                        if (str3 != "0" && nzero == 0)
                        {
                            ch1 = str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            if (str3 == "0" && nzero >= 3)
                            {
                                ch1 = "";
                                ch2 = "";
                                nzero = nzero + 1;
                            }
                            else
                            {
                                if (j >= 11)
                                {
                                    ch1 = "";
                                    nzero = nzero + 1;
                                }
                                else
                                {
                                    ch1 = "";
                                    ch2 = str2.Substring(i, 1);
                                    nzero = nzero + 1;
                                }
                            }
                        }
                    }
                }
                if (i == (j - 11) || i == (j - 3))
                    ch2 = str2.Substring(i, 1);
                str5 = str5 + ch1 + ch2;
                if (i == j - 1 && str3 == "0")
                    str5 = str5 + '整';
            }
            if (num == 0)
                str5 = "零元整";
            return str5;
        }

        /// <summary>
        /// 运行Shell命令
        /// </summary>
        /// <param name="cmd">命令行</param>
        /// <returns></returns>
        public static String RunShell(string cmd)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.StandardInput.WriteLine(cmd);
            process.StandardInput.WriteLine("exit");
            process.StandardInput.AutoFlush = true;
            string value = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();
            return value;
        }

        /// <summary>
        /// Xml转实体对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="element">Xml元素</param>
        /// <returns></returns>
        public static T XmlToEntity<T>(this XElement element) where T : class, new()
        {
            T obj = Activator.CreateInstance(typeof(T)) as T;
            AttrSetProp(element.Attributes(), obj);
            WhileElements(element, obj);
            return obj;

            void WhileElements(XElement eles, object param0)
            {
                Type type = param0.GetType();
                foreach (var ele in eles.Elements())
                {
                    var prop = type.GetProperty(ele.Name.LocalName);
                    if (prop.IsNull() || !prop.PropertyType.IsGenericType || prop.PropertyType.GenericTypeArguments.NotExist()) continue;
                    var List = prop.GetValue(param0) ?? Activator.CreateInstance(prop.PropertyType);
                    var itemType = prop.PropertyType.GenericTypeArguments.FirstOrDefault();
                    var Add = prop.PropertyType.GetMethod("Add", new[] { itemType });
                    if (Add.IsNull()) continue;
                    var instance = Activator.CreateInstance(itemType);
                    Add.Invoke(List, new object[] { instance });
                    AttrSetProp(ele.Attributes(), instance);
                    if (ele.HasElements)
                    {
                        WhileElements(ele, instance);
                    }
                    prop.SetValue(param0, List);
                }
            }

            void AttrSetProp(IEnumerable<XAttribute> attrs, object param1)
            {
                Type type = param1.GetType();
                foreach (var attr in attrs)
                {
                    var prop = type.GetProperty(attr.Name.LocalName);
                    if (prop.IsNull()) continue;
                    prop.SetValue(param1, attr.Value);
                }
            }
        }

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
        /// 判断是否为引用类型
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static bool IsClass(this object obj)
        {
            if (obj.GetType().BaseType == typeof(object))
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
        public static bool IsValue(this object obj)
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
        public static bool IsStruct(this object obj)
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
        public static bool IsEnum(this object obj)
        {
            if (obj.GetType().BaseType == typeof(Enum))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 首字母小写
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string FirstLower(this string str)
        {
            if (str.NotNull())
            {
                return str.Substring(0, 1).ToLower() + str.Substring(1, str.Length - 1);
            }
            return str;
        }

        /// <summary>
        /// 类型转换深拷贝
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TTarget MapTo<TSource, TTarget>(this TSource source)
            where TTarget : class
            where TSource : class
        {
            return SmartBuilder.DynamicMethod<Func<TSource, TTarget>>(string.Empty, func =>
            {
                var sourceEntity = func.NewEntity<TSource>(func.EmitParamRef<TSource>(0));
                var targetEntity = func.NewEntity<TTarget>();
                foreach (var sourceItem in typeof(TSource).GetProperties())
                {
                    var targetItem = typeof(TTarget).GetProperty(sourceItem.Name);
                    if (targetItem == null || sourceItem.PropertyType != targetItem.PropertyType)
                        continue;
                    targetEntity.SetValue(sourceItem.Name, sourceEntity.GetValue(sourceItem.Name));
                }
                targetEntity.Output();
                func.EmitReturn();
            }).Invoke(source);
        }

        /// <summary>
        /// 规则性格式化
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramters"></param>
        /// <returns></returns>
        public static string RulesFormat(this string value, params string[] paramters)
        {
            for (int i = 0; i < paramters.Length; i++)
            {
                value = value.Replace($"{{{i}}}", paramters[i]);
            }
            return value;
        }

        /// <summary>
        /// 获取浮点数的精度
        /// </summary>
        /// <param name="m_float"></param>
        /// <returns></returns>
        public static int GetPrecision(this float m_float)
        {
            return GetPrecision((decimal)m_float);
        }

        /// <summary>
        /// 获取浮点数的精度
        /// </summary>
        /// <param name="m_float"></param>
        /// <returns></returns>
        public static int GetPrecision(this double m_float)
        {
            return GetPrecision((decimal)m_float);
        }

        /// <summary>
        /// 获取浮点数的精度
        /// </summary>
        /// <param name="m_float"></param>
        /// <returns></returns>
        public static int GetPrecision(this decimal m_float)
        {
            var str = m_float.ToString();
            var index = str.IndexOf(".");
            if (index > -1)
            {
                return str.Substring(index + 1).TrimEnd('0').Length;
            }
            else
                return 0;
        }

        /// <summary>
        /// 保证字符串都拥有
        /// </summary>
        /// <param name="str"></param>
        /// <param name="parameters"></param>
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


        #region 
        private static bool BasicValidation<T>(FastProperty propertie, T attr, StringBuilder strBuilder) where T : BasicAttribute
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

        private static bool StringValidation(FastProperty propertie, StringRuleAttribute attr, StringBuilder strBuilder)
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

        private static bool NumericValidation(FastProperty propertie, NumericRuleAttribute attr, StringBuilder strBuilder)
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

        private static bool DecimalValidation(FastProperty propertie, DecimalRuleAttribute attr, StringBuilder strBuilder)
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
