using BigCookieKit.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BigCookieKit
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class RemarkAttribute : Attribute
    {
        private string remark;
        public string Remark { get => remark; set => remark = value; }
        public RemarkAttribute(string remark)
        {
            this.Remark = remark;
        }
    }
}
