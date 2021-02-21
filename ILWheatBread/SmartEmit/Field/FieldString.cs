using System;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace ILWheatBread.SmartEmit.Field
{
    public class FieldString : FieldManager<String>
    {
        internal FieldString(LocalBuilder stack, ILGenerator generator) : base(stack, generator)
        {
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldBoolean IsNull()
        {
            return this.IsNull(this);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldBoolean IsNullOrEmpty()
        {
            Output();
            return new FieldBoolean(this.ReflectStaticMethod("IsNullOrEmpty", typeof(String)).ReturnRef(), this);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator ==(FieldString field, String value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator ==(FieldString field, LocalBuilder value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator ==(FieldString field, FieldString value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator !=(FieldString field, String value)
        {
            return ManagerGX.Comparer(
               ManagerGX.Comparer(field, value, OpCodes.Ceq),
               field.NewInt32(), OpCodes.Ceq);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator !=(FieldString field, LocalBuilder value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator !=(FieldString field, FieldString value)
        {
            return ManagerGX.Comparer(
               ManagerGX.Comparer(field, value, OpCodes.Ceq),
               field.NewInt32(), OpCodes.Ceq);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldString operator +(FieldString field, String value)
        {
            return ManagerGX.Compute(field, value, OpCodes.Add);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VariableManager operator +(FieldString field, LocalBuilder value)
        {
            return ManagerGX.Compute(field, value, OpCodes.Add);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldString operator +(FieldString field, FieldString value)
        {
            return ManagerGX.Compute(field, value, OpCodes.Add);
        }
    }
}
