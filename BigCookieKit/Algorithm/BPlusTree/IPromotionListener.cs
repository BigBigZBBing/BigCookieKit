using System;

namespace BPlusTree
{
    public interface IPromotionListener<T> where T : class, IComparable
    {
        void RootIs(BTreeNode<T> node);
    }
}