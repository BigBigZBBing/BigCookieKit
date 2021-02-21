using System;
using System.Reflection.Emit;

namespace ILWheatBread.SmartEmit.Field
{
    public class FieldFloat : CanCompute<Single>
    {
        internal FieldFloat(LocalBuilder stack, ILGenerator generator) : base(stack, generator)
        {
        }
    }
}
