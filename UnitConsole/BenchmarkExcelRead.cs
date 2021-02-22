using BenchmarkDotNet.Attributes;
using BigCookieKit;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace UnitConsole
{
    public class BenchmarkExcelRead
    {

        [Benchmark(Description = "NpoiRead")]
        public void NpoiRead()
        {
            NpoiKit npoiKit = new NpoiKit(@"C:\Users\zbb58\Desktop\test.xlsx");
            npoiKit.CreateConfig(config =>
            {
                config.ColumnNameRow = 1;
                config.StartRow = 2;
            });
            DataTable dt = npoiKit.ToDataTable(npoiKit.GetSheet("Sheet1"));
        }

        [Benchmark(Description = "OpenXmlRead")]
        public void OpenXmlRead()
        {
            ReadExcelKit excelKit = new ReadExcelKit(@"C:\Users\zbb58\Desktop\test.xlsx");
            excelKit.CreateConfig(config =>
            {
                config.ColumnNameRow = 1;
                config.StartRow = 2;
            });
            DataTable dt = excelKit.ReadDataTable(1);
        }
    }
}
