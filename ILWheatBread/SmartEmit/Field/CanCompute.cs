using System;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace ILWheatBread.SmartEmit.Field
{
    public class CanCompute<T> : FieldManager<T> where T : struct
    {
        internal CanCompute(LocalBuilder stack, ILGenerator generator) : base(stack, generator)
        {
        }

        public static implicit operator FieldInt32(CanCompute<T> field) => new FieldInt32(field.instance, field.generator);

        public static implicit operator FieldInt64(CanCompute<T> field) => new FieldInt64(field.instance, field.generator);

        public static implicit operator FieldFloat(CanCompute<T> field) => new FieldFloat(field.instance, field.generator);

        public static implicit operator FieldDouble(CanCompute<T> field) => new FieldDouble(field.instance, field.generator);

        public static implicit operator FieldDecimal(CanCompute<T> field) => new FieldDecimal(field.instance, field.generator);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator >(CanCompute<T> field, Decimal value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator >(CanCompute<T> field, Double value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator >(CanCompute<T> field, Single value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator >(CanCompute<T> field, Int64 value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator >(CanCompute<T> field, Int32 value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator >(CanCompute<T> field, LocalBuilder value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator >(CanCompute<T> field, CanCompute<Decimal> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator >(CanCompute<T> field, CanCompute<Double> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator >(CanCompute<T> field, CanCompute<Single> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator >(CanCompute<T> field, CanCompute<Int64> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator >(CanCompute<T> field, CanCompute<Int32> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator <(CanCompute<T> field, Decimal value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator <(CanCompute<T> field, Double value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator <(CanCompute<T> field, Single value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator <(CanCompute<T> field, Int64 value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator <(CanCompute<T> field, Int32 value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator <(CanCompute<T> field, LocalBuilder value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator <(CanCompute<T> field, CanCompute<Decimal> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator <(CanCompute<T> field, CanCompute<Double> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator <(CanCompute<T> field, CanCompute<Single> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator <(CanCompute<T> field, CanCompute<Int64> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator <(CanCompute<T> field, CanCompute<Int32> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator >=(CanCompute<T> field, Decimal value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator >=(CanCompute<T> field, Double value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator >=(CanCompute<T> field, Single value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator >=(CanCompute<T> field, Int64 value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator >=(CanCompute<T> field, Int32 value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator >=(CanCompute<T> field, LocalBuilder value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator >=(CanCompute<T> field, CanCompute<Decimal> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator >=(CanCompute<T> field, CanCompute<Double> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator >=(CanCompute<T> field, CanCompute<Single> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator >=(CanCompute<T> field, CanCompute<Int64> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator >=(CanCompute<T> field, CanCompute<Int32> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Cgt, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator <=(CanCompute<T> field, Decimal value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator <=(CanCompute<T> field, Double value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator <=(CanCompute<T> field, Single value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator <=(CanCompute<T> field, Int64 value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator <=(CanCompute<T> field, Int32 value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator <=(CanCompute<T> field, LocalBuilder value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator <=(CanCompute<T> field, CanCompute<Decimal> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator <=(CanCompute<T> field, CanCompute<Double> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator <=(CanCompute<T> field, CanCompute<Single> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator <=(CanCompute<T> field, CanCompute<Int64> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator <=(CanCompute<T> field, CanCompute<Int32> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Clt, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator ==(CanCompute<T> field, Decimal value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator ==(CanCompute<T> field, Double value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator ==(CanCompute<T> field, Single value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator ==(CanCompute<T> field, Int64 value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator ==(CanCompute<T> field, Int32 value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator ==(CanCompute<T> field, LocalBuilder value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator ==(CanCompute<T> field, CanCompute<Decimal> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator ==(CanCompute<T> field, CanCompute<Double> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator ==(CanCompute<T> field, CanCompute<Single> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator ==(CanCompute<T> field, CanCompute<Int64> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator ==(CanCompute<T> field, CanCompute<Int32> value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator !=(CanCompute<T> field, Decimal value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator !=(CanCompute<T> field, Double value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator !=(CanCompute<T> field, Single value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator !=(CanCompute<T> field, Int64 value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator !=(CanCompute<T> field, Int32 value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator !=(CanCompute<T> field, LocalBuilder value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator !=(CanCompute<T> field, CanCompute<Decimal> value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator !=(CanCompute<T> field, CanCompute<Double> value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator !=(CanCompute<T> field, CanCompute<Single> value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator !=(CanCompute<T> field, CanCompute<Int64> value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator !=(CanCompute<T> field, CanCompute<Int32> value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Decimal> operator +(CanCompute<T> field, Decimal value)
        {
            return ManagerGX.Compute<T, Decimal>(field, value, OpCodes.Add);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Double> operator +(CanCompute<T> field, Double value)
        {
            return ManagerGX.Compute<T, Double>(field, value, OpCodes.Add);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Single> operator +(CanCompute<T> field, Single value)
        {
            return ManagerGX.Compute<T, Single>(field, value, OpCodes.Add);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Int64> operator +(CanCompute<T> field, Int64 value)
        {
            return ManagerGX.Compute<T, Int64>(field, value, OpCodes.Add);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Int32> operator +(CanCompute<T> field, Int32 value)
        {
            return ManagerGX.Compute<T, Int32>(field, value, OpCodes.Add);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VariableManager operator +(CanCompute<T> field, LocalBuilder value)
        {
            return ManagerGX.Compute<T>(field, value, OpCodes.Add);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Decimal> operator +(CanCompute<T> field, CanCompute<Decimal> value)
        {
            return ManagerGX.Compute<T, Decimal>(field, value, OpCodes.Add);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Double> operator +(CanCompute<T> field, CanCompute<Double> value)
        {
            return ManagerGX.Compute<T, Double>(field, value, OpCodes.Add);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Single> operator +(CanCompute<T> field, CanCompute<Single> value)
        {
            return ManagerGX.Compute<T, Single>(field, value, OpCodes.Add);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Int64> operator +(CanCompute<T> field, CanCompute<Int64> value)
        {
            return ManagerGX.Compute<T, Int64>(field, value, OpCodes.Add);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Int32> operator +(CanCompute<T> field, CanCompute<Int32> value)
        {
            return ManagerGX.Compute<T, Int32>(field, value, OpCodes.Add);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Decimal> operator -(CanCompute<T> field, Decimal value)
        {
            return ManagerGX.Compute<T, Decimal>(field, value, OpCodes.Sub);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Double> operator -(CanCompute<T> field, Double value)
        {
            return ManagerGX.Compute<T, Double>(field, value, OpCodes.Sub);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Single> operator -(CanCompute<T> field, Single value)
        {
            return ManagerGX.Compute<T, Single>(field, value, OpCodes.Sub);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Int64> operator -(CanCompute<T> field, Int64 value)
        {
            return ManagerGX.Compute<T, Int64>(field, value, OpCodes.Sub);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Int32> operator -(CanCompute<T> field, Int32 value)
        {
            return ManagerGX.Compute<T, Int32>(field, value, OpCodes.Sub);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VariableManager operator -(CanCompute<T> field, LocalBuilder value)
        {
            return ManagerGX.Compute(field, value, OpCodes.Sub);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Decimal> operator -(CanCompute<T> field, CanCompute<Decimal> value)
        {
            return ManagerGX.Compute<T, Decimal>(field, value, OpCodes.Sub);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Double> operator -(CanCompute<T> field, CanCompute<Double> value)
        {
            return ManagerGX.Compute<T, Double>(field, value, OpCodes.Sub);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Single> operator -(CanCompute<T> field, CanCompute<Single> value)
        {
            return ManagerGX.Compute<T, Single>(field, value, OpCodes.Sub);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Int64> operator -(CanCompute<T> field, CanCompute<Int64> value)
        {
            return ManagerGX.Compute<T, Int64>(field, value, OpCodes.Sub);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Int32> operator -(CanCompute<T> field, CanCompute<Int32> value)
        {
            return ManagerGX.Compute<T, Int32>(field, value, OpCodes.Sub);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Decimal> operator *(CanCompute<T> field, Decimal value)
        {
            return ManagerGX.Compute<T, Decimal>(field, value, OpCodes.Mul);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Double> operator *(CanCompute<T> field, Double value)
        {
            return ManagerGX.Compute<T, Double>(field, value, OpCodes.Mul);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Single> operator *(CanCompute<T> field, Single value)
        {
            return ManagerGX.Compute<T, Single>(field, value, OpCodes.Mul);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Int64> operator *(CanCompute<T> field, Int64 value)
        {
            return ManagerGX.Compute<T, Int64>(field, value, OpCodes.Mul);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Int32> operator *(CanCompute<T> field, Int32 value)
        {
            return ManagerGX.Compute<T, Int32>(field, value, OpCodes.Mul);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VariableManager operator *(CanCompute<T> field, LocalBuilder value)
        {
            return ManagerGX.Compute(field, value, OpCodes.Mul);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Decimal> operator *(CanCompute<T> field, CanCompute<Decimal> value)
        {
            return ManagerGX.Compute<T, Decimal>(field, value, OpCodes.Mul);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Double> operator *(CanCompute<T> field, CanCompute<Double> value)
        {
            return ManagerGX.Compute<T, Double>(field, value, OpCodes.Mul);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Single> operator *(CanCompute<T> field, CanCompute<Single> value)
        {
            return ManagerGX.Compute<T, Single>(field, value, OpCodes.Mul);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Int64> operator *(CanCompute<T> field, CanCompute<Int64> value)
        {
            return ManagerGX.Compute<T, Int64>(field, value, OpCodes.Mul);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Int32> operator *(CanCompute<T> field, CanCompute<Int32> value)
        {
            return ManagerGX.Compute<T, Int32>(field, value, OpCodes.Mul);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Decimal> operator /(CanCompute<T> field, Decimal value)
        {
            return ManagerGX.Compute<T, Decimal>(field, value, OpCodes.Rem);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Double> operator /(CanCompute<T> field, Double value)
        {
            return ManagerGX.Compute<T, Double>(field, value, OpCodes.Rem);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Single> operator /(CanCompute<T> field, Single value)
        {
            return ManagerGX.Compute<T, Single>(field, value, OpCodes.Rem);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Int64> operator /(CanCompute<T> field, Int64 value)
        {
            return ManagerGX.Compute<T, Int64>(field, value, OpCodes.Rem);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Int32> operator /(CanCompute<T> field, Int32 value)
        {
            return ManagerGX.Compute<T, Int32>(field, value, OpCodes.Rem);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VariableManager operator /(CanCompute<T> field, LocalBuilder value)
        {
            return ManagerGX.Compute(field, value, OpCodes.Rem);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Decimal> operator /(CanCompute<T> field, CanCompute<Decimal> value)
        {
            return ManagerGX.Compute<T, Decimal>(field, value, OpCodes.Rem);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Double> operator /(CanCompute<T> field, CanCompute<Double> value)
        {
            return ManagerGX.Compute<T, Double>(field, value, OpCodes.Rem);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Single> operator /(CanCompute<T> field, CanCompute<Single> value)
        {
            return ManagerGX.Compute<T, Single>(field, value, OpCodes.Rem);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Int64> operator /(CanCompute<T> field, CanCompute<Int64> value)
        {
            return ManagerGX.Compute<T, Int64>(field, value, OpCodes.Rem);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CanCompute<Int32> operator /(CanCompute<T> field, CanCompute<Int32> value)
        {
            return ManagerGX.Compute<T, Int32>(field, value, OpCodes.Rem);
        }
    }
}
