using System;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public partial class FuncGenerator : EmitBasic
    {
        internal FuncGenerator(ILGenerator generator) : base(generator)
        {
            this.generator = generator;
        }

        public void For(int init, LocalBuilder length, Action<CanCompute<int>, TabManager> build)
        {
            if (length.LocalType != typeof(int)) ManagerGX.ShowEx<TypeAccessException>("Type not is [int]");
            ManagerGX.For(this, init, length, build);
        }

        public void For(LocalBuilder init, LocalBuilder length, Action<CanCompute<int>, TabManager> build)
        {
            if (init.LocalType != typeof(int)) ManagerGX.ShowEx<TypeAccessException>("Type not is [int]");
            if (length.LocalType != typeof(int)) ManagerGX.ShowEx<TypeAccessException>("Type not is [int]");
            ManagerGX.For(this, init, length, build);
        }

        public void For(int init, int length, Action<CanCompute<int>, TabManager> build)
        {
            ManagerGX.For(this, init, length, build);
        }

        public void Forr(int init, LocalBuilder length, Action<CanCompute<int>, TabManager> build)
        {
            if (length.LocalType != typeof(int)) ManagerGX.ShowEx<TypeAccessException>("Type not is [int]");
            ManagerGX.Forr(this, init, length, build);
        }

        public void Forr(LocalBuilder init, LocalBuilder length, Action<CanCompute<int>, TabManager> build)
        {
            if (init.LocalType != typeof(int)) ManagerGX.ShowEx<TypeAccessException>("Type not is [int]");
            if (length.LocalType != typeof(int)) ManagerGX.ShowEx<TypeAccessException>("Type not is [int]");
            ManagerGX.Forr(this, init, length, build);
        }

        public void Forr(int init, int length, Action<CanCompute<int>, TabManager> build)
        {
            ManagerGX.Forr(this, init, length, build);
        }

        public void Foreach(LocalBuilder init, Action<LocalBuilder> build)
        {
        }

        public AssertManager IF(LocalBuilder assert, Action builder)
        {
            if (assert.LocalType != typeof(bool)) ManagerGX.ShowEx<TypeAccessException>("Type not is [bool]");
            return new AssertManager(this, new Tuple<LocalBuilder, Action>(assert, builder));
        }

        public AssertManager IF(FieldManager<bool> assert, Action builder)
        {
            return new AssertManager(this, new Tuple<LocalBuilder, Action>(assert, builder));
        }

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

        public TryCatchManager Try(Action builder)
        {
            BeginExceptionBlock();
            builder();
            return new TryCatchManager(generator);
        }

        public void Return() => Emit(OpCodes.Ret);
    }
}