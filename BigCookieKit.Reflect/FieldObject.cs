﻿using System;
using System.Reflection;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public class FieldObject : FieldManager<object>
    {
        internal Type asidentity { get; set; }

        public Type Type => asidentity;

        internal FieldObject(LocalBuilder stack, ILGenerator generator) : base(stack, generator)
        {
            asidentity = stack.LocalType;
        }

        public FieldObject As<T>()
        {
            LocalBuilder temp = DeclareLocal(typeof(T));
            Output();
            Emit(OpCodes.Castclass, typeof(T));
            Emit(OpCodes.Stloc_S, temp);
            return new FieldObject(temp, this);
        }

        public FieldObject As(Type type)
        {
            LocalBuilder temp = DeclareLocal(type);
            Output();
            Emit(OpCodes.Castclass, type);
            Emit(OpCodes.Stloc_S, temp);
            return new FieldObject(temp, this);
        }

        public FieldBoolean IsNull()
        {
            LocalBuilder assert = DeclareLocal(typeof(bool));
            Output();
            Emit(OpCodes.Ldnull);
            Emit(OpCodes.Ceq);
            Emit(OpCodes.Stloc_S, assert);
            return new FieldBoolean(assert, this);
        }

        public void SetField(string fieldName, LocalBuilder value)
        {
            FieldInfo field = asidentity.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null) ManagerGX.ShowEx<ArgumentNullException>("Field not is exists");
            if (field.FieldType != value.LocalType) ManagerGX.ShowEx<TypeAccessException>("Field type and target type different");
            Output();
            Emit(OpCodes.Ldloc_S, value);
            Emit(OpCodes.Stfld, field);
        }

        public void SetField(string fieldName, object value)
        {
            FieldInfo field = asidentity.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null) ManagerGX.ShowEx<ArgumentNullException>("Field not is exists");
            if (field.FieldType != value.GetType()) ManagerGX.ShowEx<TypeAccessException>("Field type and target type different");
            Output();
            this.EmitValue(value, field.FieldType);
            Emit(OpCodes.Stfld, field);
        }

        public LocalBuilder GetField(string fieldName)
        {
            FieldInfo field = asidentity.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null) ManagerGX.ShowEx<ArgumentNullException>("Field not is exists");
            LocalBuilder local = DeclareLocal(field.FieldType);
            Output();
            Emit(OpCodes.Ldfld, field);
            Emit(OpCodes.Stloc_S, local);
            return local;
        }

        public void SetPropterty(string propName, LocalBuilder value)
        {
            PropertyInfo prop = asidentity.GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (prop == null) ManagerGX.ShowEx<ArgumentNullException>("Propterty not is exists");
            if (prop.PropertyType != value.LocalType) ManagerGX.ShowEx<TypeAccessException>("Propterty type and target type different");
            Output();
            Emit(OpCodes.Ldloc_S, value);
            Emit(OpCodes.Callvirt, prop.GetSetMethod());
        }

        public void SetPropterty(string propName, object value)
        {
            PropertyInfo prop = asidentity.GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (prop == null) ManagerGX.ShowEx<ArgumentNullException>("Propterty not is exists");
            if (prop.PropertyType != value.GetType()) ManagerGX.ShowEx<TypeAccessException>("Propterty type and target type different");
            Output();
            this.EmitValue(value, prop.PropertyType);
            Emit(OpCodes.Callvirt, prop.GetSetMethod());
        }

        public LocalBuilder GetPropterty(string propName)
        {
            PropertyInfo prop = asidentity.GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (prop == null) ManagerGX.ShowEx<ArgumentNullException>("Propterty not is exists");
            LocalBuilder local = DeclareLocal(prop.PropertyType);
            Output();
            Emit(OpCodes.Callvirt, prop.GetGetMethod());
            Emit(OpCodes.Stloc_S, local);
            return local;
        }

        public LocalBuilder GetDynamicPropterty(string propName)
        {
            var type = new FieldObject(Call("GetType").ReturnRef(), generator);
            var prop = new FieldObject(type.Call("GetProperty", ManagerGX.NewString(this, propName)).ReturnRef(), generator);
            return prop.Call("GetValue", this).ReturnRef();
        }

        public FieldObject Operator(string oper, object value)
        {
            var Type = value.GetType();

            if (Type != typeof(byte)
                && Type != typeof(short)
                && Type != typeof(int)
                && Type != typeof(long)
                && Type != typeof(float)
                && Type != typeof(double)
                && Type != typeof(decimal)
                && Type != typeof(DateTime))
            {
                return this.NewObject();
            }

            try
            {
                FieldManager<object> res = null;

                switch (oper)
                {
                    case "+":
                        res = ManagerGX.Compute(this, this.NewObject(value), OpCodes.Add);
                        break;
                    case "-":
                        res = ManagerGX.Compute(this, this.NewObject(value), OpCodes.Sub);
                        break;
                    case "*":
                        res = ManagerGX.Compute(this, this.NewObject(value), OpCodes.Mul);
                        break;
                    case "/":
                        res = ManagerGX.Compute(this, this.NewObject(value), OpCodes.Div);
                        break;

                }

                if (Type != typeof(DateTime) && this.Type != typeof(DateTime))
                {
                    switch (oper)
                    {
                        case "%":
                            res = ManagerGX.Compute(this, this.NewObject(value), OpCodes.Rem);
                            break;
                        case "<<":
                            res = ManagerGX.Compute(this, this.NewObject(value), OpCodes.And, OpCodes.Shl);
                            break;
                        case ">>":
                            res = ManagerGX.Compute(this, this.NewObject(value), OpCodes.And, OpCodes.Shl);
                            break;
                        case "^":
                            res = ManagerGX.Compute(this, this.NewObject(value), OpCodes.Xor);
                            break;
                        case "&":
                            res = ManagerGX.Compute(this, this.NewObject(value), OpCodes.And);
                            break;
                    }
                }

                return this.NewObject(res);
            }
            catch
            {
                return this.NewObject();
            }
        }

        public override MethodManager Call(string methodName, params LocalBuilder[] parameters)
        {
            return this.ReflectMethod(methodName, asidentity, parameters);
        }

        public static FieldBoolean operator ==(FieldObject field, object value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        public static FieldBoolean operator ==(FieldObject field, LocalBuilder value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        public static FieldBoolean operator ==(FieldObject field, VariableManager value)
        {
            return ManagerGX.Comparer(field, value, OpCodes.Ceq);
        }

        public static FieldBoolean operator !=(FieldObject field, object value)
        {
            return ManagerGX.Comparer(
               ManagerGX.Comparer(field, value, OpCodes.Ceq),
               field.NewInt32(), OpCodes.Ceq);
        }

        public static FieldBoolean operator !=(FieldObject field, LocalBuilder value)
        {
            return ManagerGX.Comparer(
                ManagerGX.Comparer(field, value, OpCodes.Ceq),
                field.NewInt32(), OpCodes.Ceq);
        }

        public static FieldBoolean operator !=(FieldObject field, VariableManager value)
        {
            return ManagerGX.Comparer(
               ManagerGX.Comparer(field, value, OpCodes.Ceq),
               field.NewInt32(), OpCodes.Ceq);
        }
    }
}