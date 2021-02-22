using System;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace BigCookieKit.Reflect
{
    public partial class FuncGenerator : EmitBasic
    {
        internal FuncGenerator(ILGenerator generator) : base(generator)
        {
            this.generator = generator;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void For(Int32 init, LocalBuilder length, Action<FieldInt32> build)
        {
            ManagerGX.For(this, init, length, build);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void For(LocalBuilder init, LocalBuilder length, Action<FieldInt32> build)
        {
            ManagerGX.For(this, init, length, build);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void For(Int32 init, Int32 length, Action<FieldInt32> build)
        {
            ManagerGX.For(this, init, length, build);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Forr(LocalBuilder init, Action<FieldInt32> builder)
        {
            Label _for = DefineLabel();
            Label _endfor = DefineLabel();
            LocalBuilder index = DeclareLocal(typeof(Int32));
            Emit(OpCodes.Ldloc_S, init);
            Emit(OpCodes.Ldlen);
            Emit(OpCodes.Ldc_I4_1);
            Emit(OpCodes.Sub);
            Emit(OpCodes.Stloc_S, index);
            Emit(OpCodes.Br, _endfor);
            MarkLabel(_for);
            builder?.Invoke(new FieldInt32(index, generator));
            Emit(OpCodes.Ldloc_S, index);
            Emit(OpCodes.Ldc_I4_1);
            Emit(OpCodes.Sub);
            Emit(OpCodes.Stloc_S, index);
            MarkLabel(_endfor);
            Emit(OpCodes.Ldloc_S, index);
            Emit(OpCodes.Ldc_I4_0);
            Emit(OpCodes.Clt);
            Emit(OpCodes.Ldc_I4_0);
            Emit(OpCodes.Ceq);
            Emit(OpCodes.Brtrue, _for);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AssertManager IF(LocalBuilder assert, Action builder)
        {
            return new AssertManager(generator, new Tuple<LocalBuilder, Action>(assert, builder));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AssertManager IF<T>(FieldManager<T> assert, Action builder)
        {
            return new AssertManager(generator, new Tuple<LocalBuilder, Action>(assert, builder));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void While(Action assert, Action builder)
        {
            var START = DefineLabel();
            var FALSE = DefineLabel();
            MarkLabel(START);
            assert();
            Emit(OpCodes.Brfalse, FALSE);
            builder();
            Emit(OpCodes.Br, START);
            MarkLabel(FALSE);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryCatchManager Try(Action builder)
        {
            BeginExceptionBlock();
            builder();
            return new TryCatchManager(generator);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Return()
        {
            Emit(OpCodes.Ret);
        }
    }
}
