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

        public FieldBoolean Contains(LocalBuilder value)
        {
            return new FieldBoolean(Call("Contains", value).ReturnRef(), generator);
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

    public class FieldList : FieldManager
    {
        public FieldList(LocalBuilder stack, ILGenerator generator) : base(stack, generator)
        {
        }

        public void Add(LocalBuilder value)
        {
            Call("Add", value);
        }

        public FieldBoolean Contains(LocalBuilder value)
        {
            return new FieldBoolean(Call("Contains", value).ReturnRef(), generator);
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