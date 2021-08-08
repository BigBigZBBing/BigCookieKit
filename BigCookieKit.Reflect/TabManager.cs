using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public class TabManager : EmitBasic
    {
        private Label pointer { get; set; }

        internal TabManager(ILGenerator generator, Label pointer) : base(generator)
        {
            this.pointer = pointer;
        }

        public void Break()
        {
            Emit(OpCodes.Br, pointer);
        }
    }
}