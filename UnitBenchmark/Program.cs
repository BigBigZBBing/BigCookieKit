using BenchmarkDotNet.Running;
using System;

namespace UnitBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run(typeof(BoyerMooreBenchmark));
        }
    }
}
