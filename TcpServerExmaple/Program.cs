using BigCookieKit.Network;

using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TcpServerExmaple
{
    class Program
    {
        static int connection = 0;

        static void Main(string[] args)
        {
            Console.Title = $"连接数:{connection}";
            NetworkServer tcpServer = new NetworkServer(7447);
            tcpServer.BufferSize = 4096;
            tcpServer.OnConnect = user =>
            {
                Console.WriteLine($"{user.UserHost}:{user.UserPort}接入~");
                Interlocked.Increment(ref connection);
                Console.Title = $"连接数:{connection}";
            };

            tcpServer.OnReceive = (user, packet) =>
            {
                string res = Encoding.UTF8.GetString(packet);
                Console.WriteLine($"[{user.UserHost}:{user.UserPort}]:{res}");
                //user.SendMessage("收到~");
            };

            tcpServer.OnExit = user =>
            {
                Console.WriteLine($"{user.UserHost}:{user.UserPort}离开~");
                Interlocked.Decrement(ref connection);
                Console.Title = $"连接数:{connection}";
            };

            //tcpServer.Handle = new EasyHandle();
            //tcpServer.Handle.AddPipe<TestPipe>();

            tcpServer.Start();

            Thread.Sleep(-1);
        }
    }

    class TestPipe : IPipe
    {
        public async Task InvokeAsync(Session session, Action context)
        {
            Console.WriteLine("接收前1");
            context?.Invoke();
            Console.WriteLine("接收后1");
        }
    }
}
