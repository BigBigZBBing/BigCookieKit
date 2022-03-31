using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.AspCore.Standard
{
    public abstract class ApiPager
    {
        public int PageIndex { get; set; }

        public int PageCount { get; set; }
    }
}
