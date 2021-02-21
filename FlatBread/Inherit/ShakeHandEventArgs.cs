using FlatBread.Buffer;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace FlatBread.Inherit
{
    public class ShakeHandEventArgs : SocketAsyncEventArgs
    {
        /// <summary>
        /// 接收使用的接套字
        /// </summary>
        internal ReceiveEventArgs ReceiveEventArgs { get; set; }

        /// <summary>
        /// 发送使用的接套字
        /// </summary>
        internal SendEventArgs SendEventArgs { get; set; }

        /// <summary>
        /// 重新连接机制
        /// </summary>
        internal Action ConnectAction { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ReceiveEventArgs">接收使用的接套字</param>
        /// <param name="SendEventArgs">发送使用的接套字</param>
        internal ShakeHandEventArgs(ReceiveEventArgs ReceiveEventArgs, SendEventArgs SendEventArgs)
        {
            this.ReceiveEventArgs = ReceiveEventArgs;
            this.SendEventArgs = SendEventArgs;
        }

        /// <summary>
        /// 清理缓存
        /// </summary>
        internal void Clear()
        {
            base.AcceptSocket?.Close();
            base.AcceptSocket?.Dispose();
            base.AcceptSocket = null;
        }

        /// <summary>
        /// 重新连接
        /// </summary>
        internal void ReConnect()
        {
            ConnectAction?.Invoke();
        }
    }
}
