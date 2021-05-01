using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace BigCookieKit.Reflect
{
    public class MethodStroke

    {
        private MethodBuilder methodBuilder;

        internal MethodStroke(MethodBuilder methodBuilder)
        {
            this.methodBuilder = methodBuilder;
        }

        public void Builder(Action<FuncGenerator> builder)
        {
            builder(new FuncGenerator(methodBuilder.GetILGenerator()));
        }

        public void CustomAttr(ConstructorInfo ctor, params object[] args)
        {
            methodBuilder.SetCustomAttribute(new CustomAttributeBuilder(ctor, args));
        }

        public void CustomAttr(ConstructorInfo ctor, params byte[] binary)
        {
            methodBuilder.SetCustomAttribute(ctor, binary);
        }
    }
}
