using BigCookieKit.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit
{
    /// <summary>
    /// 整数规则
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class NumericRuleAttribute 
        : BasicAttribute
    {
        public NumericRuleAttribute(string Name = "") : base(Name)
        {
        }

        /// <summary>
        /// 大于
        /// </summary>
        public object Greater { get; set; }

        /// <summary>
        /// 小于
        /// </summary>
        public object Less { get; set; }

        /// <summary>
        /// 等于
        /// </summary>
        public object Equal { get; set; }

        /// <summary>
        /// 不等于
        /// </summary>
        public object NoEqual { get; set; }
    }
}
