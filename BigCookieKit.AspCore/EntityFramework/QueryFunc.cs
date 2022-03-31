using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.AspCore.EntityFramework
{
    public enum QueryFunc
    {
        Equal,
        NoEqual,
        Like,
        NotLike,
        In,
        NotIn,
        GrThen,
        GrThenOrEqual,
        LeThen,
        LeThenOrEqual,
    }
}
