using System;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public class FieldManager<T> : FieldManager
    {
        internal FieldManager(LocalBuilder stack, ILGenerator generator) : base(stack, generator)
        {
        }

        public static implicit operator LocalBuilder(FieldManager<T> field) => field.instance;

        public static implicit operator FieldString(FieldManager<T> field) => new FieldString(field.instance, field.generator);

        public static implicit operator FieldBoolean(FieldManager<T> field) => new FieldBoolean(field.instance, field.generator);

        public static implicit operator FieldDateTime(FieldManager<T> field) => new FieldDateTime(field.instance, field.generator);

        public static implicit operator FieldByte(FieldManager<T> field) => new FieldByte(field.instance, field.generator);

        public static implicit operator FieldInt16(FieldManager<T> field) => new FieldInt16(field.instance, field.generator);

        public static implicit operator FieldInt32(FieldManager<T> field) => new FieldInt32(field.instance, field.generator);

        public static implicit operator FieldInt64(FieldManager<T> field) => new FieldInt64(field.instance, field.generator);

        public static implicit operator FieldFloat(FieldManager<T> field) => new FieldFloat(field.instance, field.generator);

        public static implicit operator FieldDouble(FieldManager<T> field) => new FieldDouble(field.instance, field.generator);

        public static implicit operator FieldDecimal(FieldManager<T> field) => new FieldDecimal(field.instance, field.generator);

        public static implicit operator CanCompute<Byte>(FieldManager<T> field) => new CanCompute<Byte>(field.instance, field.generator);

        public static implicit operator CanCompute<Int16>(FieldManager<T> field) => new CanCompute<Int16>(field.instance, field.generator);

        public static implicit operator CanCompute<Int32>(FieldManager<T> field) => new CanCompute<Int32>(field.instance, field.generator);

        public static implicit operator CanCompute<Int64>(FieldManager<T> field) => new CanCompute<Int64>(field.instance, field.generator);

        public static implicit operator CanCompute<Single>(FieldManager<T> field) => new CanCompute<Single>(field.instance, field.generator);

        public static implicit operator CanCompute<Double>(FieldManager<T> field) => new CanCompute<Double>(field.instance, field.generator);

        public static implicit operator CanCompute<Decimal>(FieldManager<T> field) => new CanCompute<Decimal>(field.instance, field.generator);
    }

    public class FieldManager : VariableManager
    {
        internal Type identity;

        internal FieldManager(LocalBuilder stack, ILGenerator generator) : base(stack, generator)
        {
            identity = stack.LocalType;
        }

        public FieldObject AsObject()
        {
            var temp = this.NewObject();
            Output();
            if (identity.IsValueType)
            {
                Emit(OpCodes.Box, typeof(Object));
            }
            else
            {
                Emit(OpCodes.Castclass, typeof(Object));
            }
            temp.Input();
            return temp;
        }

        public virtual MethodManager Call(String methodName, params LocalBuilder[] parameters)
        {
            return this.ReflectMethod(methodName, identity, parameters);
        }

        public static implicit operator LocalBuilder(FieldManager field) => field.instance;
    }
}