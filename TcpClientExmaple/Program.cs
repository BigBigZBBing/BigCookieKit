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
            NetworkClient client = new NetworkClient("127.0.0.1", 7447);

            client.OnConnect = user =>
            {
                Random random = new Random();
                while (true)
                {
                    byte[] message = Encoding.UTF8.GetBytes("测试数据" + random.Next(short.MaxValue));
                    //user.SendMessage(Console.ReadLine());
                    user.SendMessage(message);
                }
            };

            client.OnCallBack = (user, packet) =>
            {
                Console.WriteLine(Encoding.UTF8.GetString(packet));
            };

            client.Start();

            Thread.Sleep(-1);
        }
    }
}
