using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public partial class FuncGenerator
    {
        [Obsolete("Change is wipe new")]
        public FieldObject NewObject(Object value = default(Object))
        {
            return ManagerGX.NewObject(this, value);
        }

        [Obsolete("Change is wipe new")]
        public FieldObject NewObject(LocalBuilder value)
        {
            return new FieldObject(value, this);
        }

        public FieldObject Object(Object value = default(Object))
        {
            return ManagerGX.NewObject(this, value);
        }

        public FieldObject Object(LocalBuilder value)
        {
            return new FieldObject(value, this);
        }

        [Obsolete("Change is wipe new")]
        public FieldString NewString(String value = default(String))
        {
            return ManagerGX.NewString(this, value);
        }

        [Obsolete("Change is wipe new")]
        public FieldString NewString(LocalBuilder value)
        {
            if (value.LocalType != typeof(String)) ManagerGX.ShowEx<TypeAccessException>("Type not is [String]");
            return new FieldString(value, this);
        }

        public FieldString String(String value = default(String))
        {
            return ManagerGX.NewString(this, value);
        }

        public FieldString String(LocalBuilder value)
        {
            if (value.LocalType != typeof(String)) ManagerGX.ShowEx<TypeAccessException>("Type not is [String]");
            return new FieldString(value, this);
        }

        [Obsolete("Change is wipe new")]
        public FieldBoolean NewBoolean(Boolean value = default(Boolean))
        {
            return ManagerGX.NewBoolean(this, value);
        }

        [Obsolete("Change is wipe new")]
        public FieldBoolean NewBoolean(LocalBuilder value)
        {
            if (value.LocalType != typeof(Boolean)) ManagerGX.ShowEx<TypeAccessException>("Type not is [Boolean]");
            return new FieldBoolean(value, this);
        }

        public FieldBoolean Boolean(Boolean value = default(Boolean))
        {
            return ManagerGX.NewBoolean(this, value);
        }

        public FieldBoolean Boolean(LocalBuilder value)
        {
            if (value.LocalType != typeof(Boolean)) ManagerGX.ShowEx<TypeAccessException>("Type not is [Boolean]");
            return new FieldBoolean(value, this);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<Byte> NewByte(Byte value = default(Byte))
        {
            return ManagerGX.NewByte(this, value);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<Byte> NewByte(LocalBuilder value)
        {
            if (value.LocalType != typeof(Byte)) ManagerGX.ShowEx<TypeAccessException>("Type not is [Byte]");
            return new CanCompute<Byte>(value, this);
        }

        public CanCompute<Byte> Byte(Byte value = default(Byte))
        {
            return ManagerGX.NewByte(this, value);
        }

        public CanCompute<Byte> Byte(LocalBuilder value)
        {
            if (value.LocalType != typeof(Byte)) ManagerGX.ShowEx<TypeAccessException>("Type not is [Byte]");
            return new CanCompute<Byte>(value, this);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<Int16> NewInt16(Int16 value = default(Int16))
        {
            return ManagerGX.NewInt16(this, value);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<Int16> NewInt16(LocalBuilder value)
        {
            if (value.LocalType != typeof(Int16)) ManagerGX.ShowEx<TypeAccessException>("Type not is [Int16]");
            return new CanCompute<Int16>(value, this);
        }

        public CanCompute<Int16> Int16(Int16 value = default(Int16))
        {
            return ManagerGX.NewInt16(this, value);
        }

        public CanCompute<Int16> Int16(LocalBuilder value)
        {
            if (value.LocalType != typeof(Int16)) ManagerGX.ShowEx<TypeAccessException>("Type not is [Int16]");
            return new CanCompute<Int16>(value, this);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<Int32> NewInt32(Int32 value = default(Int32))
        {
            return ManagerGX.NewInt32(this, value);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<Int32> NewInt32(LocalBuilder value)
        {
            if (value.LocalType != typeof(Int32)) ManagerGX.ShowEx<TypeAccessException>("Type not is [Int32]");
            return new CanCompute<Int32>(value, this);
        }

        public CanCompute<Int32> Int32(Int32 value = default(Int32))
        {
            return ManagerGX.NewInt32(this, value);
        }

        public CanCompute<Int32> Int32(LocalBuilder value)
        {
            if (value.LocalType != typeof(Int32)) ManagerGX.ShowEx<TypeAccessException>("Type not is [Int32]");
            return new CanCompute<Int32>(value, this);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<Int64> NewInt64(Int64 value = default(Int64))
        {
            return ManagerGX.NewInt64(this, value);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<Int64> NewInt64(LocalBuilder value)
        {
            if (value.LocalType != typeof(Int64)) ManagerGX.ShowEx<TypeAccessException>("Type not is [Int64]");
            return new CanCompute<Int64>(value, this);
        }

        public CanCompute<Int64> Int64(Int64 value = default(Int64))
        {
            return ManagerGX.NewInt64(this, value);
        }

        public CanCompute<Int64> Int64(LocalBuilder value)
        {
            if (value.LocalType != typeof(Int64)) ManagerGX.ShowEx<TypeAccessException>("Type not is [Int64]");
            return new CanCompute<Int64>(value, this);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<Single> NewFloat(Single value = default(Single))
        {
            return ManagerGX.NewFloat(this, value);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<Single> NewFloat(LocalBuilder value)
        {
            if (value.LocalType != typeof(Single)) ManagerGX.ShowEx<TypeAccessException>("Type not is [Single]");
            return new CanCompute<Single>(value, this);
        }

        public CanCompute<Single> Float(Single value = default(Single))
        {
            return ManagerGX.NewFloat(this, value);
        }

        public CanCompute<Single> Float(LocalBuilder value)
        {
            if (value.LocalType != typeof(Single)) ManagerGX.ShowEx<TypeAccessException>("Type not is [Single]");
            return new CanCompute<Single>(value, this);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<Double> NewDouble(Double value = default(Double))
        {
            return ManagerGX.NewDouble(this, value);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<Double> NewDouble(LocalBuilder value)
        {
            if (value.LocalType != typeof(Double)) ManagerGX.ShowEx<TypeAccessException>("Type not is [Double]");
            return new CanCompute<Double>(value, this);
        }

        public CanCompute<Double> Double(Double value = default(Double))
        {
            return ManagerGX.NewDouble(this, value);
        }

        public CanCompute<Double> Double(LocalBuilder value)
        {
            if (value.LocalType != typeof(Double)) ManagerGX.ShowEx<TypeAccessException>("Type not is [Double]");
            return new CanCompute<Double>(value, this);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<Decimal> NewDecimal(Decimal value = default(Decimal))
        {
            return ManagerGX.NewDecimal(this, value);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<Decimal> NewDecimal(LocalBuilder value)
        {
            if (value.LocalType != typeof(Decimal)) ManagerGX.ShowEx<TypeAccessException>("Type not is [Decimal]");
            return new CanCompute<Decimal>(value, this);
        }

        public CanCompute<Decimal> Decimal(Decimal value = default(Decimal))
        {
            return ManagerGX.NewDecimal(this, value);
        }

        public CanCompute<Decimal> Decimal(LocalBuilder value)
        {
            if (value.LocalType != typeof(Decimal)) ManagerGX.ShowEx<TypeAccessException>("Type not is [Decimal]");
            return new CanCompute<Decimal>(value, this);
        }

        [Obsolete("Change is wipe new")]
        public FieldDateTime NewDateTime(DateTime value = default(DateTime))
        {
            return ManagerGX.NewDateTime(this, value);
        }

        [Obsolete("Change is wipe new")]
        public FieldDateTime NewDateTime(LocalBuilder value)
        {
            if (value.LocalType != typeof(DateTime)) ManagerGX.ShowEx<TypeAccessException>("Type not is [DateTime]");
            return new FieldDateTime(value, this);
        }

        public FieldDateTime DateTime(DateTime value = default(DateTime))
        {
            return ManagerGX.NewDateTime(this, value);
        }

        public FieldDateTime DateTime(LocalBuilder value)
        {
            if (value.LocalType != typeof(DateTime)) ManagerGX.ShowEx<TypeAccessException>("Type not is [DateTime]");
            return new FieldDateTime(value, this);
        }

        public FieldArray<T> NewArray<T>(Int32 length)
        {
            return ManagerGX.NewArray<T>(this, length);
        }

        public FieldArray<T> NewArray<T>(CanCompute<Int32> length)
        {
            return ManagerGX.NewArray<T>(this, length);
        }

        public FieldArray<T> NewArray<T>(LocalBuilder value)
        {
            if (value.LocalType != typeof(Int32)) ManagerGX.ShowEx<TypeAccessException>($"Type not is [Int32]");
            return new FieldArray<T>(value, this, -1);
        }

        public FieldEntity<T> NewEntity<T>()
        {
            return ManagerGX.NewEntity<T>(this);
        }

        public FieldEntity<T> NewEntity<T>(LocalBuilder value)
        {
            if (value.LocalType != typeof(T)) ManagerGX.ShowEx<TypeAccessException>($"Type not is [{typeof(T)?.Name}]");
            return new FieldEntity<T>(value, this);
        }

        public FieldEntity<T> NewEntity<T>(T value)
        {
            return ManagerGX.NewEntity<T>(this, value);
        }

        public FieldList<T> NewList<T>()
        {
            return ManagerGX.NewList<T>(this);
        }

        public FieldList<T> NewList<T>(LocalBuilder value)
        {
            if (value.LocalType != typeof(List<T>)) ManagerGX.ShowEx<TypeAccessException>($"Type not is [{typeof(List<T>)?.Name}]");
            return new FieldList<T>(value, this);
        }
    }
}