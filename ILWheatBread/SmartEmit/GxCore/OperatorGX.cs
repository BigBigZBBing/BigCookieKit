using ILWheatBread.SmartEmit.Field;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace ILWheatBread.SmartEmit
{
    internal static partial class ManagerGX
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static FieldManager<T> Compute<T, T1>(FieldManager<T> field, T1 value, OpCode code)
        {
            field.Output();
            field.EmitValue(value);
            field.Emit(code);
            field.Input();
            return field;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static VariableManager Compute<T>(FieldManager<T> field, LocalBuilder value, OpCode code)
        {
            field.Output();
            field.Emit(OpCodes.Ldloc_S, value);
            field.Emit(code);
            field.Input();
            return field;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static FieldManager<T> Compute<T, T1>(FieldManager<T> field, FieldManager<T1> value, OpCode code)
        {
            field.Output();
            value.Output();
            field.Emit(code);
            field.Input();
            return field;
        }
    }
}
