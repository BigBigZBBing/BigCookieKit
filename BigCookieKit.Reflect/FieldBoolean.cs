﻿using System;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public class FieldBoolean : FieldManager<bool>
    {
        internal FieldBoolean(LocalBuilder stack, ILGenerator il) : base(stack, il)
        {
        }

        public static FieldBoolean operator ==(FieldBoolean field, bool value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        public static FieldBoolean operator ==(FieldBoolean field, LocalBuilder value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        public static FieldBoolean operator ==(FieldBoolean field, FieldBoolean value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        public static FieldBoolean operator !=(FieldBoolean field, bool value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }

        public static FieldBoolean operator !=(FieldBoolean field, LocalBuilder value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }

        public static FieldBoolean operator !=(FieldBoolean field, FieldBoolean value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }

        public static FieldBoolean operator |(FieldBoolean field, FieldBoolean value)
        {
            var assert = field.NewBoolean();
            var _true = field.DefineLabel();
            field.Output();
            field.Emit(OpCodes.Ldc_I4_1);
            field.Emit(OpCodes.Beq_S, _true);
            field.Emit(OpCodes.Ldc_I4_0);
            assert.Input();
            value.Output();
            field.Emit(OpCodes.Ldc_I4_1);
            field.Emit(OpCodes.Beq_S, _true);
            field.MarkLabel(_true);
            return assert;
        }

        public static FieldBoolean operator &(FieldBoolean field, FieldBoolean value)
        {
            var assert = field.NewBoolean();
            var _false = field.DefineLabel();
            field.Output();
            field.Emit(OpCodes.Ldc_I4_0);
            field.Emit(OpCodes.Beq_S, _false);
            value.Output();
            field.Emit(OpCodes.Ldc_I4_0);
            field.Emit(OpCodes.Beq_S, _false);
            field.Emit(OpCodes.Ldc_I4_1);
            assert.Input();
            field.MarkLabel(_false);
            return assert;
        }
    }
}