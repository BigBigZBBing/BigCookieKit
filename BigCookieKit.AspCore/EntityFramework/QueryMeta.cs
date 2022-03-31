using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.AspCore.EntityFramework
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class QueryMeta : Attribute
    {
        public QueryFunc Where { get; set; }

        public string Name { get; set; }

        public QueryMeta(QueryFunc where)
        {
            this.Where = where;
        }

        public QueryMeta(QueryFunc Where, string Name)
        {
            this.Where = Where;
            this.Name = Name;
        }
    }
}
