using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace BigCookieKit.Reflect
{
    public class FieldList<T> : FieldManager<List<T>>
    {
        
        public FieldList(LocalBuilder stack, ILGenerator generator) : base(stack, generator)
        {
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(LocalBuilder value)
        {
            Call("Add", value);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Contains(LocalBuilder value)
        {
            Call("Contains", value);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAt(LocalBuilder value)
        {
            Call("RemoveAt", value);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldInt32 GetCount()
        {
            return new FieldInt32(Call("get_Count").ReturnRef(), generator);
        }
    }
}
