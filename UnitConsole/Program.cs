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

namespace UnitConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            //BenchmarkRunner.Run<ReflectCompare>();

            //Console.WriteLine($"字节数:[{Encoding.UTF8.GetBytes(Encode.message).Length}]");

            //BenchmarkRunner.Run<Encode>();

            //Encode encode = new Encode();
            //encode.MemoryStreamEncode();

            //BPlusTreeTest.Example();

            Snowflake.WorkId = 0;

            for (int i = 0; i < 1000; i++)
            {
                Console.WriteLine(Snowflake.NextId());
            }

            Console.ReadKey();

            //var provider = new RSAProvider<SHA256>();
            //var t1 = provider.ExportPublicKey();
            //var t2 = RSATest.WritePkcs1PublicKey(provider.publickKey).Encode();
            //var res = t1.Except(t2.ToList()).ToList();

            //var t3 = provider.ExportPrivateKey();
            //var t4 = RSATest.WritePkcs1PrivateKey(provider.privateKey).Encode();
            //var t6 = RSATest.WritePkcs8PrivateKey(t4).Encode();
            //var res1 = t3.Except(t4.ToList()).ToList();


            ApiClient api = new ApiClient("127.0.0.1", 8888);

            IApiContact api1 = api.GetInstance<IApiContact>();
        }
    }

    public interface IApiContact
    {
        public void Ok();
    }
}
