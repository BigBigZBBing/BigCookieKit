using GeneralKit.Attributes;
using System;

namespace GeneralKit
{
    /// <summary>
    /// 字符串规则
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class StringRuleAttribute : BasicAttribute
    {
        public StringRuleAttribute(string Name) : base(Name)
        {
        }

        /// <summary>
        /// 最大长度
        /// </summary>
        public int? MaxLength { get; set; }

        /// <summary>
        /// 最小长度
        /// </summary>
        public int? MinLength { get; set; }

        /// <summary>
        /// 正则表达式
        /// </summary>
        public string RegExp { get; set; }
    }
}
