using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Communication
{
    /// <summary>
    /// 基础网络
    /// </summary>
    public class Network
    {
        /// <summary>
        /// 网络地址
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 网络协议
        /// </summary>
        public virtual NetworkProtocol Protocol { get; set; }

        /// <summary>
        /// 应用模式
        /// </summary>
        public virtual ApplyMode Mode { get; set; }

        /// <summary>
        /// 地址方案
        /// <para>默认为IPV4</para>
        /// </summary>
        public AddressFamily AddressFamily { get; set; } = AddressFamily.InterNetwork;

        /// <summary>
        /// 通信方式
        /// </summary>
        public SocketType SocketType { get; set; } = SocketType.Stream;

        /// <summary>
        /// 编码方式
        /// </summary>
        public Encoding Encoder { get; set; } = Encoding.UTF8;

        /// <summary>
        /// 数据处理器
        /// </summary>
        public Handle Handle { get; set; }

        /// <summary>
        /// 缓存片大小 默认4K
        /// </summary>
        public int BufferSize { get; set; } = 4096;

        /// <summary>
        /// SSL协议
        /// </summary>
        public SslProtocols SslProtocols { get; set; } = SslProtocols.None;

        /// <summary>
        /// 当前套接字
        /// </summary>
        internal XSocket CurrSocket { get; set; }
        /// <summary>
        /// 安全证书
        /// </summary>
        public X509Certificate Certificate { get; set; }

        /// <summary>
        /// 缓存池
        /// </summary>
        internal ArrayPool<byte> BufferPool = ArrayPool<byte>.Create();

        public Network(string Host, int Port)
        {
            this.Host = Host;
            this.Port = Port;
        }

        /// <summary>
        /// 解析地址
        /// </summary>
        /// <returns></returns>
        public IPEndPoint GetAddress() => new IPEndPoint(Dns.GetHostAddresses(Host).FirstOrDefault(), Port);
    }
}
