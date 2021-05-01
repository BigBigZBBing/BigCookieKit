using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;

namespace BigCookieKit.Reflect
{
    public class FieldStroke

    {
        private FieldBuilder fieldBuilder;

        private ConstructorBuilder publicBuilder;

        private ConstructorBuilder staticBuilder;

        public FieldStroke(FieldBuilder fieldBuilder, ConstructorBuilder publicBuilder, ConstructorBuilder staticBuilder)
        {
            this.fieldBuilder = fieldBuilder;
            this.publicBuilder = publicBuilder;
            this.staticBuilder = staticBuilder;
        }

        public void Constant<T>(T constant)
        {
            FuncGenerator func;
            if (fieldBuilder.IsStatic)
            {
                func = new FuncGenerator(staticBuilder.GetILGenerator());
                func.EmitValue<T>(constant);
                func.Emit(OpCodes.Stsfld, fieldBuilder);
            }
            else
            {
                func = new FuncGenerator(publicBuilder.GetILGenerator());
                func.Emit(OpCodes.Ldarg_0);
                func.EmitValue<T>(constant);
                func.Emit(OpCodes.Stfld, fieldBuilder);
            }
        }

        public void Constant(MethodInfo method)
        {
            FuncGenerator func;
            if (fieldBuilder.IsStatic)
            {
                func = new FuncGenerator(staticBuilder.GetILGenerator());
                func.Emit(OpCodes.Call, method);
                func.Emit(OpCodes.Stsfld, fieldBuilder);
            }
            else
            {
                func = new FuncGenerator(publicBuilder.GetILGenerator());
                func.Emit(OpCodes.Ldarg_0);
                func.Emit(OpCodes.Call, method);
                func.Emit(OpCodes.Stfld, fieldBuilder);
            }
        }

        public void CustomAttr(ConstructorInfo ctor, params object[] args)
        {
            fieldBuilder.SetCustomAttribute(new CustomAttributeBuilder(ctor, args));
        }

        public void CustomAttr(ConstructorInfo ctor, params byte[] binary)
        {
            fieldBuilder.SetCustomAttribute(ctor, binary);
        }
    }
}
