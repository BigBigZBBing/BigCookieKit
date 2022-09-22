using System;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks.Dataflow;
using System.Threading.Tasks;
using System.Threading;
using BigCookieKit.Reflect;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json.Serialization;
using System.Net.Sockets;
using System.Reflection;
using BigCookieKit.IO;
using System.Collections.Generic;
using BigCookieKit.Algorithm;
using System.Security.Cryptography;
using BigCookieKit;
using BigCookieKit.Rpc;
using System.Security.Claims;

namespace UnitConsole
{
    class Program
    {
        public class ActorModel
        {
            public Actor<ActorModel> Actor { get; set; }

            public object Packet { get; set; }
        }

        static void Main(string[] args)
        {
            //BenchmarkRunner.Run<ReflectCompare>();

            //RpcClient api = new RpcClient("127.0.0.1", 8888);
            //IApiContact api1 = api.GetInstance<IApiContact>();
            //api1.Test("name");

            Actor<string> actor = new Actor<string>((items) =>
            {
                foreach (var item in items)
                {
                    Console.WriteLine("收到:" + item);
                }
            }, 10, int.MaxValue, 1);

            for (int i = 0; i < 5; i++)
            {
                actor.Post(i.ToString());
            }
            actor.Complete();

            Console.ReadKey();
        }
    }

    public interface IApiContact
    {
        public void Ok();

        public void Test(string name);
    }
}
