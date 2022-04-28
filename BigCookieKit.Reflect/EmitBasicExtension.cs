using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public static class EmitBasicExtension
    {
        public static FieldNullable<T> AsNullable<T>(this FieldManager<T> field) where T : struct
        {
            return new FieldNullable<T>(field, field.generator);
        }

        public static void Argument(this EmitBasic basic, int index)
        {
            switch (index)
            {
                case 0: basic.Emit(OpCodes.Ldarg_0); break;
                case 1: basic.Emit(OpCodes.Ldarg_1); break;
                case 2: basic.Emit(OpCodes.Ldarg_2); break;
                case 3: basic.Emit(OpCodes.Ldarg_3); break;
                default: basic.Emit(OpCodes.Ldarg_S, index); break;
            }
        }

        public static LocalBuilder ArgumentRef<T>(this EmitBasic basic, int index) where T : class
        {
            LocalBuilder param = basic.DeclareLocal(typeof(T));
            basic.Argument(index);
            basic.Emit(OpCodes.Stloc_S, param);
            return param;
        }

        public static LocalBuilder ArgumentRef(this EmitBasic basic, int index, Type type)
        {
            LocalBuilder param = basic.DeclareLocal(type);
            basic.Argument(index);
            basic.Emit(OpCodes.Stloc_S, param);
            return param;
        }

        public static void Throw(this EmitBasic basic, LocalBuilder ex)
        {
            if (ex == null)
            {
                basic.Emit(OpCodes.Rethrow);
                return;
            }
            basic.Emit(OpCodes.Ldloc_S, ex);
            basic.Emit(OpCodes.Throw);
        }

        public static void Throw<T>(this EmitBasic basic, string message = null) where T : Exception
        {
            var _ex = basic.DeclareLocal(typeof(T));
            basic.EmitValue(message);
            basic.Emit(OpCodes.Newobj, typeof(T).GetConstructor(new[] { typeof(string) }));
            basic.Emit(OpCodes.Stloc_S, _ex);
            basic.Throw(_ex);
        }

        internal static MethodManager ReflectMethod<T>(this VariableManager basic, string MethodName)
        {
            Type type = typeof(T);
            MethodInfo method = type.GetMethod(MethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, CallingConventions.Standard, Type.EmptyTypes, null);
            basic.Output();
            basic.Emit(OpCodes.Callvirt, method);
            if (method.ReturnType != null && method.ReturnType != typeof(void)) basic.tiggerPop = true;
            return new MethodManager(basic, method.ReturnType);
        }

        internal static MethodManager ReflectMethod<T>(this VariableManager basic, string MethodName, params LocalBuilder[] parameters)
        {
            Type type = typeof(T);
            MethodInfo method = type.GetMethod(MethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, CallingConventions.Standard, parameters.Select(x => x.LocalType).ToArray(), null);
            basic.Output();
            parameters.ToList().ForEach(x => basic.Emit(OpCodes.Ldloc_S, x));
            basic.Emit(OpCodes.Callvirt, method);
            if (method.ReturnType != null && method.ReturnType != typeof(void)) basic.tiggerPop = true;
            return new MethodManager(basic, method.ReturnType);
        }

        internal static MethodManager ReflectMethod(this VariableManager basic, string MethodName, Type type)
        {
            MethodInfo method = type.GetMethod(MethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, CallingConventions.Standard, Type.EmptyTypes, null);
            basic.Output();
            basic.Emit(OpCodes.Callvirt, method);
            if (method.ReturnType != null && method.ReturnType != typeof(void)) basic.tiggerPop = true;
            return new MethodManager(basic, method.ReturnType);
        }

        internal static MethodManager ReflectMethod(this VariableManager basic, string MethodName, Type type, params LocalBuilder[] parameters)
        {
            MethodInfo method = type.GetMethod(MethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, CallingConventions.Standard, parameters.Select(x => x.LocalType).ToArray(), null);
            if (method == null) throw new MethodAccessException("Not exists this method!");
            basic.Output();
            parameters.ToList().ForEach(x => basic.Emit(OpCodes.Ldloc_S, x));
            basic.Emit(OpCodes.Callvirt, method);
            if (method.ReturnType != null && method.ReturnType != typeof(void)) basic.tiggerPop = true;
            return new MethodManager(basic, method.ReturnType);
        }

        public static MethodManager ReflectStaticMethod(this EmitBasic basic, string MethodName, Type type)
        {
            MethodInfo method = type.GetMethod(MethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance, null, CallingConventions.Standard, Type.EmptyTypes, null);
            basic.Emit(OpCodes.Call, method);
            if (method.ReturnType != null && method.ReturnType != typeof(void)) basic.tiggerPop = true;
            return new MethodManager(basic, method.ReturnType);
        }

        public static MethodManager ReflectStaticMethod(this EmitBasic basic, string MethodName, Type type, params LocalBuilder[] parameters)
        {
            MethodInfo method = type.GetMethod(MethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance, null, CallingConventions.Standard, parameters.Select(x => x.LocalType).ToArray(), null);
            parameters.ToList().ForEach(x => basic.Emit(OpCodes.Ldloc_S, x));
            basic.Emit(OpCodes.Call, method);
            if (method.ReturnType != null && method.ReturnType != typeof(void)) basic.tiggerPop = true;
            return new MethodManager(basic, method.ReturnType);
        }
    }
}