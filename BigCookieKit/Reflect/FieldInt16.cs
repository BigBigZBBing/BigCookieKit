using System;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public class FieldInt16 : CanCompute<Int16>
    {
        internal FieldInt16(LocalBuilder stack, ILGenerator generator) : base(stack, generator)
        {
        }
    }
}
