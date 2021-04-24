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

            //BenchmarkRunner.Run<ReflectCompare>();

            //Console.WriteLine($"字节数:[{Encoding.UTF8.GetBytes(Encode.message).Length}]");

            //BenchmarkRunner.Run<Encode>();

            //Encode encode = new Encode();
            //encode.MemoryStreamEncode();

        }
    }
}
