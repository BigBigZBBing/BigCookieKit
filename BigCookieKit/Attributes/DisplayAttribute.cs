using BigCookieKit.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BigCookieKit
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class DisplayAttribute
        : Attribute
    {
        public string Value { get; set; }

        public DisplayAttribute(string value)
        {
            this.Value = value;
        }
    }
}
