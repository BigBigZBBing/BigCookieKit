using System;
using System.Collections.Generic;
using System.Text;

namespace BPlusTree
{
    public class BTreeNode<T> where T : class, IComparable
    {
        private readonly Guid identifier;
        private readonly int order;
        private readonly IPromotionListener<T> tree;

        private BTreeNodeElementInterstice<T> firstInterstice;

        public BTreeNode(int order, IPromotionListener<T> tree)
        {
            identifier = Guid.NewGuid();
            this.order = order;
            this.tree = tree;
        }

        public BTreeNode<T> Parent { private get; set; }

        private Guid Identifier
        {
            get { return identifier; }
        }

        private BTreeNodeElementInterstice<T> FirstInterstice
        {
            get { return firstInterstice; }
        }

        private BTreeNodeElement<T> FirstElement
        {
            get
            {
                if (firstInterstice == null) return null;
                return firstInterstice.Larger;
            }
        }

        private int Count
        {
            get
            {
                if (firstInterstice == null) return 0;
                int count = 0;
                BTreeNodeElement<T> element = firstInterstice.Larger;
                while (element != null)
                {
                    ++count;
                    element = element.Next;
                }
                return count;
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            BTreeNodeElement<T> start = FirstElement;

            while (start != null)
            {
                builder.Append(start + " - ");
                start = start.Next;
            }
            builder.Append(string.Format("self = {0}, parent = {1}", identifier,
                                         Parent == null ? "NULL" : Parent.Identifier.ToString()));
            builder.AppendLine();
            BTreeNodeElementInterstice<T> startInterstice = FirstInterstice;
            while (startInterstice != null)
            {
                if (!(startInterstice.NodePointer is NullBTreeNode<T>))
                    builder.AppendLine(startInterstice.NodePointer.ToString());
                startInterstice = startInterstice.Next;
            }

            return builder.ToString();
        }

        public void Insert(T value)
        {
            var element = new BTreeNodeElement<T>(value);
            element.PreviousInterval = new BTreeNodeElementInterstice<T>(null, element);
            element.NextInterval = new BTreeNodeElementInterstice<T>(element, null);

            InternalInsert(element);
        }

        private void Insert(BTreeNodeElement<T> element)
        {
            var copiedElement = new BTreeNodeElement<T>(element.Value);
            copiedElement.PreviousInterval = new BTreeNodeElementInterstice<T>(element.PreviousInterval.NodePointer,
                                                                               null, copiedElement);
            copiedElement.NextInterval = new BTreeNodeElementInterstice<T>(element.NextInterval.NodePointer,
                                                                           copiedElement,
                                                                           null);

            copiedElement.PreviousInterval.NodePointer.Parent = copiedElement.NextInterval.NodePointer.Parent = this;
            InternalInsert(copiedElement);
        }

        private void InternalInsert(BTreeNodeElement<T> element)
        {
            InsertInCurrentNode(element);
            if (Count <= order) return;

            int indexOfValueToPromote = (int) Math.Round((double) Count/2) - 1;
            BTreeNodeElement<T> elementToPromote = ElementAt(indexOfValueToPromote);
            Console.Out.WriteLine("{0} will be promoted", elementToPromote.Value);
            BTreeNode<T> node = DissectedNodeFrom(elementToPromote.Next);
            if (! (FirstInterstice.NodePointer is NullBTreeNode<T>))
            {
                elementToPromote.Previous.BreakForwardLink();
            }
            elementToPromote.TerminateForwardLink();

            var promotedReplica = new BTreeNodeElement<T>(elementToPromote.Value);
            Promote(promotedReplica, this, node);
        }

        private void Promote(BTreeNodeElement<T> elementToPromote, BTreeNode<T> smallerNode, BTreeNode<T> largerNode)
        {
            if (Parent == null)
            {
                Parent = new BTreeNode<T>(order, tree);
                tree.RootIs(Parent);
            }

            elementToPromote.NextInterval.Container = elementToPromote.PreviousInterval.Container = Parent;
            elementToPromote.PreviousInterval.NodePointer = smallerNode;
            elementToPromote.NextInterval.NodePointer = largerNode;
            Parent.Insert(elementToPromote);
        }

        private BTreeNode<T> DissectedNodeFrom(BTreeNodeElement<T> element)
        {
            BTreeNodeElement<T> start = element;
            var node = new BTreeNode<T>(order, tree);
            while (start != null)
            {
                node.Insert(start);
                start = start.Next;
            }
            return node;
        }

        private BTreeNodeElement<T> ElementAt(int index)
        {
            BTreeNodeElement<T> start = FirstElement;
            int i = 0;
            while (i < index)
            {
                start = start.NextInterval.Larger;
                ++ i;
            }
            return start;
        }

        private void InsertInCurrentNode(BTreeNodeElement<T> element)
        {
            if (firstInterstice == null)
            {
                element.PreviousInterval.Smaller = null;
                element.NextInterval.Larger = null;
                firstInterstice = element.PreviousInterval;
                element.PreviousInterval.Container = element.NextInterval.Container = this;
                return;
            }

            BTreeNodeElementInterstice<T> insertionInterstice = IntersticeToSearch(element.Value);
            if (insertionInterstice.Smaller != null)
            {
                insertionInterstice.Smaller.NextInterval = element.PreviousInterval;
            }
            if (insertionInterstice.Larger != null)
            {
                insertionInterstice.Larger.PreviousInterval = element.NextInterval;
            }
            element.PreviousInterval.Smaller = insertionInterstice.Smaller;
            element.NextInterval.Larger = insertionInterstice.Larger;

            if (insertionInterstice == firstInterstice) firstInterstice = element.PreviousInterval;
        }

        public BTreeNode<T> Find(T value)
        {
            BTreeNodeElement<T> node = ElementContaining(value);
            if (node != null) return this;
            BTreeNodeElementInterstice<T> interstice = IntersticeToSearch(value);
            if (interstice == null || interstice.NodePointer is NullBTreeNode<T>) return this;
            return interstice.NodePointer.Find(value);
        }
        
        public BTreeNodeElement<T> Search(T value)
        {
            BTreeNodeElement<T> element = ElementContaining(value);
            if (element != null) return element;
            BTreeNodeElementInterstice<T> interstice = IntersticeToSearch(value);
            if (interstice == null || interstice.NodePointer is NullBTreeNode<T>) return new NullBTreeNodeElement<T>();
            return interstice.NodePointer.Search(value);
        }

        private BTreeNodeElement<T> ElementContaining(T value)
        {
            if (firstInterstice == null) return null;
            BTreeNodeElement<T> element = firstInterstice.Larger;
            while (element != null)
            {
                if (element.Value.Equals(value)) return element;
                element = element.Next;
            }
            return null;
        }

        private BTreeNodeElementInterstice<T> IntersticeToSearch(T value)
        {
            BTreeNodeElementInterstice<T> counter = firstInterstice;
            while (counter != null)
            {
                if
                    (counter.Smaller == null && counter.Larger != null && counter.Larger.Value.CompareTo(value) > 0 ||
                     counter.Smaller != null && counter.Larger == null && counter.Smaller.Value.CompareTo(value) < 0 ||
                     counter.Smaller != null && counter.Larger != null && counter.Smaller.Value.CompareTo(value) < 0 &&
                     counter.Larger.Value.CompareTo(value) > 0)
                    return counter;
                counter = counter.Next;
            }
            return null;
        }
    }
}