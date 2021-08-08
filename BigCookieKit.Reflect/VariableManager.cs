using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public class VariableManager : EmitBasic
    {
        internal LocalBuilder instance;

        internal VariableManager(LocalBuilder stack, ILGenerator generator) : base(generator)
        {
            this.instance = stack;
        }

        public void Output()
        {
            base.Emit(OpCodes.Ldloc_S, this.instance);
        }

        public void Input()
        {
            base.Emit(OpCodes.Stloc_S, this.instance);
        }
    }
}