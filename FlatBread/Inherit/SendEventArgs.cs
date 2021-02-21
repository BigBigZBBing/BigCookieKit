using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace FlatBread.Inherit
{
    /// <summary>
    /// 操作行为使用的接套字
    /// </summary>
    internal class SendEventArgs : SocketAsyncEventArgs
    {
        /// <summary>
        /// 发送回调
        /// </summary>
        internal Action<SocketAsyncEventArgs> SendAction { get; set; }

        //消息封包
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Encode(byte[] message)
        {
            var len = message.Length;
            Span<byte> packet;
            if (len <= byte.MaxValue)
            {
                packet = new byte[len + 2];
                packet[0] = 1;
                packet[1] = (byte)len;
                message.CopyTo(packet.Slice(2));
            }
            else if (len <= short.MaxValue)
            {
                packet = new byte[len + 3];
                packet[0] = 2;
                packet[2] = (byte)(len >> 8);
                packet[1] = (byte)len;
                message.CopyTo(packet.Slice(3));
            }
            else
            {
                packet = new byte[len + 5];
                packet[0] = 3;
                packet[4] = (byte)(len >> 24);
                packet[3] = (byte)(len >> 16);
                packet[2] = (byte)(len >> 8);
                packet[1] = (byte)len;
                message.CopyTo(packet.Slice(5));
            }

            //设置缓冲区
            SetBuffer(packet.ToArray());
            //给缓冲区赋值
            packet.CopyTo(MemoryBuffer.Span);
        }

        /// <summary>
        /// 请求断开封包
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Disconnect()
        {
            Span<byte> packet;
            packet = new byte[2];
            packet[0] = 0xFF;
            packet[1] = 0;
            SetBuffer(packet.ToArray());
            packet.CopyTo(MemoryBuffer.Span);
        }

        /// <summary>
        /// 请求重连封包
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Reconnection()
        {
            Span<byte> packet;
            packet = new byte[2];
            packet[0] = 0xFE;
            packet[1] = 0;
            SetBuffer(packet.ToArray());
            packet.CopyTo(MemoryBuffer.Span);
        }

    }
}
