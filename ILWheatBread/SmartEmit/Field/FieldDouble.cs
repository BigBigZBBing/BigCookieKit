using System;
using System.Reflection.Emit;

namespace ILWheatBread.SmartEmit.Field
{
    public class FieldDouble : CanCompute<Double>
    {
        internal FieldDouble(LocalBuilder stack, ILGenerator generator) : base(stack, generator)
        {
        }
    }
}
