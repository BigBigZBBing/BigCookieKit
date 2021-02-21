using System;
using System.Collections.Generic;
using System.Text;

namespace FlatBread.Buffer
{
    /// <summary>
    /// 缓冲位
    /// </summary>
    public class Buffer
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="Bytes">缓冲区</param>
        /// <param name="Offset">偏移量</param>
        /// <param name="Length">位长度</param>
        /// <returns></returns>
        public static Buffer New(ref byte[] Bytes, int Offset, int Length)
        {
            return new Buffer(ref Bytes, Offset, Length);
        }

        /// <summary>
        /// 缓冲区字节流
        /// </summary>
        private byte[] Bytes { get; set; }

        /// <summary>
        /// 缓冲位字节流
        /// </summary>
        private Memory<byte> Data { get { return Bytes.AsMemory().Slice(Offset, Length); } }

        /// <summary>
        /// 缓冲区偏移量
        /// </summary>
        private int Offset { get; set; }

        /// <summary>
        /// 缓冲位长度
        /// </summary>
        private int Length { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Bytes">缓冲区</param>
        /// <param name="Offset">偏移量</param>
        /// <param name="Length">位长度</param>
        public Buffer(ref byte[] Bytes, int Offset, int Length)
        {
            this.Bytes = Bytes;
            this.Offset = Offset;
            this.Length = Length;
        }

        /// <summary>
        /// 清扫缓冲位
        /// </summary>
        public void Free()
        {
            for (int i = 0; i < Length; i++)
            {
                this[i] = 0;
            }
        }

        public byte this[int index]
        {
            get
            {
                return Bytes[index + Offset];
            }
            set
            {
                Bytes[index + Offset] = value;
            }
        }

        public static implicit operator Memory<byte>(Buffer buffer) => buffer.Data;
    }
}
