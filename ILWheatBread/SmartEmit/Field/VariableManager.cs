using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace ILWheatBread.SmartEmit.Field
{
    public class VariableManager : EmitBasic
    {
        internal LocalBuilder instance;

        internal VariableManager(LocalBuilder stack, ILGenerator generator) : base(generator)
        {
            this.instance = stack;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Output()
        {
            base.Emit(OpCodes.Ldloc_S, this.instance);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Input()
        {
            base.Emit(OpCodes.Stloc_S, this.instance);
        }
    }
}
