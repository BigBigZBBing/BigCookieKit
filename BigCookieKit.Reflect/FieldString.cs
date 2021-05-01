using System;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace BigCookieKit.Reflect
{
    public class FieldString : FieldManager<String>
    {
        internal FieldString(LocalBuilder stack, ILGenerator generator) : base(stack, generator)
        {
        }

        
        
        public FieldBoolean IsNull()
        {
            return this.IsNull(this);
        }

        
        
        public FieldBoolean IsNullOrEmpty()
        {
            Output();
            return new FieldBoolean(this.ReflectStaticMethod("IsNullOrEmpty", typeof(String)).ReturnRef(), this);
        }

        
        
        public static FieldBoolean operator ==(FieldString field, String value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        
        
        public static FieldBoolean operator ==(FieldString field, LocalBuilder value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        
        
        public static FieldBoolean operator ==(FieldString field, FieldString value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        
        
        public static FieldBoolean operator !=(FieldString field, String value)
        {
            return ManagerGX.Comparer(
               ManagerGX.Comparer(field, value, OpCodes.Ceq),
               field.NewInt32(), OpCodes.Ceq);
        }

        
        
        public static FieldBoolean operator !=(FieldString field, LocalBuilder value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }

        
        
        public static FieldBoolean operator !=(FieldString field, FieldString value)
        {
            return ManagerGX.Comparer(
               ManagerGX.Comparer(field, value, OpCodes.Ceq),
               field.NewInt32(), OpCodes.Ceq);
        }

        
        
        public static FieldString operator +(FieldString field, String value)
        {
            return ManagerGX.Compute(field, value, OpCodes.Add);
        }

        
        
        public static VariableManager operator +(FieldString field, LocalBuilder value)
        {
            return ManagerGX.Compute(field, value, OpCodes.Add);
        }

        
        
        public static FieldString operator +(FieldString field, FieldString value)
        {
            return ManagerGX.Compute(field, value, OpCodes.Add);
        }
    }
}
