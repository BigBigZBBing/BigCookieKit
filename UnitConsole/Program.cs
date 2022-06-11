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

            List<string> list = new List<string>();
            for (int i = 0; i < 100; i++)
            {
                list.Add("测试" + i);
            }
            var result = list.Where(x => x == "测试55");
            foreach (var item in result)
            {
                ;
            }
            //RpcClient api = new RpcClient("127.0.0.1", 8888);
            //IApiContact api1 = api.GetInstance<IApiContact>();
            //api1.Test("name");

            Console.ReadKey();
        }
    }

    public interface IApiContact
    {
        public void Ok();

        public void Test(string name);
    }
}
