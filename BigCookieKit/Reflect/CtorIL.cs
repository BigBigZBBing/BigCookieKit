using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace BigCookieKit.Reflect
{
    public class CtorIL
    {
        private ConstructorBuilder constructorBuilder;

        internal CtorIL(ConstructorBuilder constructorBuilder)
        {
            this.constructorBuilder = constructorBuilder;
        }


    }
}
