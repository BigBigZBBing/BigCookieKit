using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace BigCookieKit.Algorithm
{
    /// <summary>
    /// 雪花算法(变种版)
    /// <code>
    /// 格式:
    /// 1.unsigned	1bit
    /// 2.time  	41bit
    /// 3.work      10bit
    /// 4.timebit	8bit
    /// 5.lastindex	4bit
    /// </code>
    /// </summary>
    public static class Snowflake
    {
        public static int? WorkId { get; set; }

        private static DateTime StartTimeStamp = DateTime.Now;
        private static long _lastTimestamp = -1L;
        private static int _lastIndex = 0;
        private static int _timeBit = 1;
        private static long _maxTime = FullBit(1, 41);
        private static long _maxWork = FullBit(1, 10);
        private static long _maxTimeBit = FullBit(1, 8);
        private static long _maxLastIndex = FullBit(1, 4);

        public static long NextId()
        {
            if (WorkId == null)
            {
                throw new ArgumentNullException(nameof(WorkId));
            }
            var _workId = WorkId.Value;

            long currentTimeStamp = DateTime.Now.Ticks - StartTimeStamp.Ticks;
            if (currentTimeStamp > _lastTimestamp)
            {
                Interlocked.Exchange(ref _lastIndex, 0);
                Interlocked.Exchange(ref _lastTimestamp, currentTimeStamp);
            }
            else if (currentTimeStamp == _lastTimestamp)
            {
                Interlocked.Increment(ref _lastIndex);
            }
            else if (currentTimeStamp < _lastTimestamp)
            {
                throw new ArgumentException("currentTimeStamp < lastTimestamp");
            }

            currentTimeStamp /= _timeBit * 100;
            while (currentTimeStamp > _maxTime)
            {
                Interlocked.Increment(ref _timeBit);
                currentTimeStamp /= _timeBit * 100;
            }

            if (currentTimeStamp > _maxTime) throw new ArgumentNullException(nameof(currentTimeStamp));
            if (_workId > _maxWork) throw new ArgumentNullException(nameof(_workId));
            if (_timeBit > _maxTimeBit) throw new ArgumentNullException(nameof(_maxTimeBit));
            if (_lastIndex > _maxLastIndex) throw new ArgumentNullException(nameof(_maxLastIndex));

            var time = currentTimeStamp << 22;
            var work = _workId << 12;
            var timebit = _timeBit << 4;

            return time | work | timebit | _lastIndex;
        }

        private static long FullBit(long number, int bit)
        {
            var left = number << bit;
            var right = left - 1;
            return left + right;
        }
    }
}
