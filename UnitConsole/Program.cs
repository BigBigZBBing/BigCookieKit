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

namespace UnitConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var a1 = GC.GetTotalMemory(true);
            Chain<string> stream = new Chain<string>();
            for (int i = 0; i < short.MaxValue; i++)
            {
                stream.Add("你说啥" + i);
            }
            var a2 = GC.GetTotalMemory(true);
            Console.WriteLine(a2 - a1);
            var array = stream.ToArray();

            Console.ReadKey();
            //List<string> list = new List<string>();
            //for (int i = 0; i < 1000; i++)
            //{
            //    list.Add("你说啥" + i);
            //}

            //BenchmarkRunner.Run<ReflectCompare>();

            //Console.WriteLine($"字节数:[{Encoding.UTF8.GetBytes(Encode.message).Length}]");

            //BenchmarkRunner.Run<Encode>();

            //Encode encode = new Encode();
            //encode.MemoryStreamEncode();

        }
    }
}
