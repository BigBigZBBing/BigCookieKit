namespace BigCookieKit.AspCore.ConcurrencyLimiter
{
    public class ConcurrencyLimiterOption
    {
        /// <summary>
        /// 限流地址
        /// </summary>
        public string RequestPath { get; set; }

        /// <summary>
        /// 队列最大等待数量
        /// </summary>
        public int RequestQueueLimit { get; set; }

        /// <summary>
        /// 最大并行数量
        /// </summary>
        public int MaxConcurrentRequests { get; set; }

        /// <summary>
        /// 队列满返回消息
        /// </summary>
        public string QueueFullMessage { get; set; }
    }
}
