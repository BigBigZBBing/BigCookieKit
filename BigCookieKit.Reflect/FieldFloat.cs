using System;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public class FieldFloat : CanCompute<Single>
    {
        internal FieldFloat(LocalBuilder stack, ILGenerator generator) : base(stack, generator)
        {
        }
    }
}