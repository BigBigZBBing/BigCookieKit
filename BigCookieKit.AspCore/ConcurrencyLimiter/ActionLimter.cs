
using System;
using System.Threading;

namespace BigCookieKit.AspCore.ConcurrencyLimiter
{
    public class ActionLimter
    {
        public ActionLimter(int maxCount, int maxWait)
        {
            Semaphore = new SemaphoreSlim(maxCount, maxCount);
            this.maxWait = maxWait;
        }

        /// <summary>
        /// 信号灯
        /// </summary>
        public SemaphoreSlim Semaphore { get; set; }

        private int waitCount = 0;
        private readonly int maxWait;
        private readonly object _lock = new object();

        public bool Doing(Action action)
        {
            if (Volatile.Read(ref waitCount) >= maxWait)
                return false;
            Interlocked.Increment(ref waitCount);
            Semaphore.Wait();
            action?.Invoke();
            Semaphore.Release();
            Interlocked.Decrement(ref waitCount);
            return true;
        }
    }
}
