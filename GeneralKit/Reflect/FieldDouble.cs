using System;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public class FieldDouble : CanCompute<Double>
    {
        internal FieldDouble(LocalBuilder stack, ILGenerator generator) : base(stack, generator)
        {
        }
    }
}
