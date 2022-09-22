using BenchmarkDotNet.Running;

using BigCookieKit.Algorithm;

using System;

namespace UnitBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            //string s1 = "HERE IS A SIMPLE EXAMPLE";
            //string s2 = "EXAMPLE";
            //BoyerMoore.BoyerMooreFirstMatch(s1.ToCharArray(), s2.ToCharArray());

            BoyerMooreBenchmark boyerMooreBenchmark = new BoyerMooreBenchmark();
            boyerMooreBenchmark.BoyerMooreChar();

            //BenchmarkRunner.Run(typeof(BoyerMooreBenchmark));
            //BenchmarkRunner.Run(typeof(MapToBenchmark));
            //BenchmarkRunner.Run(typeof(ExcelReadBenchmark));
        }
    }
}
