using ILWheatBread.SmartEmit.Field;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace ILWheatBread.SmartEmit
{
    public partial class FuncGenerator
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldObject NewObject(Object value = default(Object))
        {
            return ManagerGX.NewObject(this, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldObject NewObject(LocalBuilder value)
        {
            return new FieldObject(value, this);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldString NewString(String value = default(String))
        {
            return ManagerGX.NewString(this, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldString NewString(LocalBuilder value)
        {
            if (value.LocalType != typeof(String)) ManagerGX.ShowEx("Type not is [String]");
            return new FieldString(value, this);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldBoolean NewBoolean(Boolean value = default(Boolean))
        {
            return ManagerGX.NewBoolean(this, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldBoolean NewBoolean(LocalBuilder value)
        {
            if (value.LocalType != typeof(Boolean)) ManagerGX.ShowEx("Type not is [Boolean]");
            return new FieldBoolean(value, this);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CanCompute<Int32> NewInt32(Int32 value = default(Int32))
        {
            return ManagerGX.NewInt32(this, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CanCompute<Int32> NewInt32(LocalBuilder value)
        {
            if (value.LocalType != typeof(Int32)) ManagerGX.ShowEx("Type not is [Int32]");
            return new FieldInt32(value, this);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CanCompute<Int64> NewInt64(Int64 value = default(Int64))
        {
            return ManagerGX.NewInt64(this, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CanCompute<Int64> NewInt64(LocalBuilder value)
        {
            if (value.LocalType != typeof(Int64)) ManagerGX.ShowEx("Type not is [Int64]");
            return new FieldInt64(value, this);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CanCompute<Single> NewFloat(Single value = default(Single))
        {
            return ManagerGX.NewFloat(this, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CanCompute<Single> NewFloat(LocalBuilder value)
        {
            if (value.LocalType != typeof(Single)) ManagerGX.ShowEx("Type not is [Single]");
            return new FieldFloat(value, this);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CanCompute<Double> NewDouble(Double value = default(Double))
        {
            return ManagerGX.NewDouble(this, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CanCompute<Double> NewDouble(LocalBuilder value)
        {
            if (value.LocalType != typeof(Double)) ManagerGX.ShowEx("Type not is [Double]");
            return new FieldDouble(value, this);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CanCompute<Decimal> NewDecimal(Decimal value = default(Decimal))
        {
            return ManagerGX.NewDecimal(this, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CanCompute<Decimal> NewDecimal(LocalBuilder value)
        {
            if (value.LocalType != typeof(Decimal)) ManagerGX.ShowEx("Type not is [Decimal]");
            return new FieldDecimal(value, this);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldDateTime NewDateTime(DateTime value = default(DateTime))
        {
            return ManagerGX.NewDateTime(this, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldDateTime NewDateTime(LocalBuilder value)
        {
            if (value.LocalType != typeof(DateTime)) ManagerGX.ShowEx("Type not is [DateTime]");
            return new FieldDateTime(value, this);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldArray<T> NewArray<T>(Int32 length)
        {
            return ManagerGX.NewArray<T>(this, length);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldArray<T> NewArray<T>(CanCompute<Int32> length)
        {
            return ManagerGX.NewArray<T>(this, length);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldArray<T> NewArray<T>(LocalBuilder value)
        {
            if (value.LocalType != typeof(T)) ManagerGX.ShowEx($"Type not is [{typeof(T)?.BaseType?.Name}]");
            return new FieldArray<T>(value, this, -1);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldEntity<T> NewEntity<T>()
        {
            return ManagerGX.NewEntity<T>(this);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldEntity<T> NewEntity<T>(LocalBuilder value)
        {
            if (value.LocalType != typeof(T)) ManagerGX.ShowEx($"Type not is [{typeof(T)?.Name}]");
            return new FieldEntity<T>(value, this);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldEntity<T> NewEntity<T>(T value)
        {
            return ManagerGX.NewEntity<T>(this, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldList<T> NewList<T>()
        {
            return ManagerGX.NewList<T>(this);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldList<T> NewList<T>(LocalBuilder value)
        {
            if (value.LocalType != typeof(List<T>)) ManagerGX.ShowEx($"Type not is [{typeof(List<T>)?.Name}]");
            return new FieldList<T>(value, this);
        }
    }
}
