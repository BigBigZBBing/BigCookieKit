using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public class AssertManager
    {
        private FuncGenerator generator;
        private List<Tuple<LocalBuilder, Action>> context = new List<Tuple<LocalBuilder, Action>>();

        internal AssertManager(FuncGenerator generator, Tuple<LocalBuilder, Action> context)
        {
            this.generator = generator;
            this.context.Add(context);
        }

        public AssertManager ElseIF(LocalBuilder assert, Action builder)
        {
            context.Add(new Tuple<LocalBuilder, Action>(assert, builder));
            return this;
        }

        public AssertManager ElseIF(FieldManager<bool> assert, Action builder)
        {
            context.Add(new Tuple<LocalBuilder, Action>(assert, builder));
            return this;
        }

        public void Else(Action<ILGenerator> builder)
        {
            Label end = generator.DefineLabel();
            Label lab = generator.DefineLabel();
            bool first = true;
            foreach (var item in context)
            {
                if (!first) generator.MarkLabel(lab);
                lab = generator.DefineLabel();
                generator.Emit(OpCodes.Ldloc_S, item.Item1);
                generator.Emit(OpCodes.Brfalse, lab);
                item.Item2?.Invoke();
                generator.Emit(OpCodes.Br, end);
                first = false;
            }
            generator.MarkLabel(lab);
            builder?.Invoke(generator);
            generator.Emit(OpCodes.Br, end);
            generator.MarkLabel(end);
        }

        public void IFEnd()
        {
            Label end = generator.DefineLabel();
            Label lab = generator.DefineLabel();
            bool first = true;
            foreach (var item in context)
            {
                if (!first) generator.MarkLabel(lab);
                lab = generator.DefineLabel();
                generator.Emit(OpCodes.Ldloc_S, item.Item1);
                generator.Emit(OpCodes.Brfalse, lab);
                item.Item2?.Invoke();
                generator.Emit(OpCodes.Br, end);
                first = false;
            }
            generator.MarkLabel(lab);
            generator.MarkLabel(end);
        }
    }
}