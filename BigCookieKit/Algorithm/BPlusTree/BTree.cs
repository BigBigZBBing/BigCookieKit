using System;
using System.Collections.Generic;

namespace BPlusTree
{
    public class BTree<T> : IPromotionListener<T> where T : class, IComparable
    {
        private BTreeNode<T> root;

        public BTree(int order)
        {
            root = new BTreeNode<T>(order, this);
        }

        #region IPromotionListener<T> Members

        public void RootIs(BTreeNode<T> node)
        {
            root = node;
        }

        #endregion

        public void Insert(T value)
        {
            var node = root.Find(value);
            node.Insert(value);
        }

        public override string ToString()
        {
            return root.ToString();
        }

        public BTreeNodeElement<T> Search(T searchKey)
        {
            return root.Search(searchKey);
        }
    }
}