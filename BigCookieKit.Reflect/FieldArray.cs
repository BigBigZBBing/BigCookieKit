using System;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public class FieldArray<T> : FieldManager<T[]>
    {
        internal int Length { get; set; }
        internal CanCompute<int> ILLength { get; set; }

        internal FieldArray(LocalBuilder stack, ILGenerator generator, int Length) : base(stack, generator)
        {
            this.Length = Length;
        }

        public FieldBoolean IsNull()
        {
            return this.IsNull(this);
        }

        public LocalBuilder this[int index]
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

        public LocalBuilder GetValue(CanCompute<int> index)
        {
            var value = DeclareLocal(typeof(T));
            Emit(OpCodes.Ldloc_S, instance);
            Emit(OpCodes.Ldloc_S, index);
            this.PopArray(identity);
            Emit(OpCodes.Stloc_S, value);
            return value;
        }

        public void SetValue(CanCompute<int> index, LocalBuilder value)
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

        public CanCompute<int> FindIndex(LocalBuilder value)
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

        public void Copy(FieldArray<T> target, CanCompute<int> length)
        {
            this.For(0, length, (int1, tab) =>
            {
                var local = DeclareLocal(target.identity);
                GetValue(int1);
                Emit(OpCodes.Stloc_S, local);
                target.SetValue(int1, local);
            });
        }

        public CanCompute<int> GetLength()
        {
            if ((object)ILLength == null)
            {
                if (Length == -1)
                {
                    LocalBuilder temp = DeclareLocal(typeof(int));
                    Output();
                    Emit(OpCodes.Ldlen);
                    Emit(OpCodes.Stloc_S, temp);
                    ILLength = new CanCompute<int>(temp, generator);
                }
                else
                    ILLength = this.NewInt32(Length);
            }
            return ILLength;
        }
    }

    public class FieldArray : FieldManager
    {
        internal int Length { get; set; }
        internal CanCompute<int> ILLength { get; set; }

        internal FieldArray(LocalBuilder stack, ILGenerator generator, int Length) : base(stack, generator)
        {
            this.Length = Length;
        }

        public FieldBoolean IsNull()
        {
            return this.IsNull(this);
        }

        public LocalBuilder this[int index]
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

        public LocalBuilder GetValue(CanCompute<int> index)
        {
            var value = DeclareLocal(instance.LocalType);
            Emit(OpCodes.Ldloc_S, instance);
            Emit(OpCodes.Ldloc_S, index);
            this.PopArray(identity);
            Emit(OpCodes.Stloc_S, value);
            return value;
        }

        public void SetValue(CanCompute<int> index, LocalBuilder value)
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

        public CanCompute<int> FindIndex(LocalBuilder value)
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

        public void Copy(LocalBuilder target, CanCompute<int> length)
        {
            Call("CopyTo", target, length);
        }

        public CanCompute<int> GetLength()
        {
            if ((object)ILLength == null)
            {
                if (Length == -1)
                {
                    LocalBuilder temp = DeclareLocal(typeof(int));
                    Output();
                    Emit(OpCodes.Ldlen);
                    Emit(OpCodes.Stloc_S, temp);
                    ILLength = new CanCompute<int>(temp, generator);
                }
                else
                    ILLength = this.NewInt32(Length);
            }
            return ILLength;
        }
    }
}