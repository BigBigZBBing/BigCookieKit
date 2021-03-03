using System;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using BigCookieKit;
using System.Data;
using System.Linq;
using System.Net.Http;
using BigCookieKit.Reflect;
using System.Threading.Tasks.Dataflow;
using System.Threading.Tasks;
using System.Threading;

namespace UnitConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            ThreadPool.SetMinThreads(100, 100);
            var block = new ActionBlock<int>(index =>
            {
                Console.WriteLine(index);
                Thread.Sleep(500);
            }, new ExecutionDataflowBlockOptions()
            {
                MaxDegreeOfParallelism = 100,
            });
            for (int i = 0; i < 400; i++)
            {
                block.Post(i);
            }
            block.Complete();
            block.Completion.Wait();
            Console.WriteLine("完成");

            Console.ReadKey();
        }
    }
}
