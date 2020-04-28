using GeneralKit.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace GeneralKit
{
    public static class Kit
    {
        /// <summary>
        /// 判断字符串是否为空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Boolean IsNull(this string str)
        {
            if (str != null && str.Length > 0)
                return false;
            return true;
        }

        /// <summary>
        /// 判断对象是否为空
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Boolean IsNull(this object obj)
        {
            if (obj != null)
                return false;
            return true;
        }

        /// <summary>
        /// 获取枚举的Remark值
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="em"></param>
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
        /// <para>{2}属性描述</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static Boolean ModelValidation<TEntity>(this TEntity Entity) where TEntity : ICheckVerify
        {
            StringBuilder str = new StringBuilder();
            try
            {
                PropertyInfo[] properties = Entity.GetType().GetProperties();
                if (properties != null)
                {
                    foreach (PropertyInfo propertie in properties)
                    {
                        object[] Attributes = propertie.GetCustomAttributes(typeof(MarkAttribute), true);
                        if (Attributes != null)
                        {
                            string Type = propertie.PropertyType.Name;
                            string Name = propertie.Name;
                            object Value = propertie.GetValue(Entity);
                            foreach (object obj in Attributes)
                            {
                                decimal @decimal;
                                MarkAttribute Attr_properties = obj as MarkAttribute;
                                if (Attr_properties != null && !Attr_properties.Error.IsNull())
                                {
                                    if (!Attr_properties.AllowEmpty && (Value.IsNull() || (Value?.ToString().Trim().Length <= 0)))
                                    {
                                        str.Append(string.Format("\r\n" + Attr_properties.Error, Name, Value, Attr_properties.Name));
                                        break;
                                    }
                                    if (Type != typeof(string).Name && decimal.TryParse(Value?.ToString(), out @decimal) && Attr_properties.Equal > -1
                                        && Convert.ToDecimal(Value) != Attr_properties.Equal)
                                    {
                                        str.Append(string.Format("\r\n" + Attr_properties.Error, Name, Value, Attr_properties.Name));
                                        break;
                                    }
                                    if (Type != typeof(string).Name && decimal.TryParse(Value?.ToString(), out @decimal) && Attr_properties.Greater > -1
                                        && Convert.ToDecimal(Value) > Attr_properties.Greater)
                                    {
                                        str.Append(string.Format("\r\n" + Attr_properties.Error, Name, Value, Attr_properties.Name));
                                        break;
                                    }
                                    if (Type != typeof(string).Name && decimal.TryParse(Value?.ToString(), out @decimal) && Attr_properties.Less > -1
                                        && Convert.ToDecimal(Value) < Attr_properties.Less)
                                    {
                                        str.Append(string.Format("\r\n" + Attr_properties.Error, Name, Value, Attr_properties.Name));
                                        break;
                                    }
                                    if (Attr_properties.MaxLength > -1 && Value?.ToString().Length > Attr_properties.MaxLength)
                                    {
                                        str.Append(string.Format("\r\n" + Attr_properties.Error, Name, Value, Attr_properties.Name));
                                        break;
                                    }
                                    if (Attr_properties.MinLength > -1 && Value?.ToString().Length < Attr_properties.MinLength)
                                    {
                                        str.Append(string.Format("\r\n" + Attr_properties.Error, Name, Value, Attr_properties.Name));
                                        break;
                                    }
                                    if ((Attr_properties.RegExp.Length > 0 || Attr_properties.ExpType != ExpType.None)
                                        && (Regex.Match(Value?.ToString(), Attr_properties.RegExp).ToString().IsNull()
                                        || Regex.Match(Value?.ToString(), Attr_properties.ExpType.Remark()).ToString().IsNull()))
                                    {
                                        str.Append(string.Format("\r\n" + Attr_properties.Error, Name, Value, Attr_properties.Name));
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            if (str.Length > 0)
                throw new Exception(str.ToString());
            return true;
        }

        /// <summary>
        /// 金钱转大写
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
        /// <param name="command"></param>
        /// <returns></returns>
        public static String RunCmd(string command)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.StandardInput.WriteLine(command);
            process.StandardInput.WriteLine("exit");
            process.StandardInput.AutoFlush = true;
            string value = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();
            return value;
        }

        /// <summary>
        /// 获取模型的字段字符串
        /// </summary>
        public static String Key<TModel>(this TModel model, Expression<Func<TModel, object>> expression) where TModel : class
        {
            if (expression == null) throw new ArgumentNullException();
            return GetPropertyVlaue("Body.Operand.Member.Name", expression) as string;
        }

        private static object GetPropertyVlaue(string fullPath, object obj)
        {
            var o = obj;
            fullPath.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(point =>
            {
                var p = o.GetType().GetProperty(point, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
                try { o = p.GetValue(o, null); } catch { }
            });
            return o;
        }
    }
}
