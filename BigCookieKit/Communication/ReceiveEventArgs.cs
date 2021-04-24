using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace BigCookieKit.Communication
{
    /// <summary>
    /// 操作行为使用的接套字
    /// </summary>
    public class ReceiveEventArgs : SocketAsyncEventArgs
    {
#if NET452
        public Memory<byte> MemoryBuffer { get { return this.Buffer; } }
#endif

        /// <summary>
        /// 接收回调
        /// </summary>
        internal Action<SocketAsyncEventArgs> ReceiveAction { get; set; }

        /// <summary>
        /// 解封包
        /// </summary>
        /// <param name="callback"></param>
        
        internal void Decode(Action<Packet> callback)
        {
            UserTokenSession UserToken = this.UserToken as UserTokenSession;
            int offset = 0;
            var curBytes = MemoryBuffer.Span;
            while (offset < BytesTransferred)
            {
                try
                {
                    //测试服务端解析二进制流出现问题
                    //if (UserToken.Mode == SocketMode.Server)
                    //    throw new Exception();
                    offset += UserToken.Cache.LoadHead(curBytes.Slice(offset));
                    offset += UserToken.Cache.LoadBody(curBytes.Slice(offset));
                }
                catch
                {
                    UserToken.SendReconnect();
                    return;
                }

                if (UserToken.Cache.IsCompleted())
                {
                    PacketCommand(callback);
                }
            }
        }

        
        bool PacketCommand(Action<Packet> bytes)
        {
            UserTokenSession UserToken = this.UserToken as UserTokenSession;
            switch (UserToken.Cache.Mode)
            {
                //如果是消息包就回调
                case MessageMode.MessageShort:
                case MessageMode.MessageInt:
                    bytes?.Invoke(UserToken.Cache);
                    break;
                //如果是请求断开
                case MessageMode.Disconect:
                    SocketDisconect();
                    break;
                //如果是请求重连
                case MessageMode.Reconnect:
                    SocketReconect();
                    break;
            }
            UserToken.Cache = null;
            return true;
        }

        
        void SocketDisconect()
        {
            UserTokenSession UserToken = this.UserToken as UserTokenSession;
            switch (UserToken.Mode)
            {
                case ApplyMode.Server:
                    UserToken.SendDisconnect();
                    break;
                case ApplyMode.Client:
                    UserToken.Channel.Disconnect(false);
                    break;
            }
        }

        
        void SocketReconect()
        {
            UserTokenSession UserToken = this.UserToken as UserTokenSession;
            switch (UserToken.Mode)
            {
                case ApplyMode.Server:
                    UserToken.SendReconnect();
                    break;
                case ApplyMode.Client:
                    UserToken.Channel.Disconnect(true);
                    UserToken.ShakeHandEvent.ReConnect();
                    break;
            }
        }
    }
}
