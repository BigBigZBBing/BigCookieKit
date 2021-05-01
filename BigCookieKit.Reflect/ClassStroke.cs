using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace BigCookieKit.Reflect
{
    public sealed class ClassStroke
    {
        private TypeBuilder typeBuilder;

        private ConstructorBuilder publicBuilder;

        private ConstructorBuilder staticBuilder;

        internal ClassStroke(TypeBuilder typeBuilder, ConstructorBuilder publicBuilder, ConstructorBuilder staticBuilder)
        {
            this.typeBuilder = typeBuilder;
            this.publicBuilder = publicBuilder;
            this.staticBuilder = staticBuilder;
        }

        public void CustomAttr(ConstructorInfo ctor, params object[] args)
        {
            typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(ctor, args));
        }

        public void CustomAttr(ConstructorInfo ctor, params byte[] binary)
        {
            typeBuilder.SetCustomAttribute(ctor, binary);
        }
    }
}
