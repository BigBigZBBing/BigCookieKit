using System;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;

namespace UnitConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<BenchmarkExcelRead>();
        }
    }
}
