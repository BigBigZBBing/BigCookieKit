using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace BigCookieKit
{
    public class Actor<T>
    {
        /// <summary>
        /// 多批次处理块
        /// </summary>
        private BatchBlock<T> _batchBlock;

        /// <summary>
        /// 多批次处理执行块
        /// </summary>
        private ActionBlock<T[]> _batchActionBlock;

        /// <summary>
        /// 单批次处理执行块
        /// </summary>
        private ActionBlock<T> _singleActionBlock;

        /// <summary>
        /// 批次处理的数量
        /// </summary>
        private int _batchSize = 1;

        /// <summary>
        /// 多批次的回调函数
        /// </summary>
        private Action<T[]> _batchAction;

        /// <summary>
        /// 单批次的回调函数
        /// </summary>
        private Action<T> _singleAction;

        /// <summary>
        /// 多批次细粒度处理量
        /// </summary>
        private int _boundedCapacity = int.MaxValue;

        /// <summary>
        /// 并行数量
        /// </summary>
        private int _maxDegreeOfParallelism = 1;

        public Actor(Action<T[]> action, int batchSize, int boundedCapacity, int maxDegreeOfParallelism)
        {
            _batchAction = action;
            _batchSize = batchSize;
            _boundedCapacity = boundedCapacity;
            _maxDegreeOfParallelism = maxDegreeOfParallelism;
            RefreshBlock();
        }

        public Actor(Action<T> action)
        {
            _singleAction = action;
            RefreshBlock();
        }

        public Actor(Action<T> action, int maxDegreeOfParallelism)
        {
            _singleAction = action;
            _maxDegreeOfParallelism = maxDegreeOfParallelism;
            RefreshBlock();
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Post(T model)
        {
            if (_batchAction != null)
            {
                return _batchBlock.Post(model);
            }
            else if (_singleAction != null)
            {
                return _singleActionBlock.Post(model);
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 等待完成
        /// </summary>
        public void Complete()
        {
            _batchBlock?.Complete();
            _batchBlock?.Completion.Wait();
            _batchActionBlock?.Complete();
            _batchActionBlock?.Completion.Wait();
            _singleActionBlock?.Complete();
            _singleActionBlock?.Completion.Wait();
            RefreshBlock();
        }

        private void RefreshBlock()
        {
            if (_batchAction != null)
            {
                _batchBlock = new BatchBlock<T>(_batchSize, new GroupingDataflowBlockOptions() { BoundedCapacity = _boundedCapacity });
                _batchActionBlock = new ActionBlock<T[]>(data => _batchAction(data), new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = _maxDegreeOfParallelism });
                _batchBlock.LinkTo(_batchActionBlock, new DataflowLinkOptions() { PropagateCompletion = true });
            }
            else if (_singleAction != null)
            {
                _singleActionBlock = new ActionBlock<T>(data => _singleAction(data), new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = _maxDegreeOfParallelism });
            }
            else
            {
                throw new ArgumentNullException();
            }
        }
    }
}
