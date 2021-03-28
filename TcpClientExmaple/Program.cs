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
                    //Console.ReadLine();
                    user.SendMessage(RamdomString());
                    //string str = Console.ReadLine();
                    //switch (str)
                    //{
                    //    case "close":
                    //        user.Disconnect();
                    //        break;
                    //    default:
                    //        user.SendMessage(str);
                    //        break;
                    //}
                }
            };

            client.OnCallBack = (user, packet) =>
            {
                Console.WriteLine(Encoding.UTF8.GetString(packet));
            };

            client.Handle = new EasyHandle();
            client.Start();

            Thread.Sleep(-1);
        }

        public static byte[] RamdomString(int length = 1024)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            var random = new Random();
            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length - 1)];
            }
            return System.Text.Encoding.UTF8.GetBytes(stringChars);
        }
    }
}
