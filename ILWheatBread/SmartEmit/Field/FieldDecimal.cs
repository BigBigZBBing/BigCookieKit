using System;
using System.Reflection.Emit;

namespace ILWheatBread.SmartEmit.Field
{
    public class FieldDecimal : CanCompute<Decimal>
    {
        internal FieldDecimal(LocalBuilder stack, ILGenerator generator) : base(stack, generator)
        {
        }
    }
}
