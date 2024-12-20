﻿using BigCookieKit.Attributes;
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
        public static string MoneyUpper(this decimal num)
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
        public static string RunShell(string cmd)
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
                    if (prop == null || !prop.PropertyType.IsGenericType || prop.PropertyType.GenericTypeArguments.NotExist()) continue;
                    var List = prop.GetValue(param0) ?? Activator.CreateInstance(prop.PropertyType);
                    var itemType = prop.PropertyType.GenericTypeArguments.FirstOrDefault();
                    var Add = prop.PropertyType.GetMethod("Add", new[] { itemType });
                    if (Add == null) continue;
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
                    if (prop == null) continue;
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
            if (!string.IsNullOrEmpty(str))
            {
                return str.Substring(0, 1).ToLower() + str.Substring(1, str.Length - 1);
            }
            return str;
        }

        /// <summary>
        /// 深拷贝
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">参数不可为空</exception>
        /// <exception cref="TypeAccessException">无效类型</exception>
        public static void CopyTo<TSource, TTarget>(this TSource source, TTarget target)
            where TSource : class
            where TTarget : class
        {
            if (source == null) throw new ArgumentNullException();
            if (target == null) throw new ArgumentNullException();

            string Key = $"{typeof(TSource).FullName}+{typeof(TTarget).FullName}+CopyTo";

            if (Cache.MapToCache.TryGetValue(Key, out Delegate deleg))
            {
                ((Action<TSource, TTarget>)deleg)?.Invoke(source, target);
                return;
            }
            deleg = SmartBuilder.DynamicMethod<Action<TSource, TTarget>>(string.Empty, IL =>
            {
                if (source.IsCustomClass())
                {
                    var _source = IL.Object(IL.ArgumentRef<TSource>(0));
                    var _target = IL.Object(IL.ArgumentRef<TTarget>(1));
                    AutoGenerate(_source, _target);

                    void AutoGenerate(FieldObject source, FieldObject target)
                    {
                        foreach (var targetItem in target.Type.GetProperties())
                        {
                            var sourceItem = source.Type.GetProperty(targetItem.Name);
                            if (sourceItem == null || sourceItem.PropertyType != targetItem.PropertyType)
                                continue;
                            target.SetPropterty(targetItem.Name, source.GetPropterty(targetItem.Name));
                        }
                    }
                }
                else throw new TypeAccessException();

                IL.Return();
            });

            Cache.MapToCache.TryAdd(Key, deleg);

            ((Action<TSource, TTarget>)deleg)?.Invoke(source, target);
        }

        /// <summary>
        /// 深拷贝
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">参数不可为空</exception>
        /// <exception cref="TypeAccessException">无效类型</exception>
        public static TTarget MapTo<TSource, TTarget>(this TSource source)
            where TSource : class
            where TTarget : class
        {
            if (source == null) throw new ArgumentNullException();

            string Key = $"{typeof(TSource).FullName}+{typeof(TTarget).FullName}+MapTo";

            if (Cache.MapToCache.TryGetValue(Key, out Delegate deleg))
            {
                return ((Func<TSource, TTarget>)deleg)?.Invoke(source);
            }
            deleg = SmartBuilder.DynamicMethod<Func<TSource, TTarget>>(string.Empty, IL =>
            {
                if (source.IsCustomClass())
                {
                    var _source = IL.Object(IL.ArgumentRef<TSource>(0));
                    var _target = IL.Object(IL.NewEntity(typeof(TTarget)));
                    AutoGenerate(_source, _target);
                    _target.Output();

                    void AutoGenerate(FieldObject source, FieldObject target)
                    {
                        foreach (var targetItem in target.Type.GetProperties())
                        {
                            var sourceItem = source.Type.GetProperty(targetItem.Name);
                            if (sourceItem == null || sourceItem.PropertyType != targetItem.PropertyType)
                                continue;
                            target.SetPropterty(targetItem.Name, source.GetPropterty(targetItem.Name));
                        }
                    }
                }
                else throw new TypeAccessException();

                IL.Return();
            });

            Cache.MapToCache.TryAdd(Key, deleg);

            return ((Func<TSource, TTarget>)deleg)?.Invoke(source);
        }

        /// <summary>
        /// 深拷贝链表
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<TTarget> MapToList<TSource, TTarget>(this List<TSource> source)
            where TSource : class
            where TTarget : class
        {
            List<TTarget> _list = new List<TTarget>();
            foreach (var item in source)
            {
                _list.Add(item.MapTo<TSource, TTarget>());
            }
            return _list;
        }

        /// <summary>
        /// 规则性格式化 {0} {1} 类似会被全部格式化
        /// </summary>
        /// <param name="value">被格式化的字符串</param>
        /// <param name="paramters">The paramters.</param>
        /// <returns></returns>
        public static string RulesFormat(this string value, params string[] paramters)
        {
            if (paramters != null)
            {
                for (int i = 0; i < paramters.Length; i++)
                {
                    value = value.Replace($"{{{i}}}", paramters[i]);
                }
            }
            return value;
        }

        /// <summary>
        /// 动态创建元组
        /// </summary>
        /// <param name="array">对象组</param>
        /// <returns>元组基</returns>
        public static ITuple CreateValueTuple(object[] array)
        {
            if (array.Length < 8)
            {
                string fullName = "System.ValueTuple`" + array.Length;
                var typesMakeGeneric = array.Select(x => $"[{x.GetType().FullName}]");
                fullName = $"{fullName}[{(string.Join(",", typesMakeGeneric))}]";
                return (ITuple)Activator.CreateInstance(Type.GetType(fullName), array);
            }
            else
            {
                string fullName = "System.ValueTuple`8";
                var splice = array[0..7];
                var typesMakeGeneric = splice.Select(x => $"[{x.GetType().FullName}]").ToList();
                var nextTuple = CreateValueTuple(array[7..]);
                typesMakeGeneric.Add($"[{nextTuple.GetType().FullName}]");
                fullName = $"{fullName}[{(string.Join(",", typesMakeGeneric))}]";
                var merge = new List<object>(splice);
                merge.Add(nextTuple);
                return (ITuple)Activator.CreateInstance(Type.GetType(fullName), merge.ToArray());
            }
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
