﻿using System;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public class FieldInt64 : CanCompute<Int64>
    {
        internal FieldInt64(LocalBuilder stack, ILGenerator generator) : base(stack, generator)
        {
        }
    }
}