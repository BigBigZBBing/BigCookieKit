using System;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace BigCookieKit.Reflect
{
    public class FieldDateTime : FieldManager<DateTime>
    {
        internal FieldDateTime(LocalBuilder stack, ILGenerator generator) : base(stack, generator)
        {
        }

        
        
        public static FieldBoolean operator ==(FieldDateTime field, DateTime value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        
        
        public static FieldBoolean operator ==(FieldDateTime field, LocalBuilder value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        
        
        public static FieldBoolean operator ==(FieldDateTime field, FieldDateTime value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        
        
        public static FieldBoolean operator !=(FieldDateTime field, DateTime value)
        {
            return ManagerGX.Comparer(
               ManagerGX.Comparer(field, value, OpCodes.Ceq),
               field.NewInt32(), OpCodes.Ceq);
        }

        
        
        public static FieldBoolean operator !=(FieldDateTime field, LocalBuilder value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }

        
        
        public static FieldBoolean operator !=(FieldDateTime field, FieldDateTime value)
        {
            return ManagerGX.Comparer(
               ManagerGX.Comparer(field, value, OpCodes.Ceq),
               field.NewInt32(), OpCodes.Ceq);
        }
    }
}
