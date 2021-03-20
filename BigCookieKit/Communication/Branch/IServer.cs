using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Communication
{
    public interface IServer
    {
        /// <summary>
        /// 开启服务
        /// </summary>
        void Start();

        /// <summary>
        /// 开始
        /// </summary>
        void Open();

        /// <summary>
        /// 连接过程
        /// </summary>
        void ProcessAccept(SocketAsyncEventArgs e);

        /// <summary>
        /// 发送过程
        /// </summary>
        void ProcessSend(SocketAsyncEventArgs e);

        /// <summary>
        /// 接收过程
        /// </summary>
        void ProcessReceive(SocketAsyncEventArgs e);

        /// <summary>
        /// 调度中心
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DispatchCenter(object sender, SocketAsyncEventArgs e);

        /// <summary>
        /// 用户连接成功回调
        /// </summary>
        Action<Session> OnConnect { get; set; }

        /// <summary>
        /// 接收到数据回调
        /// </summary>
        Action<Session, byte[]> OnReceive { get; set; }

        /// <summary>
        /// 用户断开连接回调
        /// </summary>
        Action<Session> OnExit { get; set; }
    }
}
