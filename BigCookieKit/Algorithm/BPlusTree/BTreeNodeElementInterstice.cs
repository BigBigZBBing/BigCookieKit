using System;

namespace BPlusTree
{
    public class BTreeNodeElementInterstice<T> where T : class, IComparable
    {
        private BTreeNode<T> container;
        private BTreeNode<T> nodePointer;

        public BTreeNodeElementInterstice(BTreeNode<T> pointer, BTreeNodeElement<T> smaller, BTreeNodeElement<T> larger)
        {
            Smaller = smaller;
            Larger = larger;
            NodePointer = pointer;
        }

        public BTreeNodeElementInterstice(BTreeNodeElement<T> smaller, BTreeNodeElement<T> larger)
            : this(new NullBTreeNode<T>(), smaller, larger)
        {
        }

        public BTreeNode<T> NodePointer
        {
            get { return nodePointer; }
            set
            {
                nodePointer = value;
                if (nodePointer == null) return;
                nodePointer.Parent = container;
            }
        }

        public BTreeNodeElement<T> Smaller { get; set; }

        public BTreeNodeElement<T> Larger { get; set; }

        public BTreeNodeElementInterstice<T> Next
        {
            get
            {
                if (Larger == null) return null;
                return Larger.NextInterval;
            }
        }

        public BTreeNode<T> Container
        {
            set
            {
                container = value;
                if (nodePointer == null) return;
                nodePointer.Parent = container;
            }
        }
    }

    public class NullBTreeNode<T> : BTreeNode<T> where T : class, IComparable
    {
        public NullBTreeNode() : base(0, null)
        {
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}