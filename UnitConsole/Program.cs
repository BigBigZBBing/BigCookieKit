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
    class TModel
    {
        public string Name { get; set; }

        public void TestCall()
        {
            Console.WriteLine("调用测试");
        }

        public string TestCall1()
        {
            Console.WriteLine("调用测试1");
            return "测试返回";
        }
    }
    class Program
    {
        static void Main(string[] args)
        {

            var action = SmartBuilder.DynamicMethod<Action<object>>(string.Empty, emit =>
            {
                //TModel obj = (TModel)param1;
                var obj = emit.NewObject(emit.ArgumentRef<object>(0));
                var tmodel = obj.As<TModel>();
                emit.IF(tmodel.IsNull(), () =>
                {
                    emit.ReflectStaticMethod("WriteLine", typeof(Console), emit.NewInt32(1));
                }).IFEnd();

                emit.Return();
            });
            action.Invoke(null);

            Console.ReadKey();
            //BenchmarkRunner.Run<BenchmarkExcelRead>();
        }
    }
}
