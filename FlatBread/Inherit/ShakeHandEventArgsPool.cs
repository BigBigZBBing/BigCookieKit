using FlatBread.Buffer;
using FlatBread.Session;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace FlatBread.Inherit
{
    public class ShakeHandEventArgsPool
    {
        /// <summary>
        /// 握手池
        /// </summary>
        private ConcurrentStack<ShakeHandEventArgs> ShakeHandPool { get; set; }

        /// <summary>
        /// 接收缓冲区池
        /// </summary>
        private BufferPool ReceiveBufferPool { get; set; }

        /// <summary>
        /// 发送缓冲区池
        /// </summary>
        private BufferPool SendBufferPool { get; set; }

        /// <summary>
        /// 接收池
        /// </summary>
        /// <param name="BufferSize"></param>
        /// <param name="PoolSize"></param>
        /// <param name="AcceptCompleted"></param>
        /// <param name="IOCompleted"></param>
        public ShakeHandEventArgsPool(int BufferSize, int PoolSize, EventHandler<SocketAsyncEventArgs> DispatchCenter)
        {
            ShakeHandPool = new ConcurrentStack<ShakeHandEventArgs>();
            ReceiveBufferPool = new BufferPool(BufferSize, PoolSize);
            SendBufferPool = new BufferPool(BufferSize, PoolSize);
            for (int i = 0; i < PoolSize; i++)
            {
                UserTokenSession userToken = new UserTokenSession();
                ReceiveEventArgs receiveEventArgs = new ReceiveEventArgs();
                receiveEventArgs.UserToken = userToken;
                receiveEventArgs.Completed += DispatchCenter;
                receiveEventArgs.SetBuffer(ReceiveBufferPool.Pop());

                SendEventArgs senEventArgs = new SendEventArgs();
                senEventArgs.UserToken = userToken;
                senEventArgs.Completed += DispatchCenter;
                senEventArgs.SetBuffer(SendBufferPool.Pop());

                ShakeHandEventArgs acceptEventArgs = new ShakeHandEventArgs(receiveEventArgs, senEventArgs);
                acceptEventArgs.Completed += DispatchCenter;
                acceptEventArgs.UserToken = userToken;

                ShakeHandPool.Push(acceptEventArgs);
            }
        }

        /// <summary>
        /// 推出池
        /// </summary>
        /// <returns></returns>
        public ShakeHandEventArgs Pop()
        {
            if (ShakeHandPool.TryPop(out var item))
            {
                return item;
            }
            return null;
        }

        /// <summary>
        /// 推入池
        /// </summary>
        /// <param name="item"></param>
        public void Push(ShakeHandEventArgs item)
        {
            ShakeHandPool.Push(item);
        }
    }
}
