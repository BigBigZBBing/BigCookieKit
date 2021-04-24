using BigCookieKit.Attributes;
using System;

namespace BigCookieKit
{
    /// <summary>
    /// 字符串规则
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class StringRuleAttribute 
        : BasicAttribute
    {
        public StringRuleAttribute(string Name = "") : base(Name)
        {
        }

        /// <summary>
        /// 最大长度
        /// </summary>
        public object MaxLength { get; set; }

        /// <summary>
        /// 最小长度
        /// </summary>
        public object MinLength { get; set; }

        /// <summary>
        /// 正则表达式
        /// </summary>
        public string RegExp { get; set; }
    }
}
