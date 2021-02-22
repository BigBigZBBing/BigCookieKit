using BigCookieKit.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit
{
    /// <summary>
    /// 浮点数规则
    /// </summary>
    public class DecimalRuleAttribute : BasicAttribute
    {
        public DecimalRuleAttribute(string Name) : base(Name)
        {
        }

        /// <summary>
        /// 大于
        /// </summary>
        public decimal? Greater { get; set; }

        /// <summary>
        /// 小于
        /// </summary>
        public decimal? Less { get; set; }

        /// <summary>
        /// 等于
        /// </summary>
        public decimal? Equal { get; set; }

        /// <summary>
        /// 不等于
        /// </summary>
        public decimal? NoEqual { get; set; }

        /// <summary>
        /// 精度
        /// </summary>
        public int? Precision { get; set; }
    }
}
