using BigCookieKit.Communication;
using System;
using System.Net;
using System.Text;
using System.Threading;

namespace TcpClientExmaple
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpClient client = new TcpClient("127.0.0.1", 7447);

            client.OnConnect = user =>
            {
                Random random = new Random();
                while (true)
                {
                    byte[] message = Encoding.UTF8.GetBytes("测试数据" + random.Next(short.MaxValue));
                    user.SendMessage(message);
                    Thread.Sleep(1);
                }
            };

            client.OnCallBack = (user, packet) =>
            {

            };

            client.StartConnect();

            Console.ReadLine();
        }
    }
}
