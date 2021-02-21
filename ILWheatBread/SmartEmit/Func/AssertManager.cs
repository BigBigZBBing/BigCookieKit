using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace ILWheatBread.SmartEmit
{
    public class AssertManager
    {
        private ILGenerator generator;
        private List<(LocalBuilder, Action)> context = new List<(LocalBuilder, Action)>();

        internal AssertManager(ILGenerator generator, (LocalBuilder, Action) context)
        {
            this.generator = generator;
            this.context.Add(context);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AssertManager ElseIF(LocalBuilder assert, Action builder)
        {
            context.Add((assert, builder));
            return this;
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AssertManager ElseIF<T>(FieldManager<T> assert, Action builder)
        {
            context.Add((assert, builder));
            return this;
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Else(Action<ILGenerator> builder)
        {
            Label end = generator.DefineLabel();
            Label lab = generator.DefineLabel();
            Boolean first = true;
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

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void IFEnd()
        {
            Label end = generator.DefineLabel();
            Label lab = generator.DefineLabel();
            Boolean first = true;
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
