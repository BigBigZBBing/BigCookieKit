using System;
using System.Collections.Generic;
using System.Text;

namespace BigCookieKit.Attributes
{
    public abstract class BasicAttribute : Attribute
    {
        internal BasicAttribute(string Name)
        {
            this.Name = Name;
        }

        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 验证失败消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 必填
        /// </summary>
        public bool? Required { get; set; }
    }


}
