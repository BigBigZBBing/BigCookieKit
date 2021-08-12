using System;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public class FieldByte : CanCompute<Byte>
    {
        internal FieldByte(LocalBuilder stack, ILGenerator generator) : base(stack, generator)
        {
        }
    }
}