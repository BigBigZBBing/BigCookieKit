using System.Linq;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace BigCookieKit.Reflect
{
    public sealed class PropertyStroke
    {
        internal PropertyBuilder propertyBuilder;

        internal FieldStroke fieldStroke;

        internal PropertyStroke() { }

        internal PropertyStroke(PropertyBuilder propertyBuilder, FieldStroke fieldStroke)
        {
            this.propertyBuilder = propertyBuilder;
            this.fieldStroke = fieldStroke;
        }

        public void Constant<T>(T constant)
        {
            fieldStroke.Constant(constant);
        }

        public void Constant(MethodInfo method, params LocalBuilder[] parameters)
        {
            fieldStroke.Constant(method, parameters);
        }

        public LocalBuilder GetValue(FuncGenerator generator)
        {
            return fieldStroke.GetValue(generator);
        }

        public void SetValue(FuncGenerator generator, LocalBuilder constant)
        {
            fieldStroke.SetValue(generator, constant);
        }

        public PropertyStroke AddAttribute(Type type, params object[] args)
        {
            propertyBuilder.SetCustomAttribute(new CustomAttributeBuilder(type.GetConstructor(args.Select(x => x.GetType()).ToArray()), args));
            return this;
        }

        public PropertyStroke AddAttribute(ConstructorInfo ctor, params byte[] binary)
        {
            propertyBuilder.SetCustomAttribute(ctor, binary);
            return this;
        }
    }
}