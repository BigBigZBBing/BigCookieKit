using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace BigCookieKit.Reflect
{
    public sealed class MethodStroke
    {
        internal MethodBuilder methodBuilder;

        internal FuncGenerator generator;

        internal MethodStroke() { }

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

        public MethodStroke AddAttribute(Type type, params object[] args)
        {
            methodBuilder.SetCustomAttribute(new CustomAttributeBuilder(type.GetConstructor(args.Select(x => x.GetType()).ToArray()), args));
            return this;
        }

        public MethodStroke AddAttribute(ConstructorInfo ctor, params byte[] binary)
        {
            methodBuilder.SetCustomAttribute(ctor, binary);
            return this;
        }

        public MethodStroke AddGeneric(params string[] typeNames)
        {
            methodBuilder.DefineGenericParameters(typeNames);
            return this;
        }

        public MethodStroke AddGeneric(Action<GenericStroke> stroke)
        {
            var generic = new GenericStroke(methodBuilder);
            stroke.Invoke(generic);
            generic.Builder();
            return this;
        }

        public void End()
        {
            generator.Return();
        }
    }
}