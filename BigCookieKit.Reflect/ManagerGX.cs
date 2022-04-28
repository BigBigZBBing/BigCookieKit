using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    internal static partial class ManagerGX
    {
        internal static FieldString NewString(this EmitBasic basic, string value = default(string))
        {
            return new FieldString(NewField(basic, value), basic);
        }

        internal static FieldBoolean NewBoolean(this EmitBasic basic, bool value = default(bool))
        {
            return new FieldBoolean(NewField(basic, value), basic);
        }

        internal static CanCompute<byte> NewByte(this EmitBasic basic, byte value = default(byte))
        {
            return new CanCompute<byte>(NewField(basic, value), basic);
        }

        internal static CanCompute<short> NewInt16(this EmitBasic basic, short value = default(short))
        {
            return new CanCompute<short>(NewField(basic, value), basic);
        }

        internal static CanCompute<int> NewInt32(this EmitBasic basic, int value = default(int))
        {
            return new CanCompute<int>(NewField(basic, value), basic);
        }

        internal static CanCompute<long> NewInt64(this EmitBasic basic, long value = default(long))
        {
            return new CanCompute<long>(NewField(basic, value), basic);
        }

        internal static CanCompute<float> NewFloat(this EmitBasic basic, float value = default(float))
        {
            return new CanCompute<float>(NewField(basic, value), basic);
        }

        internal static CanCompute<double> NewDouble(this EmitBasic basic, double value = default(double))
        {
            return new CanCompute<double>(NewField(basic, value), basic);
        }

        internal static CanCompute<decimal> NewDecimal(this EmitBasic basic, decimal value = default(decimal))
        {
            return new CanCompute<decimal>(NewField(basic, value), basic);
        }

        internal static FieldDateTime NewDateTime(this EmitBasic basic, DateTime value = default(DateTime))
        {
            return new FieldDateTime(NewField(basic, value), basic);
        }

        internal static FieldObject NewObject(this EmitBasic basic, object value = default(object))
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

        internal static FieldEntity NewEntity(this EmitBasic basic, Type type)
        {
            LocalBuilder item = basic.DeclareLocal(type);
            basic.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
            basic.Emit(OpCodes.Stloc_S, item);
            return new FieldEntity(item, basic);
        }

        internal static FieldEntity<T> NewEntity<T>(this EmitBasic basic, T value)
        {
            return new FieldEntity<T>(basic.MapToEntity(value), basic);
        }

        internal static FieldArray<T> NewArray<T>(this EmitBasic basic, int length = default(int))
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

        internal static FieldArray NewArray(this EmitBasic basic, Type type, int length = default(int))
        {
            LocalBuilder item = basic.DeclareLocal(type);
            basic.IntegerMap(length);
            basic.Emit(OpCodes.Newarr, Type.GetType(type.FullName.Replace("[]", "")));
            basic.Emit(OpCodes.Stloc_S, item);
            return new FieldArray(item, basic, length);
        }

        internal static FieldArray NewArray(this EmitBasic basic, Type type, LocalBuilder length)
        {
            LocalBuilder item = basic.DeclareLocal(type);
            basic.Emit(OpCodes.Ldloc_S, length);
            basic.Emit(OpCodes.Newarr, Type.GetType(type.FullName.Replace("[]", "")));
            basic.Emit(OpCodes.Stloc_S, item);
            return new FieldArray(item, basic, -1);
        }

        internal static FieldList<T> NewList<T>(this EmitBasic basic)
        {
            LocalBuilder item = basic.DeclareLocal(typeof(List<T>));
            basic.Emit(OpCodes.Newobj, typeof(List<T>).GetConstructor(Type.EmptyTypes));
            basic.Emit(OpCodes.Stloc_S, item);
            return new FieldList<T>(item, basic);
        }

        internal static FieldList NewList(this EmitBasic basic, Type type)
        {
            LocalBuilder item = basic.DeclareLocal(type);
            basic.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
            basic.Emit(OpCodes.Stloc_S, item);
            return new FieldList(item, basic);
        }

        internal static void For(this EmitBasic basic, LocalBuilder init, LocalBuilder length, Action<CanCompute<int>, TabManager> build)
        {
            Label _for = basic.DefineLabel();
            Label _endfor = basic.DefineLabel();
            Label _break = basic.DefineLabel();
            LocalBuilder index = basic.DeclareLocal(typeof(int));
            basic.Emit(OpCodes.Ldloc_S, init);
            basic.Emit(OpCodes.Stloc_S, index);
            basic.Emit(OpCodes.Br, _endfor);
            basic.MarkLabel(_for);
            build?.Invoke(new CanCompute<int>(index, basic), new TabManager(basic, _break));
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

        internal static void For(this EmitBasic basic, int init, LocalBuilder length, Action<CanCompute<int>, TabManager> build)
        {
            Label _for = basic.DefineLabel();
            Label _endfor = basic.DefineLabel();
            Label _break = basic.DefineLabel();
            LocalBuilder index = basic.DeclareLocal(typeof(int));
            basic.IntegerMap(init);
            basic.Emit(OpCodes.Stloc_S, index);
            basic.Emit(OpCodes.Br, _endfor);
            basic.MarkLabel(_for);
            build?.Invoke(new CanCompute<int>(index, basic), new TabManager(basic, _break));
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

        internal static void For(this EmitBasic basic, int init, int length, Action<CanCompute<int>, TabManager> build)
        {
            Label _for = basic.DefineLabel();
            Label _endfor = basic.DefineLabel();
            Label _break = basic.DefineLabel();
            LocalBuilder index = basic.DeclareLocal(typeof(int));
            basic.IntegerMap(init);
            basic.Emit(OpCodes.Stloc_S, index);
            basic.Emit(OpCodes.Br, _endfor);
            basic.MarkLabel(_for);
            build?.Invoke(new CanCompute<int>(index, basic), new TabManager(basic, _break));
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

        internal static void Forr(this EmitBasic basic, LocalBuilder init, LocalBuilder length, Action<CanCompute<int>, TabManager> build)
        {
            Label _for = basic.DefineLabel();
            Label _endfor = basic.DefineLabel();
            Label _break = basic.DefineLabel();
            LocalBuilder index = basic.DeclareLocal(typeof(int));
            basic.Emit(OpCodes.Ldloc_S, init);
            basic.Emit(OpCodes.Stloc_S, index);
            basic.Emit(OpCodes.Br, _endfor);
            basic.MarkLabel(_for);
            build?.Invoke(new CanCompute<int>(index, basic), new TabManager(basic, _break));
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

        internal static void Forr(this EmitBasic basic, int init, LocalBuilder length, Action<CanCompute<int>, TabManager> build)
        {
            Label _for = basic.DefineLabel();
            Label _endfor = basic.DefineLabel();
            Label _break = basic.DefineLabel();
            LocalBuilder index = basic.DeclareLocal(typeof(int));
            basic.IntegerMap(init);
            basic.Emit(OpCodes.Stloc_S, index);
            basic.Emit(OpCodes.Br, _endfor);
            basic.MarkLabel(_for);
            build?.Invoke(new CanCompute<int>(index, basic), new TabManager(basic, _break));
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

        internal static void Forr(this EmitBasic basic, int init, int length, Action<CanCompute<int>, TabManager> build)
        {
            Label _for = basic.DefineLabel();
            Label _endfor = basic.DefineLabel();
            Label _break = basic.DefineLabel();
            LocalBuilder index = basic.DeclareLocal(typeof(int));
            basic.IntegerMap(init);
            basic.Emit(OpCodes.Stloc_S, index);
            basic.Emit(OpCodes.Br, _endfor);
            basic.MarkLabel(_for);
            build?.Invoke(new CanCompute<int>(index, basic), new TabManager(basic, _break));
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

        internal static IEnumerable<KeyValuePair<string, FastProperty>> GetProps(PropertyInfo[] Props, object Instance)
        {
            foreach (var Prop in Props)
            {
                yield return new KeyValuePair<string, FastProperty>(Prop.Name, new FastProperty(Prop, Instance));
            }
        }

        internal static FieldBoolean IsNull(this EmitBasic basic, LocalBuilder value)
        {
            LocalBuilder assert = basic.DeclareLocal(typeof(bool));
            basic.Emit(OpCodes.Ldloc_S, value);
            basic.Emit(OpCodes.Ldnull);
            basic.Emit(OpCodes.Ceq);
            basic.Emit(OpCodes.Stloc_S, assert);
            return new FieldBoolean(assert, basic);
        }

        internal static void ShowEx(string Message)
        {
            throw new Exception(Message);
        }

        internal static void ShowEx<T>(string Message) where T : Exception
        {
            throw (T)Activator.CreateInstance(typeof(T), Message);
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

        private static LocalBuilder MapToEntity(this EmitBasic basic, object instance, Type type)
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