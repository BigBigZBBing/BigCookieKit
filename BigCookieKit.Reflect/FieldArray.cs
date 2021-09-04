using System;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public class FieldArray<T> : FieldManager<T[]>
    {
        internal Int32 Length { get; set; }
        internal CanCompute<Int32> ILLength { get; set; }

        internal FieldArray(LocalBuilder stack, ILGenerator generator, Int32 Length) : base(stack, generator)
        {
            this.Length = Length;
        }

        public FieldBoolean IsNull()
        {
            return this.IsNull(this);
        }

        public LocalBuilder this[Int32 index]
        {
            get
            {
                var value = DeclareLocal(typeof(T));
                Emit(OpCodes.Ldloc_S, instance);
                this.EmitValue(index);
                this.PopArray(identity);
                Emit(OpCodes.Stloc_S, value);
                return value;
            }

            set
            {
                Emit(OpCodes.Ldloc_S, instance);
                this.EmitValue(index);
                Emit(OpCodes.Ldloc_S, value);
                this.PushArray(identity);
            }
        }

        public LocalBuilder GetValue(CanCompute<Int32> index)
        {
            var value = DeclareLocal(typeof(T));
            Emit(OpCodes.Ldloc_S, instance);
            Emit(OpCodes.Ldloc_S, index);
            this.PopArray(identity);
            Emit(OpCodes.Stloc_S, value);
            return value;
        }

        public void SetValue(CanCompute<Int32> index, LocalBuilder value)
        {
            Emit(OpCodes.Ldloc_S, instance);
            Emit(OpCodes.Ldloc_S, index);
            Emit(OpCodes.Ldloc_S, value);
            this.PushArray(identity);
        }

        public FieldBoolean Exists(LocalBuilder value)
        {
            var result = this.NewBoolean();
            Label trueTo = DefineLabel();
            Label falseTo = DefineLabel();
            this.For(0, GetLength(), (build, tab) =>
            {
                GetValue(build);
                Emit(OpCodes.Ldloc_S, value);
                Emit(OpCodes.Ceq);
                Emit(OpCodes.Brfalse, falseTo);
                Emit(OpCodes.Ldc_I4_1);
                result.Input();
                Emit(OpCodes.Br, trueTo);
                MarkLabel(falseTo);
            });
            MarkLabel(trueTo);
            return result;
        }

        public CanCompute<Int32> FindIndex(LocalBuilder value)
        {
            var result = this.NewInt32(-1);
            Label trueTo = DefineLabel();
            Label falseTo = DefineLabel();
            this.For(0, GetLength(), (build, tab) =>
            {
                GetValue(build);
                Emit(OpCodes.Ldloc_S, value);
                Emit(OpCodes.Ceq);
                Emit(OpCodes.Brfalse, falseTo);
                build.Output();
                result.Input();
                Emit(OpCodes.Br, trueTo);
                MarkLabel(falseTo);
            });
            MarkLabel(trueTo);
            return result;
        }

        public void Copy(FieldArray<T> target, CanCompute<Int32> length)
        {
            this.For(0, length, (int1, tab) =>
            {
                var local = DeclareLocal(target.identity);
                GetValue(int1);
                Emit(OpCodes.Stloc_S, local);
                target.SetValue(int1, local);
            });
        }

        public CanCompute<Int32> GetLength()
        {
            if ((object)ILLength == null)
            {
                if (Length == -1)
                {
                    LocalBuilder temp = DeclareLocal(typeof(Int32));
                    Output();
                    Emit(OpCodes.Ldlen);
                    Emit(OpCodes.Stloc_S, temp);
                    ILLength = new CanCompute<Int32>(temp, generator);
                }
                else
                    ILLength = this.NewInt32(Length);
            }
            return ILLength;
        }
    }

    public class FieldArray : FieldManager
    {
        internal Int32 Length { get; set; }
        internal CanCompute<Int32> ILLength { get; set; }

        internal FieldArray(LocalBuilder stack, ILGenerator generator, Int32 Length) : base(stack, generator)
        {
            this.Length = Length;
        }

        public FieldBoolean IsNull()
        {
            return this.IsNull(this);
        }

        public LocalBuilder this[Int32 index]
        {
            get
            {
                var value = DeclareLocal(instance.LocalType);
                Emit(OpCodes.Ldloc_S, instance);
                this.EmitValue(index);
                this.PopArray(identity);
                Emit(OpCodes.Stloc_S, value);
                return value;
            }

            set
            {
                Emit(OpCodes.Ldloc_S, instance);
                this.EmitValue(index);
                Emit(OpCodes.Ldloc_S, value);
                this.PushArray(identity);
            }
        }

        public LocalBuilder GetValue(CanCompute<Int32> index)
        {
            var value = DeclareLocal(instance.LocalType);
            Emit(OpCodes.Ldloc_S, instance);
            Emit(OpCodes.Ldloc_S, index);
            this.PopArray(identity);
            Emit(OpCodes.Stloc_S, value);
            return value;
        }

        public void SetValue(CanCompute<Int32> index, LocalBuilder value)
        {
            Emit(OpCodes.Ldloc_S, instance);
            Emit(OpCodes.Ldloc_S, index);
            Emit(OpCodes.Ldloc_S, value);
            this.PushArray(identity);
        }

        public FieldBoolean Exists(LocalBuilder value)
        {
            var result = this.NewBoolean();
            Label trueTo = DefineLabel();
            Label falseTo = DefineLabel();
            this.For(0, GetLength(), (build, tab) =>
            {
                GetValue(build);
                Emit(OpCodes.Ldloc_S, value);
                Emit(OpCodes.Ceq);
                Emit(OpCodes.Brfalse, falseTo);
                Emit(OpCodes.Ldc_I4_1);
                result.Input();
                Emit(OpCodes.Br, trueTo);
                MarkLabel(falseTo);
            });
            MarkLabel(trueTo);
            return result;
        }

        public CanCompute<Int32> FindIndex(LocalBuilder value)
        {
            var result = this.NewInt32(-1);
            Label trueTo = DefineLabel();
            Label falseTo = DefineLabel();
            this.For(0, GetLength(), (build, tab) =>
            {
                GetValue(build);
                Emit(OpCodes.Ldloc_S, value);
                Emit(OpCodes.Ceq);
                Emit(OpCodes.Brfalse, falseTo);
                build.Output();
                result.Input();
                Emit(OpCodes.Br, trueTo);
                MarkLabel(falseTo);
            });
            MarkLabel(trueTo);
            return result;
        }

        public void Copy(LocalBuilder target, CanCompute<Int32> length)
        {
            Call("CopyTo", target, length);
        }

        public CanCompute<Int32> GetLength()
        {
            if ((object)ILLength == null)
            {
                if (Length == -1)
                {
                    LocalBuilder temp = DeclareLocal(typeof(Int32));
                    Output();
                    Emit(OpCodes.Ldlen);
                    Emit(OpCodes.Stloc_S, temp);
                    ILLength = new CanCompute<Int32>(temp, generator);
                }
                else
                    ILLength = this.NewInt32(Length);
            }
            return ILLength;
        }
    }
}