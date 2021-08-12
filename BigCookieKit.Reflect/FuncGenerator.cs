﻿using System;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public partial class FuncGenerator : EmitBasic
    {
        internal FuncGenerator(ILGenerator generator) : base(generator)
        {
            this.generator = generator;
        }

        public void For(Int32 init, LocalBuilder length, Action<CanCompute<Int32>, TabManager> build)
        {
            if (length.LocalType != typeof(Int32)) ManagerGX.ShowEx<TypeAccessException>("Type not is [Int32]");
            ManagerGX.For(this, init, length, build);
        }

        public void For(LocalBuilder init, LocalBuilder length, Action<CanCompute<Int32>, TabManager> build)
        {
            if (init.LocalType != typeof(Int32)) ManagerGX.ShowEx<TypeAccessException>("Type not is [Int32]");
            if (length.LocalType != typeof(Int32)) ManagerGX.ShowEx<TypeAccessException>("Type not is [Int32]");
            ManagerGX.For(this, init, length, build);
        }

        public void For(Int32 init, Int32 length, Action<CanCompute<Int32>, TabManager> build)
        {
            ManagerGX.For(this, init, length, build);
        }

        public void Forr(Int32 init, LocalBuilder length, Action<CanCompute<Int32>, TabManager> build)
        {
            if (length.LocalType != typeof(Int32)) ManagerGX.ShowEx<TypeAccessException>("Type not is [Int32]");
            ManagerGX.Forr(this, init, length, build);
        }

        public void Forr(LocalBuilder init, LocalBuilder length, Action<CanCompute<Int32>, TabManager> build)
        {
            if (init.LocalType != typeof(Int32)) ManagerGX.ShowEx<TypeAccessException>("Type not is [Int32]");
            if (length.LocalType != typeof(Int32)) ManagerGX.ShowEx<TypeAccessException>("Type not is [Int32]");
            ManagerGX.Forr(this, init, length, build);
        }

        public void Forr(Int32 init, Int32 length, Action<CanCompute<Int32>, TabManager> build)
        {
            ManagerGX.Forr(this, init, length, build);
        }

        public void Foreach(LocalBuilder init, Action<LocalBuilder> build)
        {
        }

        public AssertManager IF(LocalBuilder assert, Action builder)
        {
            if (assert.LocalType != typeof(Boolean)) ManagerGX.ShowEx<TypeAccessException>("Type not is [Boolean]");
            return new AssertManager(generator, new Tuple<LocalBuilder, Action>(assert, builder));
        }

        public AssertManager IF(FieldManager<Boolean> assert, Action builder)
        {
            return new AssertManager(generator, new Tuple<LocalBuilder, Action>(assert, builder));
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