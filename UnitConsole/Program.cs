using System;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using GeneralKit;
using System.Data;
using System.Linq;
using System.Net.Http;

namespace UnitConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("authorization", "");

            //BenchmarkRunner.Run<BenchmarkExcelRead>();
        }
    }
}
