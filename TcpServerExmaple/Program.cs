using BigCookieKit.Communication;
using System;
using System.Text;
using System.Threading.Tasks;

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
                user.SendMessage("收到~");
            };

            tcpServer.OnExit = user =>
            {
                Console.WriteLine($"{user.UserHost}:{user.UserPort}离开~");
            };

            tcpServer.Handle = new TcpHandle();
            tcpServer.Handle.AddPipe<TestPipe>();

            tcpServer.Start();

            Console.ReadLine();
        }
    }

    class TestPipe : IPipe
    {
        public async Task InvokeAsync(Action context)
        {
            Console.WriteLine("接收前1");
            context?.Invoke();
            Console.WriteLine("接收后1");
        }
    }
}
