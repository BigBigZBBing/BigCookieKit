using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    internal static partial class ManagerGX
    {
        internal static FieldBoolean Comparer<T>(FieldManager<T> field, T value, params OpCode[] codes)
        {
            var res = field.NewBoolean();
            field.Output();
            foreach (var code in codes)
            {
                field.EmitValue(value);
                field.Emit(code);
            }
            field.Emit(OpCodes.Stloc_S, res);
            return res;
        }

        internal static FieldBoolean Comparer<T, T1>(CanCompute<T> field, T1 value, params OpCode[] codes)
            where T : struct
            where T1 : struct
        {
            var res = field.NewBoolean();
            field.Output();
            foreach (var code in codes)
            {
                field.EmitValue(value);
                field.Emit(code);
            }
            field.Emit(OpCodes.Stloc_S, res);
            return res;
        }

        internal static FieldBoolean Comparer<T>(FieldManager<T> field, LocalBuilder value, params OpCode[] codes)
        {
            var res = field.NewBoolean();
            field.Output();
            foreach (var code in codes)
            {
                field.Emit(OpCodes.Ldloc_S, value);
                field.Emit(code);
            }
            field.Emit(OpCodes.Stloc_S, res);
            return res;
        }

        internal static FieldBoolean Comparer<T, T1>(FieldManager<T> field, FieldManager<T1> value, params OpCode[] codes)
        {
            var res = field.NewBoolean();
            field.Output();
            foreach (var code in codes)
            {
                value.Output();
                field.Emit(code);
            }
            field.Emit(OpCodes.Stloc_S, res);
            return res;
        }

        internal static FieldManager<T> Compute<T, T1>(FieldManager<T> field, T1 value, params OpCode[] codes)
        {
            field.Output();
            field.EmitValue(value);
            foreach (var code in codes)
            {
                field.Emit(code);
            }
            field.Input();
            return field;
        }

        internal static VariableManager Compute<T>(FieldManager<T> field, LocalBuilder value, params OpCode[] codes)
        {
            field.Output();
            field.Emit(OpCodes.Ldloc_S, value);
            foreach (var code in codes)
            {
                field.Emit(code);
            }
            field.Input();
            return field;
        }

        internal static FieldManager<T> Compute<T, T1>(FieldManager<T> field, FieldManager<T1> value, params OpCode[] codes)
        {
            field.Output();
            value.Output();
            foreach (var code in codes)
            {
                field.Emit(code);
            }
            field.Input();
            return field;
        }
    }
}