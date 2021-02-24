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
        public void For(Int32 init, LocalBuilder length, Action<FieldInt32, TabManager> build)
        {
            ManagerGX.For(this, init, length, build);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void For(LocalBuilder init, LocalBuilder length, Action<FieldInt32, TabManager> build)
        {
            ManagerGX.For(this, init, length, build);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void For(Int32 init, Int32 length, Action<FieldInt32, TabManager> build)
        {
            ManagerGX.For(this, init, length, build);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Forr(Int32 init, LocalBuilder length, Action<FieldInt32, TabManager> build)
        {
            ManagerGX.Forr(this, init, length, build);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Forr(LocalBuilder init, LocalBuilder length, Action<FieldInt32, TabManager> build)
        {
            ManagerGX.Forr(this, init, length, build);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Forr(Int32 init, Int32 length, Action<FieldInt32, TabManager> build)
        {
            ManagerGX.Forr(this, init, length, build);
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
        public void While(Action assert, Action<TabManager> builder)
        {
            var START = DefineLabel();
            var FALSE = DefineLabel();
            var BREAK = DefineLabel();
            MarkLabel(START);
            assert();
            Emit(OpCodes.Brfalse, FALSE);
            builder(new TabManager(this, BREAK));
            Emit(OpCodes.Br, START);
            MarkLabel(FALSE);
            MarkLabel(BREAK);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryCatchManager Try(Action builder)
        {
            BeginExceptionBlock();
            builder();
            return new TryCatchManager(generator);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Return() => Emit(OpCodes.Ret);
    }
}
