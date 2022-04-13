using System.Linq;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace BigCookieKit.Reflect
{
    public sealed class FieldStroke
    {
        internal FieldBuilder fieldBuilder;

        internal ConstructorBuilder publicBuilder;

        internal ConstructorBuilder staticBuilder;

        internal FieldStroke() { }

        internal FieldStroke(FieldBuilder fieldBuilder, ConstructorBuilder publicBuilder, ConstructorBuilder staticBuilder)
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

        public void Constant(MethodInfo method, params LocalBuilder[] parameters)
        {
            FuncGenerator func;
            if (fieldBuilder.IsStatic)
            {
                func = new FuncGenerator(staticBuilder.GetILGenerator());
                foreach (var item in parameters)
                {
                    func.Emit(OpCodes.Ldloc, item);
                }
                func.Emit(OpCodes.Call, method);
                func.Emit(OpCodes.Stsfld, fieldBuilder);
            }
            else
            {
                func = new FuncGenerator(publicBuilder.GetILGenerator());
                func.Emit(OpCodes.Ldarg_0);
                foreach (var item in parameters)
                {
                    func.Emit(OpCodes.Ldloc, item);
                }
                func.Emit(OpCodes.Call, method);
                func.Emit(OpCodes.Stfld, fieldBuilder);
            }
        }

        public LocalBuilder GetValue(FuncGenerator generator)
        {
            FuncGenerator func = generator;
            LocalBuilder item = func.DeclareLocal(fieldBuilder.FieldType);
            if (fieldBuilder.IsStatic)
            {
                func.Emit(OpCodes.Ldsfld, fieldBuilder);
                func.Emit(OpCodes.Stloc, item);
            }
            else
            {
                func.Emit(OpCodes.Ldarg_0);
                func.Emit(OpCodes.Ldfld, fieldBuilder);
                func.Emit(OpCodes.Stloc, item);
            }
            return item;
        }

        public void SetValue(FuncGenerator generator, LocalBuilder constant)
        {
            FuncGenerator func = generator;
            if (fieldBuilder.IsStatic)
            {
                func.Emit(OpCodes.Ldloc, constant);
                func.Emit(OpCodes.Stsfld, fieldBuilder);
            }
            else
            {
                func.Emit(OpCodes.Ldarg_0);
                func.Emit(OpCodes.Ldloc, constant);
                func.Emit(OpCodes.Stfld, fieldBuilder);
            }
        }

        public FieldStroke AddAttribute(Type type, params object[] args)
        {
            fieldBuilder.SetCustomAttribute(new CustomAttributeBuilder(type.GetConstructor(args.Select(x => x.GetType()).ToArray()), args));
            return this;
        }

        public FieldStroke AddAttribute(ConstructorInfo ctor, params byte[] binary)
        {
            fieldBuilder.SetCustomAttribute(ctor, binary);
            return this;
        }
    }
}