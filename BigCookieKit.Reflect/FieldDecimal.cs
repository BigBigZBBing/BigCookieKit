using System;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public class FieldDecimal : CanCompute<Decimal>
    {
        internal FieldDecimal(LocalBuilder stack, ILGenerator generator) : base(stack, generator)
        {
        }
    }
}