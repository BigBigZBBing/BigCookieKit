using System;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace ILWheatBread.SmartEmit.Func
{
    public class MethodManager : EmitBasic
    {
        internal Type ReturnType { get; set; }

        public MethodManager(ILGenerator generator, Type returnType) : base(generator)
        {
            ReturnType = returnType;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LocalBuilder ReturnRef()
        {
            CacheManager.retValue = false;
            LocalBuilder ret = DeclareLocal(ReturnType);
            Emit(OpCodes.Stloc_S, ret);
            return ret;
        }
    }
}
