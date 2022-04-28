using System;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public class CanCompute<T> : FieldManager<T> where T : struct
    {
        internal CanCompute(LocalBuilder stack, ILGenerator generator) : base(stack, generator)
        {
        }

        //public static implicit operator FieldByte(CanCompute<T> field) => new FieldByte(field.instance, field.generator);

        //public static implicit operator FieldInt16(CanCompute<T> field) => new FieldInt16(field.instance, field.generator);

        //public static implicit operator FieldInt32(CanCompute<T> field) => new FieldInt32(field.instance, field.generator);

        //public static implicit operator FieldInt64(CanCompute<T> field) => new FieldInt64(field.instance, field.generator);

        //public static implicit operator FieldFloat(CanCompute<T> field) => new FieldFloat(field.instance, field.generator);

        //public static implicit operator FieldDouble(CanCompute<T> field) => new FieldDouble(field.instance, field.generator);

        //public static implicit operator FieldDecimal(CanCompute<T> field) => new FieldDecimal(field.instance, field.generator);

        public static FieldBoolean operator >(CanCompute<T> field, decimal value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt);
        }

        public static FieldBoolean operator >(CanCompute<T> field, double value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt);
        }

        public static FieldBoolean operator >(CanCompute<T> field, float value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt);
        }

        public static FieldBoolean operator >(CanCompute<T> field, long value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt);
        }

        public static FieldBoolean operator >(CanCompute<T> field, int value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt);
        }

        public static FieldBoolean operator >(CanCompute<T> field, short value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt);
        }

        public static FieldBoolean operator >(CanCompute<T> field, LocalBuilder value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt);
        }

        public static FieldBoolean operator >(CanCompute<T> field, CanCompute<decimal> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt);
        }

        public static FieldBoolean operator >(CanCompute<T> field, CanCompute<double> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt);
        }

        public static FieldBoolean operator >(CanCompute<T> field, CanCompute<float> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt);
        }

        public static FieldBoolean operator >(CanCompute<T> field, CanCompute<long> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt);
        }

        public static FieldBoolean operator >(CanCompute<T> field, CanCompute<int> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt);
        }

        public static FieldBoolean operator >(CanCompute<T> field, CanCompute<short> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt);
        }

        public static FieldBoolean operator <(CanCompute<T> field, decimal value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt);
        }

        public static FieldBoolean operator <(CanCompute<T> field, double value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt);
        }

        public static FieldBoolean operator <(CanCompute<T> field, float value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt);
        }

        public static FieldBoolean operator <(CanCompute<T> field, long value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt);
        }

        public static FieldBoolean operator <(CanCompute<T> field, int value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt);
        }

        public static FieldBoolean operator <(CanCompute<T> field, short value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt);
        }

        public static FieldBoolean operator <(CanCompute<T> field, LocalBuilder value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt);
        }

        public static FieldBoolean operator <(CanCompute<T> field, CanCompute<decimal> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt);
        }

        public static FieldBoolean operator <(CanCompute<T> field, CanCompute<double> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt);
        }

        public static FieldBoolean operator <(CanCompute<T> field, CanCompute<float> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt);
        }

        public static FieldBoolean operator <(CanCompute<T> field, CanCompute<long> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt);
        }

        public static FieldBoolean operator <(CanCompute<T> field, CanCompute<int> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt);
        }

        public static FieldBoolean operator <(CanCompute<T> field, CanCompute<short> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt);
        }

        public static FieldBoolean operator >=(CanCompute<T> field, decimal value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt, OpCodes.Ceq);
        }

        public static FieldBoolean operator >=(CanCompute<T> field, double value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt, OpCodes.Ceq);
        }

        public static FieldBoolean operator >=(CanCompute<T> field, float value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt, OpCodes.Ceq);
        }

        public static FieldBoolean operator >=(CanCompute<T> field, long value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt, OpCodes.Ceq);
        }

        public static FieldBoolean operator >=(CanCompute<T> field, int value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt, OpCodes.Ceq);
        }

        public static FieldBoolean operator >=(CanCompute<T> field, short value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt, OpCodes.Ceq);
        }

        public static FieldBoolean operator >=(CanCompute<T> field, LocalBuilder value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt, OpCodes.Ceq);
        }

        public static FieldBoolean operator >=(CanCompute<T> field, CanCompute<decimal> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt, OpCodes.Ceq);
        }

        public static FieldBoolean operator >=(CanCompute<T> field, CanCompute<double> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt, OpCodes.Ceq);
        }

        public static FieldBoolean operator >=(CanCompute<T> field, CanCompute<float> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt, OpCodes.Ceq);
        }

        public static FieldBoolean operator >=(CanCompute<T> field, CanCompute<long> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt, OpCodes.Ceq);
        }

        public static FieldBoolean operator >=(CanCompute<T> field, CanCompute<int> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt, OpCodes.Ceq);
        }

        public static FieldBoolean operator >=(CanCompute<T> field, CanCompute<short> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt, OpCodes.Ceq);
        }

        public static FieldBoolean operator <=(CanCompute<T> field, decimal value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt, OpCodes.Ceq);
        }

        public static FieldBoolean operator <=(CanCompute<T> field, double value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt, OpCodes.Ceq);
        }

        public static FieldBoolean operator <=(CanCompute<T> field, float value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt, OpCodes.Ceq);
        }

        public static FieldBoolean operator <=(CanCompute<T> field, long value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt, OpCodes.Ceq);
        }

        public static FieldBoolean operator <=(CanCompute<T> field, int value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt, OpCodes.Ceq);
        }

        public static FieldBoolean operator <=(CanCompute<T> field, short value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt, OpCodes.Ceq);
        }

        public static FieldBoolean operator <=(CanCompute<T> field, LocalBuilder value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt, OpCodes.Ceq);
        }

        public static FieldBoolean operator <=(CanCompute<T> field, CanCompute<decimal> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt, OpCodes.Ceq);
        }

        public static FieldBoolean operator <=(CanCompute<T> field, CanCompute<double> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt, OpCodes.Ceq);
        }

        public static FieldBoolean operator <=(CanCompute<T> field, CanCompute<float> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt, OpCodes.Ceq);
        }

        public static FieldBoolean operator <=(CanCompute<T> field, CanCompute<long> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt, OpCodes.Ceq);
        }

        public static FieldBoolean operator <=(CanCompute<T> field, CanCompute<int> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt, OpCodes.Ceq);
        }

        public static FieldBoolean operator <=(CanCompute<T> field, CanCompute<short> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt, OpCodes.Ceq);
        }

        public static FieldBoolean operator ==(CanCompute<T> field, decimal value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        public static FieldBoolean operator ==(CanCompute<T> field, double value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        public static FieldBoolean operator ==(CanCompute<T> field, float value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        public static FieldBoolean operator ==(CanCompute<T> field, long value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        public static FieldBoolean operator ==(CanCompute<T> field, int value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        public static FieldBoolean operator ==(CanCompute<T> field, short value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        public static FieldBoolean operator ==(CanCompute<T> field, LocalBuilder value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        public static FieldBoolean operator ==(CanCompute<T> field, CanCompute<decimal> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        public static FieldBoolean operator ==(CanCompute<T> field, CanCompute<double> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        public static FieldBoolean operator ==(CanCompute<T> field, CanCompute<float> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        public static FieldBoolean operator ==(CanCompute<T> field, CanCompute<long> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        public static FieldBoolean operator ==(CanCompute<T> field, CanCompute<int> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        public static FieldBoolean operator ==(CanCompute<T> field, CanCompute<short> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        public static FieldBoolean operator !=(CanCompute<T> field, decimal value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }

        public static FieldBoolean operator !=(CanCompute<T> field, double value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }

        public static FieldBoolean operator !=(CanCompute<T> field, float value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }

        public static FieldBoolean operator !=(CanCompute<T> field, long value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }

        public static FieldBoolean operator !=(CanCompute<T> field, int value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }

        public static FieldBoolean operator !=(CanCompute<T> field, short value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }

        public static FieldBoolean operator !=(CanCompute<T> field, LocalBuilder value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }

        public static FieldBoolean operator !=(CanCompute<T> field, CanCompute<decimal> value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }

        public static FieldBoolean operator !=(CanCompute<T> field, CanCompute<double> value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }

        public static FieldBoolean operator !=(CanCompute<T> field, CanCompute<float> value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }

        public static FieldBoolean operator !=(CanCompute<T> field, CanCompute<long> value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }

        public static FieldBoolean operator !=(CanCompute<T> field, CanCompute<int> value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }

        public static FieldBoolean operator !=(CanCompute<T> field, CanCompute<short> value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }

        public static CanCompute<decimal> operator +(CanCompute<T> field, decimal value)
        {
            return ManagerGX.Compute<T, decimal>(field, value, OpCodes.Add);
        }

        public static CanCompute<double> operator +(CanCompute<T> field, double value)
        {
            return ManagerGX.Compute<T, double>(field, value, OpCodes.Add);
        }

        public static CanCompute<float> operator +(CanCompute<T> field, float value)
        {
            return ManagerGX.Compute<T, float>(field, value, OpCodes.Add);
        }

        public static CanCompute<long> operator +(CanCompute<T> field, long value)
        {
            return ManagerGX.Compute<T, long>(field, value, OpCodes.Add);
        }

        public static CanCompute<int> operator +(CanCompute<T> field, int value)
        {
            return ManagerGX.Compute<T, int>(field, value, OpCodes.Add);
        }

        public static CanCompute<short> operator +(CanCompute<T> field, short value)
        {
            return ManagerGX.Compute<T, short>(field, value, OpCodes.Add);
        }

        public static CanCompute<byte> operator +(CanCompute<T> field, byte value)
        {
            return ManagerGX.Compute<T, byte>(field, value, OpCodes.Add);
        }

        public static VariableManager operator +(CanCompute<T> field, LocalBuilder value)
        {
            return ManagerGX.Compute<T>(field, value, OpCodes.Add);
        }

        public static CanCompute<decimal> operator +(CanCompute<T> field, CanCompute<decimal> value)
        {
            return ManagerGX.Compute<T, decimal>(field, value, OpCodes.Add);
        }

        public static CanCompute<double> operator +(CanCompute<T> field, CanCompute<double> value)
        {
            return ManagerGX.Compute<T, double>(field, value, OpCodes.Add);
        }

        public static CanCompute<float> operator +(CanCompute<T> field, CanCompute<float> value)
        {
            return ManagerGX.Compute<T, float>(field, value, OpCodes.Add);
        }

        public static CanCompute<long> operator +(CanCompute<T> field, CanCompute<long> value)
        {
            return ManagerGX.Compute<T, long>(field, value, OpCodes.Add);
        }

        public static CanCompute<int> operator +(CanCompute<T> field, CanCompute<int> value)
        {
            return ManagerGX.Compute<T, int>(field, value, OpCodes.Add);
        }

        public static CanCompute<short> operator +(CanCompute<T> field, CanCompute<short> value)
        {
            return ManagerGX.Compute<T, short>(field, value, OpCodes.Add);
        }

        public static CanCompute<byte> operator +(CanCompute<T> field, CanCompute<byte> value)
        {
            return ManagerGX.Compute<T, byte>(field, value, OpCodes.Add);
        }

        public static CanCompute<decimal> operator -(CanCompute<T> field, decimal value)
        {
            return ManagerGX.Compute<T, decimal>(field, value, OpCodes.Sub);
        }

        public static CanCompute<double> operator -(CanCompute<T> field, double value)
        {
            return ManagerGX.Compute<T, double>(field, value, OpCodes.Sub);
        }

        public static CanCompute<float> operator -(CanCompute<T> field, float value)
        {
            return ManagerGX.Compute<T, float>(field, value, OpCodes.Sub);
        }

        public static CanCompute<long> operator -(CanCompute<T> field, long value)
        {
            return ManagerGX.Compute<T, long>(field, value, OpCodes.Sub);
        }

        public static CanCompute<int> operator -(CanCompute<T> field, int value)
        {
            return ManagerGX.Compute<T, int>(field, value, OpCodes.Sub);
        }

        public static CanCompute<short> operator -(CanCompute<T> field, short value)
        {
            return ManagerGX.Compute<T, short>(field, value, OpCodes.Sub);
        }

        public static CanCompute<byte> operator -(CanCompute<T> field, byte value)
        {
            return ManagerGX.Compute<T, byte>(field, value, OpCodes.Sub);
        }

        public static VariableManager operator -(CanCompute<T> field, LocalBuilder value)
        {
            return ManagerGX.Compute(field, value, OpCodes.Sub);
        }

        public static CanCompute<decimal> operator -(CanCompute<T> field, CanCompute<decimal> value)
        {
            return ManagerGX.Compute<T, decimal>(field, value, OpCodes.Sub);
        }

        public static CanCompute<double> operator -(CanCompute<T> field, CanCompute<double> value)
        {
            return ManagerGX.Compute<T, double>(field, value, OpCodes.Sub);
        }

        public static CanCompute<float> operator -(CanCompute<T> field, CanCompute<float> value)
        {
            return ManagerGX.Compute<T, float>(field, value, OpCodes.Sub);
        }

        public static CanCompute<long> operator -(CanCompute<T> field, CanCompute<long> value)
        {
            return ManagerGX.Compute<T, long>(field, value, OpCodes.Sub);
        }

        public static CanCompute<int> operator -(CanCompute<T> field, CanCompute<int> value)
        {
            return ManagerGX.Compute<T, int>(field, value, OpCodes.Sub);
        }

        public static CanCompute<short> operator -(CanCompute<T> field, CanCompute<short> value)
        {
            return ManagerGX.Compute<T, short>(field, value, OpCodes.Sub);
        }

        public static CanCompute<byte> operator -(CanCompute<T> field, CanCompute<byte> value)
        {
            return ManagerGX.Compute<T, byte>(field, value, OpCodes.Sub);
        }

        public static CanCompute<decimal> operator *(CanCompute<T> field, decimal value)
        {
            return ManagerGX.Compute<T, decimal>(field, value, OpCodes.Mul);
        }

        public static CanCompute<double> operator *(CanCompute<T> field, double value)
        {
            return ManagerGX.Compute<T, double>(field, value, OpCodes.Mul);
        }

        public static CanCompute<float> operator *(CanCompute<T> field, float value)
        {
            return ManagerGX.Compute<T, float>(field, value, OpCodes.Mul);
        }

        public static CanCompute<long> operator *(CanCompute<T> field, long value)
        {
            return ManagerGX.Compute<T, long>(field, value, OpCodes.Mul);
        }

        public static CanCompute<int> operator *(CanCompute<T> field, int value)
        {
            return ManagerGX.Compute<T, int>(field, value, OpCodes.Mul);
        }

        public static CanCompute<short> operator *(CanCompute<T> field, short value)
        {
            return ManagerGX.Compute<T, short>(field, value, OpCodes.Mul);
        }

        public static CanCompute<byte> operator *(CanCompute<T> field, byte value)
        {
            return ManagerGX.Compute<T, byte>(field, value, OpCodes.Mul);
        }

        public static VariableManager operator *(CanCompute<T> field, LocalBuilder value)
        {
            return ManagerGX.Compute(field, value, OpCodes.Mul);
        }

        public static CanCompute<decimal> operator *(CanCompute<T> field, CanCompute<decimal> value)
        {
            return ManagerGX.Compute<T, decimal>(field, value, OpCodes.Mul);
        }

        public static CanCompute<double> operator *(CanCompute<T> field, CanCompute<double> value)
        {
            return ManagerGX.Compute<T, double>(field, value, OpCodes.Mul);
        }

        public static CanCompute<float> operator *(CanCompute<T> field, CanCompute<float> value)
        {
            return ManagerGX.Compute<T, float>(field, value, OpCodes.Mul);
        }

        public static CanCompute<long> operator *(CanCompute<T> field, CanCompute<long> value)
        {
            return ManagerGX.Compute<T, long>(field, value, OpCodes.Mul);
        }

        public static CanCompute<int> operator *(CanCompute<T> field, CanCompute<int> value)
        {
            return ManagerGX.Compute<T, int>(field, value, OpCodes.Mul);
        }

        public static CanCompute<short> operator *(CanCompute<T> field, CanCompute<short> value)
        {
            return ManagerGX.Compute<T, short>(field, value, OpCodes.Mul);
        }

        public static CanCompute<byte> operator *(CanCompute<T> field, CanCompute<byte> value)
        {
            return ManagerGX.Compute<T, byte>(field, value, OpCodes.Mul);
        }

        public static CanCompute<decimal> operator /(CanCompute<T> field, decimal value)
        {
            return ManagerGX.Compute<T, decimal>(field, value, OpCodes.Rem);
        }

        public static CanCompute<double> operator /(CanCompute<T> field, double value)
        {
            return ManagerGX.Compute<T, double>(field, value, OpCodes.Rem);
        }

        public static CanCompute<float> operator /(CanCompute<T> field, float value)
        {
            return ManagerGX.Compute<T, float>(field, value, OpCodes.Rem);
        }

        public static CanCompute<long> operator /(CanCompute<T> field, long value)
        {
            return ManagerGX.Compute<T, long>(field, value, OpCodes.Rem);
        }

        public static CanCompute<int> operator /(CanCompute<T> field, int value)
        {
            return ManagerGX.Compute<T, int>(field, value, OpCodes.Rem);
        }

        public static CanCompute<short> operator /(CanCompute<T> field, short value)
        {
            return ManagerGX.Compute<T, short>(field, value, OpCodes.Rem);
        }

        public static CanCompute<byte> operator /(CanCompute<T> field, byte value)
        {
            return ManagerGX.Compute<T, byte>(field, value, OpCodes.Rem);
        }

        public static VariableManager operator /(CanCompute<T> field, LocalBuilder value)
        {
            return ManagerGX.Compute(field, value, OpCodes.Rem);
        }

        public static CanCompute<decimal> operator /(CanCompute<T> field, CanCompute<decimal> value)
        {
            return ManagerGX.Compute<T, decimal>(field, value, OpCodes.Rem);
        }

        public static CanCompute<double> operator /(CanCompute<T> field, CanCompute<double> value)
        {
            return ManagerGX.Compute<T, double>(field, value, OpCodes.Rem);
        }

        public static CanCompute<float> operator /(CanCompute<T> field, CanCompute<float> value)
        {
            return ManagerGX.Compute<T, float>(field, value, OpCodes.Rem);
        }

        public static CanCompute<long> operator /(CanCompute<T> field, CanCompute<long> value)
        {
            return ManagerGX.Compute<T, long>(field, value, OpCodes.Rem);
        }

        public static CanCompute<int> operator /(CanCompute<T> field, CanCompute<int> value)
        {
            return ManagerGX.Compute<T, int>(field, value, OpCodes.Rem);
        }

        public static CanCompute<short> operator /(CanCompute<T> field, CanCompute<short> value)
        {
            return ManagerGX.Compute<T, short>(field, value, OpCodes.Rem);
        }

        public static CanCompute<byte> operator /(CanCompute<T> field, CanCompute<byte> value)
        {
            return ManagerGX.Compute<T, byte>(field, value, OpCodes.Rem);
        }
    }
}