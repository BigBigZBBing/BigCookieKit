using System;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace BigCookieKit.Reflect
{
    public class FieldObject : FieldManager<Object>
    {
        internal Type asidentity { get; set; }

        internal FieldObject(LocalBuilder stack, ILGenerator generator) : base(stack, generator)
        {
            asidentity = stack.LocalType;
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldObject As<T>()
        {
            LocalBuilder temp = DeclareLocal(typeof(T));
            Output();
            Emit(OpCodes.Castclass, typeof(T));
            Emit(OpCodes.Stloc_S, temp);
            return new FieldObject(temp, this);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldObject As(Type type)
        {
            LocalBuilder temp = DeclareLocal(type);
            Output();
            Emit(OpCodes.Castclass, type);
            Emit(OpCodes.Stloc_S, temp);
            return new FieldObject(temp, this);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldBoolean IsNull()
        {
            LocalBuilder assert = DeclareLocal(typeof(Boolean));
            Output();
            Emit(OpCodes.Ldnull);
            Emit(OpCodes.Ceq);
            Emit(OpCodes.Stloc_S, assert);
            return new FieldBoolean(assert, this);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override MethodManager Call(String methodName, params LocalBuilder[] parameters)
        {
            return this.ReflectMethod(methodName, asidentity, parameters);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator ==(FieldObject field, Object value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator ==(FieldObject field, LocalBuilder value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator ==(FieldObject field, VariableManager value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator !=(FieldObject field, Object value)
        {
            return ManagerGX.Comparer(
               ManagerGX.Comparer(field, value, OpCodes.Ceq),
               field.NewInt32(), OpCodes.Ceq);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator !=(FieldObject field, LocalBuilder value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldBoolean operator !=(FieldObject field, VariableManager value)
        {
            return ManagerGX.Comparer(
               ManagerGX.Comparer(field, value, OpCodes.Ceq),
               field.NewInt32(), OpCodes.Ceq);
        }
    }
}
