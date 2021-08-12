using System;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public class FieldInt32 : CanCompute<Int32>
    {
        internal FieldInt32(LocalBuilder stack, ILGenerator generator) : base(stack, generator)
        {
        }
    }
}