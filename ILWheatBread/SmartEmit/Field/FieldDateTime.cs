using System;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace ILWheatBread.SmartEmit.Field
{
    public class FieldDateTime : FieldManager<DateTime>
    {
        internal FieldDateTime(LocalBuilder stack, ILGenerator generator) : base(stack, generator)
        {
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator ==(FieldDateTime field, DateTime value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator ==(FieldDateTime field, LocalBuilder value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator ==(FieldDateTime field, FieldDateTime value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator !=(FieldDateTime field, DateTime value)
        {
            return ManagerGX.Comparer(
               ManagerGX.Comparer(field, value, OpCodes.Ceq),
               field.NewInt32(), OpCodes.Ceq);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator !=(FieldDateTime field, LocalBuilder value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator !=(FieldDateTime field, FieldDateTime value)
        {
            return ManagerGX.Comparer(
               ManagerGX.Comparer(field, value, OpCodes.Ceq),
               field.NewInt32(), OpCodes.Ceq);
        }
    }
}
