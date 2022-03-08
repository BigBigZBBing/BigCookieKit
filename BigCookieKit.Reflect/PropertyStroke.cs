using System.Reflection;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public sealed class PropertyStroke
    {
        private PropertyBuilder propertyBuilder;

        private FieldStroke fieldStroke;

        internal PropertyStroke(PropertyBuilder propertyBuilder, FieldStroke fieldStroke)
        {
            this.propertyBuilder = propertyBuilder;
            this.fieldStroke = fieldStroke;
        }

        public void Constant<T>(T constant)
        {
            fieldStroke.Constant(constant);
        }

        public void Constant(MethodInfo method)
        {
            fieldStroke.Constant(method);
        }

        public void Constant(FuncGenerator generator, LocalBuilder constant)
        {
            fieldStroke.Constant(generator, constant);
        }

        public void CustomAttr(ConstructorInfo ctor, params object[] args)
        {
            propertyBuilder.SetCustomAttribute(new CustomAttributeBuilder(ctor, args));
        }

        public void CustomAttr(ConstructorInfo ctor, params byte[] binary)
        {
            propertyBuilder.SetCustomAttribute(ctor, binary);
        }
    }
}