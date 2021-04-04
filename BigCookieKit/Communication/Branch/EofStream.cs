using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Communication
{
    public class EofStream
    {
        private Memory<byte> m_bytes;

        private int m_offset;

        private const int defaultcap = 4;

        public int Capacity
        {
            get => m_bytes.Length;
            set
            {
                if (value > m_bytes.Length)
                {
                    if (value > 0)
                    {
                        Memory<byte> newItems = new byte[value];
                        if (m_offset > 0)
                        {
                            m_bytes.CopyTo(newItems);
                        }
                        m_bytes = newItems;
                    }
                }
            }
        }

        public EofStream()
        {
        }

        public EofStream(int capacity = defaultcap) : this()
        {
            m_bytes = new byte[capacity];
        }

        public int Count
        {
            get
            {
                return m_offset;
            }
        }

        public int Position
        {
            get => m_offset;
            set { m_offset = value; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(byte item)
        {
            EnsureCapacity();
            m_bytes.Span[m_offset] = item;
            m_offset++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(Memory<byte> items)
        {
            EnsureCapacity(items.Length);
            items.CopyTo(m_bytes.Slice(m_offset));
            m_offset += items.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(ReadOnlyMemory<byte> items)
        {
            EnsureCapacity(items.Length);
            items.CopyTo(m_bytes.Slice(m_offset));
            m_offset += items.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            m_offset = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ToArray()
        {
            return m_bytes.Slice(0, m_offset).ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCapacity(int increment = 1)
        {
            var max = m_offset + increment;
            if (max >= Capacity)
            {
                if (increment > defaultcap)
                {
                    var addcapacity = Capacity + increment;
                    Capacity = addcapacity;
                }
                else
                {
                    var addcapacity = Capacity + defaultcap;
                    Capacity = addcapacity;
                }
            }
        }
    }
}
