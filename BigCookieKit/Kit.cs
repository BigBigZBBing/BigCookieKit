using BigCookieKit.Attributes;
using BigCookieKit.Reflect;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace BigCookieKit
{
    /// <summary>
    /// 通用工具箱
    /// </summary>
    public static partial class Kit
    {
        /// <summary>
        /// 金钱转大写
        /// <para>(保留两位小数)</para>
        /// </summary>
        /// <param name="num">数字</param>
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
        /// 首字母小写
        /// </summary>
        /// <param name="str">The string.</param>
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
        /// 深拷贝
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static TSource MapTo<TSource>(this TSource source)
            where TSource : class
        {
            if (Cache.MapToCache.TryGetValue($"{typeof(TSource).FullName}+{typeof(TSource).FullName}", out Delegate deleg))
            {
                return ((Func<TSource, TSource>)deleg)?.Invoke(source);
            }
            deleg = SmartBuilder.DynamicMethod<Func<TSource, TSource>>(string.Empty, IL =>
            {
                if (source is Stream)
                {
                    var _source = IL.Object(IL.ArgumentRef<TSource>(0));
                    var _target = IL.Object(new MemoryStream());
                    var begin = IL.Object(SeekOrigin.Begin);
                    _source.Call("Seek", IL.Int64(), begin);
                    _source.Call("CopyTo", _target);
                    _target.Output();
                }
                else if (source is IDictionary)
                {
                    var _source = IL.Object(IL.ArgumentRef<TSource>(0));
                    var _target = IL.Object(Activator.CreateInstance(typeof(TSource)));

                    var keys = IL.Object(_source.GetPropterty("Keys"));
                    IL.For(0, keys.GetPropterty("Count"), (index, tab) =>
                    {
                        index.Output();

                    });
                }
                else if (typeof(TSource).IsClass && typeof(TSource).IsPrimitive)
                {
                    var _source = IL.NewEntity<TSource>(IL.ArgumentRef<TSource>(0));
                    var _target = IL.NewEntity<TSource>();
                    foreach (var sourceItem in typeof(TSource).GetProperties())
                    {
                        var targetItem = typeof(TSource).GetProperty(sourceItem.Name, BindingFlags.Public | BindingFlags.Instance);
                        if (targetItem == null || sourceItem.PropertyType != targetItem.PropertyType)
                            continue;
                        _target.SetValue(sourceItem.Name, _source.GetValue(sourceItem.Name));
                    }
                    _target.Output();
                }
                else throw new TypeAccessException();

                IL.Return();
            });

            if (!Cache.MapToCache.TryAdd($"{typeof(TSource).FullName}+{typeof(TSource).FullName}", deleg))
            {
                throw new ArgumentException();
            }

            return ((Func<TSource, TSource>)deleg)?.Invoke(source);
        }

        /// <summary>
        /// 规则性格式化 {0} {1} 类似会被全部格式化
        /// </summary>
        /// <param name="value">被格式化的字符串</param>
        /// <param name="paramters">The paramters.</param>
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
        /// 博伊尔-摩尔算法匹配单次
        /// </summary>
        /// <param name="source">内容</param>
        /// <param name="pattern">模式</param>
        /// <param name="offset">初始偏移量</param>
        /// <returns></returns>
        public static int BoyerMooreFirstMatch(byte[] source, byte[] pattern, int offset = 0)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            var bads = new int[256];
            var total = source.Length;
            var length = pattern.Length;
            var b_length = total - length;
            var last = length - 1;

            if (total == 0 || length == 0 || length > total) return -1;

            for (var i = 0; i < 256; i++)
            {
                bads[i] = length;
            }

            for (var i = last; i > 0; i--)
            {
                var pet = pattern[i];
                for (int t = 0; t < length - i; t++)
                {
                    if (pet == pattern[t])
                    {
                        bads[pet] = last - t;
                        break;
                    }
                }
            }

            while (offset <= b_length)
            {
                int i;
                for (i = last; source[offset + i] == pattern[i]; i--)
                {
                    if (i == 0) return offset;
                }
                offset += bads[source[offset + i]];
            }

            return -1;
        }

        /// <summary>
        /// 博伊尔-摩尔算法匹配单次
        /// </summary>
        /// <param name="source">内容</param>
        /// <param name="pattern">模式</param>
        /// <param name="offset">初始偏移量</param>
        /// <returns></returns>
        public static int BoyerMooreFirstMatch(char[] source, char[] pattern, int offset = 0)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            var bads = new int[256];
            var total = source.Length;
            var length = pattern.Length;
            var b_length = total - length;
            var last = length - 1;

            if (total == 0 || length == 0 || length > total) return -1;

            for (var i = 0; i < 256; i++)
            {
                bads[i] = length;
            }

            for (var i = last; i > 0; i--)
            {
                var pet = pattern[i];
                for (int t = 0; t < length - i; t++)
                {
                    if (pet == pattern[t])
                    {
                        bads[pet] = last - t;
                        break;
                    }
                }
            }

            while (offset <= b_length)
            {
                int i;
                for (i = last; source[offset + i] == pattern[i]; i--)
                {
                    if (i == 0) return offset;
                }
                offset += bads[source[offset + i]];
            }

            return -1;
        }

        /// <summary>
        /// 博伊尔-摩尔算法匹配所有
        /// </summary>
        /// <param name="source">内容</param>
        /// <param name="pattern">模式</param>
        /// <param name="offset">初始偏移量</param>
        /// <returns></returns>
        public static int[] BoyerMooreMatchAll(byte[] source, byte[] pattern, int offset = 0)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            List<int> res = new List<int>();
            var bads = new int[256];
            var total = source.Length;
            var length = pattern.Length;
            var b_length = total - length;
            var last = length - 1;

            if (total == 0 || length == 0 || length > total) return res.ToArray();

            for (var i = 0; i < 256; i++)
            {
                bads[i] = length;
            }

            for (var i = last; i > 0; i--)
            {
                var pet = pattern[i];
                for (int t = 0; t < length - i; t++)
                {
                    if (pet == pattern[t])
                    {
                        bads[pet] = last - t;
                        break;
                    }
                }
            }

            while (offset <= b_length)
            {
                int i;
                for (i = last; source[offset + i] == pattern[i]; i--)
                {
                    if (i == 0)
                    {
                        res.Add(offset);
                        break;
                    }
                }
                offset += bads[source[offset + i]];
            }

            return res.ToArray();
        }

        /// <summary>
        /// 博伊尔-摩尔算法匹配所有
        /// </summary>
        /// <param name="source">内容</param>
        /// <param name="pattern">模式</param>
        /// <param name="offset">初始偏移量</param>
        /// <returns></returns>
        public static int[] BoyerMooreMatchAll(char[] source, char[] pattern, int offset = 0)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            List<int> res = new List<int>();
            var bads = new int[256];
            var total = source.Length;
            var length = pattern.Length;
            var b_length = total - length;
            var last = length - 1;

            if (total == 0 || length == 0 || length > total) return res.ToArray();

            for (var i = 0; i < 256; i++)
            {
                bads[i] = length;
            }

            for (var i = last; i > 0; i--)
            {
                var pet = pattern[i];
                for (int t = 0; t < length - i; t++)
                {
                    if (pet == pattern[t])
                    {
                        bads[pet] = last - t;
                        break;
                    }
                }
            }

            while (offset <= b_length)
            {
                int i;
                for (i = last; source[offset + i] == pattern[i]; i--)
                {
                    if (i == 0)
                    {
                        res.Add(offset);
                        break;
                    }
                }
                offset += bads[source[offset + i]];
            }

            return res.ToArray();
        }

        /// <summary>
        /// 全局组件缓存
        /// </summary>
        public static partial class Cache
        {
            /// <summary>
            /// MapTo构建的缓存
            /// </summary>
            public static ConcurrentDictionary<string, Delegate> MapToCache = new ConcurrentDictionary<string, Delegate>();
        }
    }
}
