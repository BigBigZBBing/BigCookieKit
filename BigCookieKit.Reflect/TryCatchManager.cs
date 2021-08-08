using System;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public class TryCatchManager
    {
        private ILGenerator generator;

        internal TryCatchManager(ILGenerator generator)
        {
            this.generator = generator;
        }

        public TryCatchManager Catch(Action<LocalBuilder> builder)
        {
            generator.BeginCatchBlock(typeof(Exception));
            var ex = generator.DeclareLocal(typeof(Exception));
            generator.Emit(OpCodes.Stloc_S, ex);
            builder(ex);
            return this;
        }

        public TryCatchManager Finally(Action builder)
        {
            generator.BeginFinallyBlock();
            builder();
            return this;
        }

        public void TryEnd()
        {
            generator.EndExceptionBlock();
        }
    }
}