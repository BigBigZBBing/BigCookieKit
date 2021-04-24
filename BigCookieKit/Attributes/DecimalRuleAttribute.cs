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
    public class DecimalRuleAttribute 
        : NumericRuleAttribute
    {
        public DecimalRuleAttribute(string Name = "") : base(Name)
        {
        }

        /// <summary>
        /// 精度
        /// </summary>
        public object Precision { get; set; } 
    }
}
