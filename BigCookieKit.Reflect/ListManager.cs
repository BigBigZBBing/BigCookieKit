using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public class FieldList<T> : FieldManager<List<T>>
    {
        public FieldList(LocalBuilder stack, ILGenerator generator) : base(stack, generator)
        {
        }

        public void Add(LocalBuilder value)
        {
            Call("Add", value);
        }

        public void Contains(LocalBuilder value)
        {
            Call("Contains", value);
        }

        public void RemoveAt(LocalBuilder value)
        {
            Call("RemoveAt", value);
        }

        public CanCompute<Int32> GetCount()
        {
            return new CanCompute<Int32>(Call("get_Count").ReturnRef(), generator);
        }
    }
}