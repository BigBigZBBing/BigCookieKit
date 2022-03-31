using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.AspCore.EntityFramework
{
    public class JoinSelect<T1, T2>
        where T1 : class
        where T2 : class
    {
        private readonly T1 arg1;
        private readonly T2 arg2;
        private object _value;

        public JoinSelect(T1 arg1, T2 arg2)
        {
            this.arg1 = arg1;
            this.arg2 = arg2;
        }

        public T As<T>(Func<T1, T2, T> selector)
            where T : class
        {
            this._value = selector.Invoke(arg1, arg2);
            return this._value as T;
        }
    }
}
