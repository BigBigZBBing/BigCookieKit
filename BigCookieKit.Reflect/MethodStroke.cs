using System;
using System.Reflection;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public class MethodStroke

    {
        private MethodBuilder methodBuilder;
        private FuncGenerator generator;

        internal MethodStroke(MethodBuilder methodBuilder)
        {
            this.methodBuilder = methodBuilder;
            generator = new FuncGenerator(methodBuilder.GetILGenerator());
        }

        public MethodStroke Builder(Action<FuncGenerator> builder)
        {
            builder(generator);
            return this;
        }

        public MethodStroke CustomAttr(ConstructorInfo ctor, params object[] args)
        {
            methodBuilder.SetCustomAttribute(new CustomAttributeBuilder(ctor, args));
            return this;
        }

        public MethodStroke CustomAttr(ConstructorInfo ctor, params byte[] binary)
        {
            methodBuilder.SetCustomAttribute(ctor, binary);
            return this;
        }

        public void End()
        {
            generator.Return();
        }
    }
}