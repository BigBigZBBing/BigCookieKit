using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public partial class FuncGenerator
    {
        public LocalBuilder Initialize(Type type, params LocalBuilder[] parameters)
        {
            var item = this.DeclareLocal(type);
            this.Emit(OpCodes.Newobj, type.GetConstructor(parameters.Select(x => x.LocalType).ToArray()));
            this.Emit(OpCodes.Stloc_S, item);
            return item;
        }

        [Obsolete("Change is wipe new")]
        public FieldObject NewObject(LocalBuilder value)
        {
            return new FieldObject(value, this);
        }

        public FieldObject Object(LocalBuilder value)
        {
            return new FieldObject(value, this);
        }

        [Obsolete("Change is wipe new")]
        public FieldString NewString(string value = default(string))
        {
            return ManagerGX.NewString(this, value);
        }

        [Obsolete("Change is wipe new")]
        public FieldString NewString(LocalBuilder value)
        {
            if (value.LocalType != typeof(string)) ManagerGX.ShowEx<TypeAccessException>("Type not is [string]");
            return new FieldString(value, this);
        }

        public FieldString String(string value = default(string))
        {
            return ManagerGX.NewString(this, value);
        }

        public FieldString String(LocalBuilder value)
        {
            if (value.LocalType != typeof(string)) ManagerGX.ShowEx<TypeAccessException>("Type not is [string]");
            return new FieldString(value, this);
        }

        [Obsolete("Change is wipe new")]
        public FieldBoolean NewBoolean(bool value = default(bool))
        {
            return ManagerGX.NewBoolean(this, value);
        }

        [Obsolete("Change is wipe new")]
        public FieldBoolean NewBoolean(LocalBuilder value)
        {
            if (value.LocalType != typeof(bool)) ManagerGX.ShowEx<TypeAccessException>("Type not is [bool]");
            return new FieldBoolean(value, this);
        }

        public FieldBoolean Boolean(bool value = default(bool))
        {
            return ManagerGX.NewBoolean(this, value);
        }

        public FieldBoolean Boolean(LocalBuilder value)
        {
            if (value.LocalType != typeof(bool)) ManagerGX.ShowEx<TypeAccessException>("Type not is [bool]");
            return new FieldBoolean(value, this);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<byte> NewByte(byte value = default(byte))
        {
            return ManagerGX.NewByte(this, value);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<byte> NewByte(LocalBuilder value)
        {
            if (value.LocalType != typeof(byte)) ManagerGX.ShowEx<TypeAccessException>("Type not is [byte]");
            return new CanCompute<byte>(value, this);
        }

        public CanCompute<byte> Byte(byte value = default(byte))
        {
            return ManagerGX.NewByte(this, value);
        }

        public CanCompute<byte> Byte(LocalBuilder value)
        {
            if (value.LocalType != typeof(byte)) ManagerGX.ShowEx<TypeAccessException>("Type not is [byte]");
            return new CanCompute<byte>(value, this);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<short> NewInt16(short value = default(short))
        {
            return ManagerGX.NewInt16(this, value);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<short> NewInt16(LocalBuilder value)
        {
            if (value.LocalType != typeof(short)) ManagerGX.ShowEx<TypeAccessException>("Type not is [short]");
            return new CanCompute<short>(value, this);
        }

        public CanCompute<short> Int16(short value = default(short))
        {
            return ManagerGX.NewInt16(this, value);
        }

        public CanCompute<short> Int16(LocalBuilder value)
        {
            if (value.LocalType != typeof(short)) ManagerGX.ShowEx<TypeAccessException>("Type not is [short]");
            return new CanCompute<short>(value, this);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<int> NewInt32(int value = default(int))
        {
            return ManagerGX.NewInt32(this, value);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<int> NewInt32(LocalBuilder value)
        {
            if (value.LocalType != typeof(int)) ManagerGX.ShowEx<TypeAccessException>("Type not is [int]");
            return new CanCompute<int>(value, this);
        }

        public CanCompute<int> Int32(int value = default(int))
        {
            return ManagerGX.NewInt32(this, value);
        }

        public CanCompute<int> Int32(LocalBuilder value)
        {
            if (value.LocalType != typeof(int)) ManagerGX.ShowEx<TypeAccessException>("Type not is [int]");
            return new CanCompute<int>(value, this);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<long> NewInt64(long value = default(long))
        {
            return ManagerGX.NewInt64(this, value);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<long> NewInt64(LocalBuilder value)
        {
            if (value.LocalType != typeof(long)) ManagerGX.ShowEx<TypeAccessException>("Type not is [long]");
            return new CanCompute<long>(value, this);
        }

        public CanCompute<long> Int64(long value = default(long))
        {
            return ManagerGX.NewInt64(this, value);
        }

        public CanCompute<long> Int64(LocalBuilder value)
        {
            if (value.LocalType != typeof(long)) ManagerGX.ShowEx<TypeAccessException>("Type not is [long]");
            return new CanCompute<long>(value, this);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<float> NewFloat(float value = default(float))
        {
            return ManagerGX.NewFloat(this, value);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<float> NewFloat(LocalBuilder value)
        {
            if (value.LocalType != typeof(float)) ManagerGX.ShowEx<TypeAccessException>("Type not is [float]");
            return new CanCompute<float>(value, this);
        }

        public CanCompute<float> Float(float value = default(float))
        {
            return ManagerGX.NewFloat(this, value);
        }

        public CanCompute<float> Float(LocalBuilder value)
        {
            if (value.LocalType != typeof(float)) ManagerGX.ShowEx<TypeAccessException>("Type not is [float]");
            return new CanCompute<float>(value, this);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<double> NewDouble(double value = default(double))
        {
            return ManagerGX.NewDouble(this, value);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<double> NewDouble(LocalBuilder value)
        {
            if (value.LocalType != typeof(double)) ManagerGX.ShowEx<TypeAccessException>("Type not is [double]");
            return new CanCompute<double>(value, this);
        }

        public CanCompute<double> Double(double value = default(double))
        {
            return ManagerGX.NewDouble(this, value);
        }

        public CanCompute<double> Double(LocalBuilder value)
        {
            if (value.LocalType != typeof(double)) ManagerGX.ShowEx<TypeAccessException>("Type not is [double]");
            return new CanCompute<double>(value, this);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<decimal> NewDecimal(decimal value = default(decimal))
        {
            return ManagerGX.NewDecimal(this, value);
        }

        [Obsolete("Change is wipe new")]
        public CanCompute<decimal> NewDecimal(LocalBuilder value)
        {
            if (value.LocalType != typeof(decimal)) ManagerGX.ShowEx<TypeAccessException>("Type not is [decimal]");
            return new CanCompute<decimal>(value, this);
        }

        public CanCompute<decimal> Decimal(decimal value = default(decimal))
        {
            return ManagerGX.NewDecimal(this, value);
        }

        public CanCompute<decimal> Decimal(LocalBuilder value)
        {
            if (value.LocalType != typeof(decimal)) ManagerGX.ShowEx<TypeAccessException>("Type not is [decimal]");
            return new CanCompute<decimal>(value, this);
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

        public FieldArray<T> NewArray<T>(int length)
        {
            return ManagerGX.NewArray<T>(this, length);
        }

        public FieldArray<T> NewArray<T>(CanCompute<int> length)
        {
            return ManagerGX.NewArray<T>(this, length);
        }

        public FieldArray NewArray(Type type, int length)
        {
            return ManagerGX.NewArray(this, type, length);
        }

        public FieldArray NewArray(Type type, CanCompute<int> length)
        {
            return ManagerGX.NewArray(this, type, length);
        }

        public FieldArray<T> NewArray<T>(LocalBuilder value)
        {
            if (value.LocalType != typeof(int)) ManagerGX.ShowEx<TypeAccessException>($"Type not is [int]");
            return new FieldArray<T>(value, this, -1);
        }

        public FieldEntity<T> NewEntity<T>()
        {
            return ManagerGX.NewEntity<T>(this);
        }

        public FieldEntity NewEntity(Type type)
        {
            return ManagerGX.NewEntity(this, type);
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

        public FieldList NewList(Type type)
        {
            return ManagerGX.NewList(this, type);
        }

        public FieldList<T> NewList<T>(LocalBuilder value)
        {
            if (value.LocalType != typeof(List<T>)) ManagerGX.ShowEx<TypeAccessException>($"Type not is [{typeof(List<T>)?.Name}]");
            return new FieldList<T>(value, this);
        }
    }
}