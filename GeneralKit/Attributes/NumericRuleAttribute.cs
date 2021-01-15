using GeneralKit.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralKit
{
    /// <summary>
    /// 整数规则
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class NumericRuleAttribute : BasicAttribute
    {
        public NumericRuleAttribute(string Name) : base(Name)
        {
        }

        /// <summary>
        /// 大于
        /// </summary>
        public int? Greater { get; set; }

        /// <summary>
        /// 小于
        /// </summary>
        public int? Less { get; set; }

        /// <summary>
        /// 等于
        /// </summary>
        public int? Equal { get; set; }

        /// <summary>
        /// 不等于
        /// </summary>
        public int? NoEqual { get; set; }
    }
}
