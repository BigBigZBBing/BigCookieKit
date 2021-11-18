using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

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

        public void CustomAttribute(Type type, params object[] args)
        {
            typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(type.GetConstructor(args.Select(x => x.GetType()).ToArray()), args));
        }

        public void CustomGeneric(params string[] typeNames)
        {
            typeBuilder.DefineGenericParameters(typeNames);
        }

        public void InheritClass(Type type)
        {
            typeBuilder.SetParent(type);
        }

        public void InheritInterface(Type type)
        {
            typeBuilder.AddInterfaceImplementation(type);
        }
    }
}