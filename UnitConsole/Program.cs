using System;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using BigCookieKit;
using System.Data;
using System.Linq;
using System.Net.Http;
using BigCookieKit.Reflect;

namespace UnitConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("authorization", "");

            var action = SmartBuilder.DynamicMethod<Action>(string.Empty, emit =>
            {
                //var int1 = 0;
                //while(true)
                //{
                //  if(int1>5) break;
                //}
                var int1 = emit.NewInt32();
                emit.While(() => emit.NewBoolean(true).Output(), tab =>
                {
                    emit.IF(int1 > 5, () => tab.Break()).IFEnd();
                    int1 += 1;
                });

                //for(int i =0; i<10 ; i++)
                //{
                //  Console.WriteLine(i);
                //}
                emit.For(0, 10, (index, tab) =>
                {
                    emit.ReflectStaticMethod("WriteLine", typeof(Console), index);
                });

                //for(int i =10; i>=0 ; i--)
                //{
                //  Console.WriteLine(i);
                //}
                emit.Forr(10, 0, (index, tab) =>
                {
                    emit.ReflectStaticMethod("WriteLine", typeof(Console), index);
                });

                emit.Return();
            });
            action.Invoke();


            //BenchmarkRunner.Run<BenchmarkExcelRead>();
        }
    }
}
