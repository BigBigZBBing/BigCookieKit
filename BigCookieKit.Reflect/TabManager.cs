using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

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
