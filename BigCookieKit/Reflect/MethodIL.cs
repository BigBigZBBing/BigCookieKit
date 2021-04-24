using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace BigCookieKit.Reflect
{
    public class MethodIL
    {
        private MethodBuilder methodBuilder;

        internal MethodIL(MethodBuilder methodBuilder)
        {
            this.methodBuilder = methodBuilder;
        }


    }
}
