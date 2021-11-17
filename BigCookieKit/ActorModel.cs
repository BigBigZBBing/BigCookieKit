using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace BigCookieKit
{
    public class ActorModel<T>
    {
        /// <summary>
        /// 批处理块
        /// </summary>
        private BatchBlock<T> _batchBlock;
        /// <summary>
        /// 批处理执行块
        /// </summary>
        private ActionBlock<T[]> _actionBlock;

        /// <summary>
        /// 基本构造函数
        /// </summary>
        /// <param name="batchSize">每次处理的数据量</param>
        /// <param name="action">执行委托方法</param>
        /// <param name="boundedCapacity">最大处理的数据量 默认 int.MaxValue 2147483647</param>
        /// <param name="maxDegreeOfParallelism">最大并行量 默认1</param>
        /// <param name="timeTrigger">定时触发批处理 默认不处理， 设置大于0则处理，秒级别</param>
        public ActorModel(int batchSize, Func<T[], Task> action, int boundedCapacity = int.MaxValue, int maxDegreeOfParallelism = 1)
        {
            _batchBlock = new BatchBlock<T>(batchSize, new GroupingDataflowBlockOptions() { BoundedCapacity = boundedCapacity });
            _actionBlock = new ActionBlock<T[]>(data => action(data), new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism });
            _batchBlock.LinkTo(_actionBlock, new DataflowLinkOptions() { PropagateCompletion = true });
        }

        /// <summary>
        /// Post 数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Post(T model)
        {
            return _batchBlock.Post(model);
        }

        /// <summary>
        /// 返回当前执行总数
        /// </summary>
        /// <returns></returns>
        public int GetBatchSum()
        {
            return _batchBlock.Receive().Count();
        }

        /// <summary>
        /// 阻塞等待
        /// </summary>
        public void Complete(bool iswait = false)
        {
            _batchBlock.Complete();
            _batchBlock.Completion.Wait();
            _actionBlock.Complete();
            if (iswait) _actionBlock.Completion.Wait();
        }

        /// <summary>
        /// 阻塞等待
        /// </summary>
        /// <param name="time">毫秒</param>
        public void Complete(int time)
        {
            _batchBlock.Complete();
            _batchBlock.Completion.Wait();
            _actionBlock.Complete();
            if (time > 0) _actionBlock.Completion.Wait(time);
        }
    }
}
