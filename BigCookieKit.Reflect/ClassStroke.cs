using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace BigCookieKit.Reflect
{
    public sealed class ClassStroke
    {
        internal TypeBuilder typeBuilder;

        internal ConstructorBuilder publicBuilder;

        internal ConstructorBuilder staticBuilder;

        internal ClassStroke() { }

        internal ClassStroke(TypeBuilder typeBuilder, ConstructorBuilder publicBuilder, ConstructorBuilder staticBuilder)
        {
            this.typeBuilder = typeBuilder;
            this.publicBuilder = publicBuilder;
            this.staticBuilder = staticBuilder;
        }

        public ClassStroke AddAttribute(Type type, params object[] args)
        {
            typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(type.GetConstructor(args.Select(x => x.GetType()).ToArray()), args));
            return this;
        }

        public ClassStroke AddAttribute(ConstructorInfo ctor, params byte[] binary)
        {
            typeBuilder.SetCustomAttribute(ctor, binary);
            return this;
        }

        public ClassStroke AddGeneric(params string[] typeNames)
        {
            typeBuilder.DefineGenericParameters(typeNames);
            return this;
        }

        public ClassStroke AddGeneric(Action<GenericStroke> stroke)
        {
            var generic = new GenericStroke(typeBuilder);
            stroke.Invoke(generic);
            generic.Builder();
            return this;
        }

        public ClassStroke InheritClass(Type type)
        {
            typeBuilder.SetParent(type);
            return this;
        }

        public ClassStroke InheritInterface(Type type)
        {
            typeBuilder.AddInterfaceImplementation(type);
            return this;
        }
    }

}