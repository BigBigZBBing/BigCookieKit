using FlatBread.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace FlatBread.Buffer
{
    public class BufferPool
    {
        /// <summary>
        /// 缓冲区内所有缓冲位
        /// </summary>
        private ConcurrentStack<Buffer> Buffers { get; set; }

        /// <summary>
        /// 缓冲区总池
        /// </summary>
        private byte[] Pool;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="BufferSize">缓冲位大小</param>
        /// <param name="PoolSize">缓冲区位数</param>
        public BufferPool(int BufferSize, int PoolSize)
        {
            Buffers = new ConcurrentStack<Buffer>();
            Pool = new byte[BufferSize * PoolSize];
            for (int i = 0; i < PoolSize; i++)
            {
                Buffers.Push(Buffer.New(ref Pool, i * BufferSize, BufferSize));
            }
        }

        /// <summary>
        /// 弹出缓冲位
        /// </summary>
        /// <returns></returns>
        public Buffer Pop()
        {
            if (Buffers.TryPop(out var item))
            {
                return item;
            }
            LogHelper.LogError("缓冲区缺失~");
            return null;
        }

        /// <summary>
        /// 推回缓冲位
        /// </summary>
        /// <param name="buffer"></param>
        public void Push(Buffer buffer)
        {
            buffer.Free();
            Buffers.Push(buffer);
        }
    }
}
