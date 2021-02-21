using System;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace ILWheatBread.SmartEmit.Func
{
    public class TryCatchManager
    {
        private ILGenerator generator;

        internal TryCatchManager(ILGenerator generator)
        {
            this.generator = generator;
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryCatchManager Catch(Action<LocalBuilder> builder)
        {
            generator.BeginCatchBlock(typeof(Exception));
            var ex = generator.DeclareLocal(typeof(Exception));
            generator.Emit(OpCodes.Stloc_S, ex);
            builder(ex);
            return this;
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryCatchManager Finally(Action builder)
        {
            generator.BeginFinallyBlock();
            builder();
            return this;
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TryEnd()
        {
            generator.EndExceptionBlock();
        }
    }
}
