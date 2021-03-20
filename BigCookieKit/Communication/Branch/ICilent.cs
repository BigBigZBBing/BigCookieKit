using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Communication
{
    public interface ICilent
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
        void ProcessConnect(SocketAsyncEventArgs e);

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
        /// 接收结果回调
        /// </summary>
        Action<Session, byte[]> OnCallBack { get; set; }

        /// <summary>
        /// 连接成功回调
        /// </summary>
        Action<Session> OnConnect { get; set; }
    }
}
