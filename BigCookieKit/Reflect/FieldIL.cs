using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace BigCookieKit.Reflect
{
    public class FieldIL
    {
        private FieldBuilder fieldBuilder;

        internal FieldIL(FieldBuilder fieldBuilder)
        {
            this.fieldBuilder = fieldBuilder;
        }


    }
}
