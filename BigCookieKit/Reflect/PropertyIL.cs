using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace BigCookieKit.Reflect
{
    public class PropertyIL
    {
        private PropertyBuilder propertyBuilder;

        internal PropertyIL(PropertyBuilder propertyBuilder)
        {
            this.propertyBuilder = propertyBuilder;
        }


    }
}
