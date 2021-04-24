using BigCookieKit.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace BigCookieKit
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class RequiredRuleAttribute 
        : BasicAttribute
    {
        public RequiredRuleAttribute(string Name = "") : base(Name)
        {
            this.Name = Name;
            base.Required = true;
        }
    }


}
