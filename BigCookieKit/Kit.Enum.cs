using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BigCookieKit
{
    public static partial class Kit
    {
        /// <summary>
        /// 根据key对应Value转字典
        /// </summary>
        /// <param name="type">枚举类型</param>
        /// <returns></returns>
        public static IDictionary<int, string> ToEnumKeyValue(Type type)
        {
            if (!type.IsEnum) throw new TypeAccessException();
            string[] Names = Enum.GetNames(type);
            Array Values = Enum.GetValues(type);
            IDictionary<int, string> dic = new Dictionary<int, string>();
            for (int i = 0; i < Values.Length; i++)
            {
                dic.Add((int)Values.GetValue(i), Names[i].ToString());
            }
            return dic;
        }

        /// <summary>
        /// 根据key对应Value转字典
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <returns></returns>
        public static IDictionary<int, string> ToEnumKeyValue<TEnum>() where TEnum : Enum
        {
            return ToEnumKeyValue(typeof(TEnum));
        }

        /// <summary>
        /// 根据key对应Display转字典
        /// </summary>
        /// <param name="type">枚举类型</param>
        /// <returns></returns>
        public static IDictionary<int, string> ToEnumKeyDisplay(Type type)
        {
            if (!type.IsEnum) throw new TypeAccessException();
            string[] Names = Enum.GetNames(type);
            Array Values = Enum.GetValues(type);
            IDictionary<int, string> dic = new Dictionary<int, string>();
            string desc = string.Empty;
            for (int i = 0; i < Values.Length; i++)
            {
                object t = Enum.Parse(type, Values.GetValue(i).ToString());
                MemberInfo[] memInfo = type.GetMember(t.ToString());
                if (memInfo != null && memInfo.Length > 0)
                {
                    object[] attrs = memInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);
                    if (attrs != null && attrs.Length > 0)
                    {
                        desc = ((DisplayAttribute)attrs[0]).Value;
                    }
                }
                dic.Add((Int32)Values.GetValue(i), string.IsNullOrEmpty(desc) ? Names[i].ToString() : desc);
            }
            return dic;
        }

        /// <summary>
        /// 根据key对应Display转字典
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <returns></returns>
        public static IDictionary<Int32, String> ToEnumKeyDisplay<TEnum>() where TEnum : Enum
        {
            return ToEnumKeyDisplay(typeof(TEnum));
        }

        //public static IDictionary<String, String> ToEnumValueDisplay(Type type)
        //{
        //    if (!type.IsEnum) throw new TypeAccessException();
        //    string[] Names = Enum.GetNames(type);
        //    Array Values = Enum.GetValues(type);
        //    IDictionary<string, string> dic = new Dictionary<String, String>();
        //    string desc = String.Empty;
        //    for (Int32 i = 0; i < Values.Length; i++)
        //    {
        //        object t = Enum.Parse(type, Values.GetValue(i).ToString());
        //        MemberInfo[] memInfo = type.GetMember(t.ToString());
        //        if (memInfo != null && memInfo.Length > 0)
        //        {
        //            object[] attrs = memInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);
        //            if (attrs != null && attrs.Length > 0)
        //            {
        //                desc = ((DisplayAttribute)attrs[0]).Value;
        //            }
        //        }
        //        dic.Add((int)Values.GetValue(i), string.IsNullOrEmpty(desc) ? Names[i].ToString() : desc);
        //    }
        //    return dic;
        //}
    }
}
