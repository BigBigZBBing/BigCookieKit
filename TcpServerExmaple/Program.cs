using BigCookieKit.Communication;
using System;
using System.Text;

namespace TcpServerExmaple
{
    class Program
    {
        static void Main(string[] args)
        {
            NetworkServer tcpServer = new NetworkServer(7447);

            tcpServer.OnConnect = user =>
            {
                Console.WriteLine($"{user.UserHost}:{user.UserPort}接入~");
            };

            tcpServer.OnReceive = (user, packet) =>
            {
                Console.WriteLine($"[{user.UserHost}:{user.UserPort}]:{Encoding.UTF8.GetString(packet)}");
                user.SendMessage("测试数据");
            };

            tcpServer.OnExit = user =>
            {
                Console.WriteLine($"{user.UserHost}:{user.UserPort}离开~");
            };

            tcpServer.Start();

            Console.ReadLine();
        }
    }
}
