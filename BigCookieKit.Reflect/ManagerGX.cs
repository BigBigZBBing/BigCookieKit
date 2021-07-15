﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace BigCookieKit.Reflect
{
    internal static partial class ManagerGX
    {


        internal static FieldString NewString(this EmitBasic basic, String value = default(String))
        {
            return new FieldString(NewField(basic, value), basic);
        }


        internal static FieldBoolean NewBoolean(this EmitBasic basic, Boolean value = default(Boolean))
        {
            return new FieldBoolean(NewField(basic, value), basic);
        }


        internal static CanCompute<Byte> NewByte(this EmitBasic basic, Byte value = default(Byte))
        {
            return new CanCompute<Byte>(NewField(basic, value), basic);
        }


        internal static CanCompute<Int16> NewInt16(this EmitBasic basic, Int16 value = default(Int16))
        {
            return new CanCompute<Int16>(NewField(basic, value), basic);
        }


        internal static CanCompute<Int32> NewInt32(this EmitBasic basic, Int32 value = default(Int32))
        {
            return new CanCompute<Int32>(NewField(basic, value), basic);
        }


        internal static CanCompute<Int64> NewInt64(this EmitBasic basic, Int64 value = default(Int64))
        {
            return new CanCompute<Int64>(NewField(basic, value), basic);
        }


        internal static CanCompute<Single> NewFloat(this EmitBasic basic, Single value = default(Single))
        {
            return new CanCompute<Single>(NewField(basic, value), basic);
        }


        internal static CanCompute<Double> NewDouble(this EmitBasic basic, Double value = default(Double))
        {
            return new CanCompute<Double>(NewField(basic, value), basic);
        }


        internal static CanCompute<Decimal> NewDecimal(this EmitBasic basic, Decimal value = default(Decimal))
        {
            return new CanCompute<Decimal>(NewField(basic, value), basic);
        }


        internal static FieldDateTime NewDateTime(this EmitBasic basic, DateTime value = default(DateTime))
        {
            return new FieldDateTime(NewField(basic, value), basic);
        }


        internal static FieldObject NewObject(this EmitBasic basic, Object value = default(Object))
        {
            return new FieldObject(NewField(basic, value), basic);
        }


        internal static FieldEntity<T> NewEntity<T>(this EmitBasic basic)
        {
            LocalBuilder item = basic.DeclareLocal(typeof(T));
            basic.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
            basic.Emit(OpCodes.Stloc_S, item);
            return new FieldEntity<T>(item, basic);
        }



        internal static FieldEntity<T> NewEntity<T>(this EmitBasic basic, T value)
        {
            return new FieldEntity<T>(basic.MapToEntity(value), basic);
        }



        internal static FieldArray<T> NewArray<T>(this EmitBasic basic, Int32 length = default(Int32))
        {
            LocalBuilder item = basic.DeclareLocal(typeof(T[]));
            basic.IntegerMap(length);
            basic.Emit(OpCodes.Newarr, typeof(T));
            basic.Emit(OpCodes.Stloc_S, item);
            return new FieldArray<T>(item, basic, length);
        }



        internal static FieldArray<T> NewArray<T>(this EmitBasic basic, LocalBuilder length)
        {
            LocalBuilder item = basic.DeclareLocal(typeof(T[]));
            basic.Emit(OpCodes.Ldloc_S, length);
            basic.Emit(OpCodes.Newarr, typeof(T));
            basic.Emit(OpCodes.Stloc_S, item);
            return new FieldArray<T>(item, basic, -1);
        }



        internal static FieldList<T> NewList<T>(this EmitBasic basic)
        {
            LocalBuilder item = basic.DeclareLocal(typeof(List<T>));
            basic.Emit(OpCodes.Newobj, typeof(List<T>).GetConstructor(Type.EmptyTypes));
            basic.Emit(OpCodes.Stloc_S, item);
            return new FieldList<T>(item, basic);
        }



        internal static void For(this EmitBasic basic, LocalBuilder init, LocalBuilder length, Action<CanCompute<Int32>, TabManager> build)
        {
            Label _for = basic.DefineLabel();
            Label _endfor = basic.DefineLabel();
            Label _break = basic.DefineLabel();
            LocalBuilder index = basic.DeclareLocal(typeof(Int32));
            basic.Emit(OpCodes.Ldloc_S, init);
            basic.Emit(OpCodes.Stloc_S, index);
            basic.Emit(OpCodes.Br, _endfor);
            basic.MarkLabel(_for);
            build?.Invoke(new CanCompute<Int32>(index, basic), new TabManager(basic, _break));
            basic.Emit(OpCodes.Ldloc_S, index);
            basic.Emit(OpCodes.Ldc_I4_1);
            basic.Emit(OpCodes.Add);
            basic.Emit(OpCodes.Stloc_S, index);
            basic.MarkLabel(_endfor);
            basic.Emit(OpCodes.Ldloc_S, index);
            basic.Emit(OpCodes.Ldloc_S, length);
            basic.Emit(OpCodes.Clt);
            basic.Emit(OpCodes.Brtrue, _for);
            basic.MarkLabel(_break);
        }



        internal static void For(this EmitBasic basic, Int32 init, LocalBuilder length, Action<CanCompute<Int32>, TabManager> build)
        {
            Label _for = basic.DefineLabel();
            Label _endfor = basic.DefineLabel();
            Label _break = basic.DefineLabel();
            LocalBuilder index = basic.DeclareLocal(typeof(Int32));
            basic.IntegerMap(init);
            basic.Emit(OpCodes.Stloc_S, index);
            basic.Emit(OpCodes.Br, _endfor);
            basic.MarkLabel(_for);
            build?.Invoke(new CanCompute<Int32>(index, basic), new TabManager(basic, _break));
            basic.Emit(OpCodes.Ldloc_S, index);
            basic.Emit(OpCodes.Ldc_I4_1);
            basic.Emit(OpCodes.Add);
            basic.Emit(OpCodes.Stloc_S, index);
            basic.MarkLabel(_endfor);
            basic.Emit(OpCodes.Ldloc_S, index);
            basic.Emit(OpCodes.Ldloc_S, length);
            basic.Emit(OpCodes.Clt);
            basic.Emit(OpCodes.Brtrue, _for);
            basic.MarkLabel(_break);
        }


        internal static void For(this EmitBasic basic, Int32 init, Int32 length, Action<CanCompute<Int32>, TabManager> build)
        {
            Label _for = basic.DefineLabel();
            Label _endfor = basic.DefineLabel();
            Label _break = basic.DefineLabel();
            LocalBuilder index = basic.DeclareLocal(typeof(Int32));
            basic.IntegerMap(init);
            basic.Emit(OpCodes.Stloc_S, index);
            basic.Emit(OpCodes.Br, _endfor);
            basic.MarkLabel(_for);
            build?.Invoke(new CanCompute<Int32>(index, basic), new TabManager(basic, _break));
            basic.Emit(OpCodes.Ldloc_S, index);
            basic.Emit(OpCodes.Ldc_I4_1);
            basic.Emit(OpCodes.Add);
            basic.Emit(OpCodes.Stloc_S, index);
            basic.MarkLabel(_endfor);
            basic.Emit(OpCodes.Ldloc_S, index);
            basic.IntegerMap(length);
            basic.Emit(OpCodes.Clt);
            basic.Emit(OpCodes.Brtrue, _for);
            basic.MarkLabel(_break);
        }


        internal static void Forr(this EmitBasic basic, LocalBuilder init, LocalBuilder length, Action<CanCompute<Int32>, TabManager> build)
        {
            Label _for = basic.DefineLabel();
            Label _endfor = basic.DefineLabel();
            Label _break = basic.DefineLabel();
            LocalBuilder index = basic.DeclareLocal(typeof(Int32));
            basic.Emit(OpCodes.Ldloc_S, init);
            basic.Emit(OpCodes.Stloc_S, index);
            basic.Emit(OpCodes.Br, _endfor);
            basic.MarkLabel(_for);
            build?.Invoke(new CanCompute<Int32>(index, basic), new TabManager(basic, _break));
            basic.Emit(OpCodes.Ldloc_S, index);
            basic.Emit(OpCodes.Ldc_I4_1);
            basic.Emit(OpCodes.Sub);
            basic.Emit(OpCodes.Stloc_S, index);
            basic.MarkLabel(_endfor);
            basic.Emit(OpCodes.Ldloc_S, index);
            basic.Emit(OpCodes.Ldloc_S, length);
            basic.Emit(OpCodes.Bge, _for);
            basic.MarkLabel(_break);
        }



        internal static void Forr(this EmitBasic basic, Int32 init, LocalBuilder length, Action<CanCompute<Int32>, TabManager> build)
        {
            Label _for = basic.DefineLabel();
            Label _endfor = basic.DefineLabel();
            Label _break = basic.DefineLabel();
            LocalBuilder index = basic.DeclareLocal(typeof(Int32));
            basic.IntegerMap(init);
            basic.Emit(OpCodes.Stloc_S, index);
            basic.Emit(OpCodes.Br, _endfor);
            basic.MarkLabel(_for);
            build?.Invoke(new CanCompute<Int32>(index, basic), new TabManager(basic, _break));
            basic.Emit(OpCodes.Ldloc_S, index);
            basic.Emit(OpCodes.Ldc_I4_1);
            basic.Emit(OpCodes.Sub);
            basic.Emit(OpCodes.Stloc_S, index);
            basic.MarkLabel(_endfor);
            basic.Emit(OpCodes.Ldloc_S, index);
            basic.Emit(OpCodes.Ldloc_S, length);
            basic.Emit(OpCodes.Bge, _for);
            basic.MarkLabel(_break);
        }


        internal static void Forr(this EmitBasic basic, Int32 init, Int32 length, Action<CanCompute<Int32>, TabManager> build)
        {
            Label _for = basic.DefineLabel();
            Label _endfor = basic.DefineLabel();
            Label _break = basic.DefineLabel();
            LocalBuilder index = basic.DeclareLocal(typeof(Int32));
            basic.IntegerMap(init);
            basic.Emit(OpCodes.Stloc_S, index);
            basic.Emit(OpCodes.Br, _endfor);
            basic.MarkLabel(_for);
            build?.Invoke(new CanCompute<Int32>(index, basic), new TabManager(basic, _break));
            basic.Emit(OpCodes.Ldloc_S, index);
            basic.Emit(OpCodes.Ldc_I4_1);
            basic.Emit(OpCodes.Sub);
            basic.Emit(OpCodes.Stloc_S, index);
            basic.MarkLabel(_endfor);
            basic.Emit(OpCodes.Ldloc_S, index);
            basic.IntegerMap(length);
            basic.Emit(OpCodes.Bge, _for);
            basic.MarkLabel(_break);
        }



        internal static IEnumerable<KeyValuePair<String, FastProperty>> GetProps(PropertyInfo[] Props, Object Instance)
        {
            foreach (var Prop in Props)
            {
                yield return new KeyValuePair<String, FastProperty>(Prop.Name, new FastProperty(Prop, Instance));
            }
        }



        internal static FieldBoolean IsNull(this EmitBasic basic, LocalBuilder value)
        {
            LocalBuilder assert = basic.DeclareLocal(typeof(Boolean));
            basic.Emit(OpCodes.Ldloc_S, value);
            basic.Emit(OpCodes.Ldnull);
            basic.Emit(OpCodes.Ceq);
            basic.Emit(OpCodes.Stloc_S, assert);
            return new FieldBoolean(assert, basic);
        }



        internal static void ShowEx(String Message)
        {
            throw new Exception(Message);
        }



        private static LocalBuilder NewField<T>(EmitBasic basic, T value)
        {
            LocalBuilder item = basic.DeclareLocal(value.GetType());
            basic.EmitValue(value);
            basic.Emit(OpCodes.Stloc_S, item);
            return item;
        }



        private static LocalBuilder MapToEntity<T>(this EmitBasic basic, T Entity)
        {
            if (Entity == null) ManagerGX.ShowEx("entity is not null!");
            var type = typeof(T);
            var ctor = type.GetConstructor(Type.EmptyTypes);
            if (!ctor.IsPublic) ManagerGX.ShowEx("type need ctor public!");
            var name = type.FullName;
            LocalBuilder model = basic.DeclareLocal(type);
            basic.Emit(OpCodes.Newobj, ctor);
            basic.Emit(OpCodes.Stloc_S, model);

            FastProperty[] emits = type.CachePropsManager();

            for (int i = 0; i < emits.Length; i++)
            {
                var propValue = emits[i].Get(Entity);
                if (propValue == null) continue;
                basic.Emit(OpCodes.Ldloc_S, model);
                basic.EmitValue(propValue, emits[i].PropertyType);
                basic.Emit(OpCodes.Callvirt, emits[i].SetMethod);
            }
            return model;
        }



        private static LocalBuilder MapToEntity(this EmitBasic basic, Object instance, Type type)
        {
            if (instance == null) ManagerGX.ShowEx("entity is not null!");
            var ctor = type.GetConstructor(Type.EmptyTypes);
            if (!ctor.IsPublic) ManagerGX.ShowEx("type need ctor public!");
            var name = type.FullName;
            LocalBuilder model = basic.DeclareLocal(type);
            basic.Emit(OpCodes.Newobj, type);
            basic.Emit(OpCodes.Stloc_S, model);

            FastProperty[] emits = type.CachePropsManager();

            for (int i = 0; i < emits.Length; i++)
            {
                var propValue = emits[i].Get(instance);
                if (propValue == null) continue;
                basic.Emit(OpCodes.Ldloc_S, model);
                basic.EmitValue(propValue, emits[i].PropertyType);
                basic.Emit(OpCodes.Callvirt, emits[i].SetMethod);
            }
            return model;
        }

    }
}
