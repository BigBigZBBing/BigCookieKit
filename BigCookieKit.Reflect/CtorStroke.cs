using System;
using System.Reflection;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public sealed class CtorStroke
    {
        private ConstructorBuilder constructorBuilder;
        private FuncGenerator generator;

        internal CtorStroke(ConstructorBuilder constructorBuilder)
        {
            this.constructorBuilder = constructorBuilder;
            generator = new FuncGenerator(constructorBuilder.GetILGenerator());
        }

        public CtorStroke Builder(Action<FuncGenerator> builder)
        {
            builder(generator);
            return this;
        }

        public CtorStroke CustomAttr(ConstructorInfo ctor, params object[] args)
        {
            constructorBuilder.SetCustomAttribute(new CustomAttributeBuilder(ctor, args));
            return this;
        }

        public CtorStroke CustomAttr(ConstructorInfo ctor, params byte[] binary)
        {
            constructorBuilder.SetCustomAttribute(ctor, binary);
            return this;
        }

        public void End()
        {
            generator.Return();
        }
    }
}