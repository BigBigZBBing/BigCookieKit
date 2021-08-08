using System;
using System.Reflection;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public sealed class CtorStroke
    {
        private ConstructorBuilder constructorBuilder;

        internal CtorStroke(ConstructorBuilder constructorBuilder)
        {
            this.constructorBuilder = constructorBuilder;
        }

        public void Builder(Action<FuncGenerator> builder)
        {
            builder(new FuncGenerator(constructorBuilder.GetILGenerator()));
        }

        public void CustomAttr(ConstructorInfo ctor, params object[] args)
        {
            constructorBuilder.SetCustomAttribute(new CustomAttributeBuilder(ctor, args));
        }

        public void CustomAttr(ConstructorInfo ctor, params byte[] binary)
        {
            constructorBuilder.SetCustomAttribute(ctor, binary);
        }
    }
}