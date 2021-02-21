using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ILWheatBread.SmartEmit.Field
{
    public class FieldNullable<T> : FieldManager<T> where T : struct
    {

        internal MethodInfo ValueInfo { get; set; }

        internal MethodInfo HasValueInfo { get; set; }

        internal MethodInfo GetValueOrDefaultInfo { get; set; }


        internal FieldNullable(LocalBuilder stack, ILGenerator generator) : base(ToNullable(stack, generator), generator)
        {
            ValueInfo = typeof(Nullable<T>).GetProperty("Value").GetGetMethod();
            HasValueInfo = typeof(Nullable<T>).GetProperty("HasValue").GetGetMethod();
            GetValueOrDefaultInfo = typeof(Nullable<T>).GetMethod("GetValueOrDefault", Type.EmptyTypes);
        }

        internal static LocalBuilder ToNullable(LocalBuilder stack, ILGenerator generator)
        {
            LocalBuilder NullValue = generator.DeclareLocal(typeof(Nullable<T>));
            generator.Emit(OpCodes.Ldloca_S, NullValue);
            generator.Emit(OpCodes.Ldloc_S, stack);
            generator.Emit(OpCodes.Call, typeof(Nullable<T>).GetConstructor(new Type[] { typeof(T) }));
            return NullValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldBoolean HasValue()
        {
            LocalBuilder Has = DeclareLocal(typeof(Boolean));
            generator.Emit(OpCodes.Ldloca_S, instance);
            Emit(OpCodes.Call, HasValueInfo);
            generator.Emit(OpCodes.Stloc_S, Has);
            return new FieldBoolean(Has, generator);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CanCompute<T> Value()
        {
            LocalBuilder Original = generator.DeclareLocal(typeof(T));
            generator.Emit(OpCodes.Ldloca_S, instance);
            Emit(OpCodes.Call, ValueInfo);
            generator.Emit(OpCodes.Stloc_S, Original);
            return new CanCompute<T>(Original, generator);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CanCompute<T> GetValueOrDefault()
        {
            LocalBuilder Default = generator.DeclareLocal(typeof(T));
            generator.Emit(OpCodes.Ldloca_S, instance);
            Emit(OpCodes.Call, GetValueOrDefaultInfo);
            generator.Emit(OpCodes.Stloc_S, Default);
            return new CanCompute<T>(Default, generator);
        }
    }
}
