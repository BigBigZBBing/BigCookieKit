using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BigCookieKit.Serialization.Binary
{
    /// <summary>
    /// 二进制序列化
    /// </summary>
    public class Binary : IDisposable
    {
        /// <summary>
        /// 处理器
        /// </summary>
        private ICollection<FormatterBase> Handles { get; set; }

        /// <summary>
        /// 压缩流
        /// </summary>
        internal Stream Stream { get; set; }

        /// <summary>
        /// 二进制核心
        /// </summary>
        public Binary Host { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; set; }

        public Binary()
        {
            Handles = new List<FormatterBase>();
            Stream = new MemoryStream();
            FormatterSort();
        }

        public T Read<T>(Type type)
        {
            foreach (var handle in Handles)
            {
                var obj = handle.Read(type);
                if (obj != null)
                {
                    return (T)obj;
                }
            }

            return default(T);
        }

        public bool Write(object value)
        {
            foreach (var handle in Handles)
            {
                if (!handle.Write(value)) return true;
            }

            return false;
        }

        public FormatterBase GetHandle<T>() where T : FormatterBase
        {
            foreach (var handle in Handles)
            {
                if (handle.GetType() == typeof(T))
                {
                    return handle;
                }
            }

            return null;
        }

        private void FormatterSort()
        {
            var tmp = new List<FormatterBase>();
            tmp.Add(new GeneralFormatter() { Host = this });

            foreach (var handle in Handles)
            {
                bool isInsert = false;
                for (int i = 0; i < tmp.Count; i++)
                {
                    var item = tmp[i];
                    if (handle.Priority < item.Priority)
                    {
                        tmp.Insert(i, handle);
                        isInsert = true;
                        break;
                    }
                }

                if (!isInsert) tmp.Add(handle);
            }

            Handles = tmp;
        }

        public void Dispose()
        {
            ((IDisposable)Stream).Dispose();
        }
    }
}
