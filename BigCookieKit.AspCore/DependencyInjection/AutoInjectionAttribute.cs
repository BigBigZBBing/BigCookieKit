using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.AspCore.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoInjectionAttribute : Attribute
    {
    }
}
