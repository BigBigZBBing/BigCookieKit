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

        //public static implicit operator FieldByte(FieldManager<T> field) => new FieldByte(field.instance, field.generator);

        //public static implicit operator FieldInt16(FieldManager<T> field) => new FieldInt16(field.instance, field.generator);

        //public static implicit operator FieldInt32(FieldManager<T> field) => new FieldInt32(field.instance, field.generator);

        //public static implicit operator FieldInt64(FieldManager<T> field) => new FieldInt64(field.instance, field.generator);

        //public static implicit operator FieldFloat(FieldManager<T> field) => new FieldFloat(field.instance, field.generator);

        //public static implicit operator FieldDouble(FieldManager<T> field) => new FieldDouble(field.instance, field.generator);

        //public static implicit operator FieldDecimal(FieldManager<T> field) => new FieldDecimal(field.instance, field.generator);

        public static implicit operator CanCompute<byte>(FieldManager<T> field) => new CanCompute<byte>(field.instance, field.generator);

        public static implicit operator CanCompute<short>(FieldManager<T> field) => new CanCompute<short>(field.instance, field.generator);

        public static implicit operator CanCompute<int>(FieldManager<T> field) => new CanCompute<int>(field.instance, field.generator);

        public static implicit operator CanCompute<long>(FieldManager<T> field) => new CanCompute<long>(field.instance, field.generator);

        public static implicit operator CanCompute<float>(FieldManager<T> field) => new CanCompute<float>(field.instance, field.generator);

        public static implicit operator CanCompute<double>(FieldManager<T> field) => new CanCompute<double>(field.instance, field.generator);

        public static implicit operator CanCompute<decimal>(FieldManager<T> field) => new CanCompute<decimal>(field.instance, field.generator);
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
                Emit(OpCodes.Box, typeof(object));
            }
            else
            {
                Emit(OpCodes.Castclass, typeof(object));
            }
            temp.Input();
            return temp;
        }

        public virtual MethodManager Call(string methodName, params LocalBuilder[] parameters)
        {
            return this.ReflectMethod(methodName, identity, parameters);
        }

        public static implicit operator LocalBuilder(FieldManager field) => field.instance;
    }
}