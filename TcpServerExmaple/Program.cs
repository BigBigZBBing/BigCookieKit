using BigCookieKit.Communication;
using System;
using System.Net;
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
                string res = Encoding.UTF8.GetString(packet);
                Console.WriteLine($"[{user.UserHost}:{user.UserPort}]:{res}");
                user.SendMessage("收到~");
                HttpListener httpListener = new HttpListener();
            };

            tcpServer.OnExit = user =>
            {
                Console.WriteLine($"{user.UserHost}:{user.UserPort}离开~");
            };

            tcpServer.Handle = new EasyHandle();
            //tcpServer.Handle.AddPipe<TestPipe>();

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
