using System;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public class MethodManager : EmitBasic
    {
        internal Type ReturnType { get; set; }

        public MethodManager(ILGenerator generator, Type returnType) : base(generator)
        {
            ReturnType = returnType;
        }

        public LocalBuilder ReturnRef()
        {
            tiggerPop = false;
            LocalBuilder ret = DeclareLocal(ReturnType);
            Emit(OpCodes.Stloc_S, ret);
            return ret;
        }
    }
}