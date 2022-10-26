using System;
using System.Collections.Generic;
using System.Text;

namespace BigCookieKit.Serialization.Binary
{
    public class DateTimeFormatter : FormatterBase
    {
        public Binary Host { get; set; }

        public int Priority { get; set; } = 2;

        public object Read(Type type)
        {
            if (type == typeof(DateTime))
            {
                var handle = Host.GetHandle<GeneralFormatter>();
                var obj = handle.Read(typeof(long));
                return DateTime.FromBinary((long)obj);
            }
            else if (type == typeof(TimeSpan))
            {
                var handle = Host.GetHandle<GeneralFormatter>();
                var obj = handle.Read(typeof(long));
                return TimeSpan.FromTicks((long)obj);
            }

            return Activator.CreateInstance(type);
        }

        public bool Write(object value)
        {
            var type = value.GetType();

            if (type == typeof(DateTime))
            {
                var tick = ((DateTime)value).Ticks;
                var handle = Host.GetHandle<GeneralFormatter>();
                return handle.Write(tick);
            }
            else if (type == typeof(TimeSpan))
            {
                var tick = ((TimeSpan)value).Ticks;
                var handle = Host.GetHandle<GeneralFormatter>();
                return handle.Write(tick);
            }

            return false;
        }
    }
}
