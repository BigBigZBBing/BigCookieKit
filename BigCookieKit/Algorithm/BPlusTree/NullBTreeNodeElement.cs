using System;

namespace BPlusTree
{
    public class NullBTreeNodeElement<T> : BTreeNodeElement<T> where T : class, IComparable
    {
        public NullBTreeNodeElement() : base(null)
        {
        }
    }
}