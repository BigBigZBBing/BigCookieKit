using System;

namespace BPlusTree
{
    public class BTreeNodeElement<T> where T:class, IComparable
    {
        private readonly T value;
        private Guid identifier;

        public override string ToString()
        {
            return value.ToString();
//            return value.ToString() + "[" + identifier + "]";
        }

        public BTreeNodeElement<T> Next 
        { 
          get
          {
              return NextInterval.Larger;
          }
        }
        
        public BTreeNodeElement<T> Previous 
        { 
          get
          {
              return PreviousInterval.Smaller;
          }
        }

        public BTreeNodeElement(T value)
        {
            this.value = value;
            NextInterval = new BTreeNodeElementInterstice<T>(this, null);
            PreviousInterval = new BTreeNodeElementInterstice<T>(null, this);
            identifier = Guid.NewGuid();
        }

        public BTreeNodeElementInterstice<T> PreviousInterval { get; set; }

        public BTreeNodeElementInterstice<T> NextInterval { get; set; }

        private bool Equals(BTreeNodeElement<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.value, value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (BTreeNodeElement<T>)) return false;
            return Equals((BTreeNodeElement<T>) obj);
        }

        public override int GetHashCode()
        {
            return (value != null ? value.GetHashCode() : 0);
        }

        public virtual T Value
        {
            get { return value; }
        }

        public void BreakForwardLink()
        {
//            NextInterval = new BTreeNodeElementInterstice<T>(NextInterval.NodePointer, this, null);
            NextInterval.Larger = null;
        }

        public void TerminateForwardLink()
        {
            NextInterval = new BTreeNodeElementInterstice<T>(this, null);
        }
    }
}