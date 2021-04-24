using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace BigCookieKit.Reflect
{
    public class ClassIL
    {
        private TypeBuilder typeBuilder;

        internal ClassIL(TypeBuilder typeBuilder)
        {
            this.typeBuilder = typeBuilder;
        }


    }
}
