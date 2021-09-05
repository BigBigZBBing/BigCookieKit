using BenchmarkDotNet.Attributes;
using BigCookieKit.Office.Xlsx;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitBenchmark
{
    public class ExcelReadBenchmark
    {
        public readonly string resource = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");

        [Benchmark]
        public void MiniRead()
        {
            string path = Path.Combine(resource, "test.xlsx");
            var rows = MiniExcelLibs.MiniExcel.Query(path);

            //自行转换成DataTable
            DataTable dt = new DataTable();
            foreach (var item in rows)
            {
                if (dt.Columns.Count == 0)
                {
                    foreach (var item1 in (IDictionary<string, object>)item)
                        dt.Columns.Add(item1.Key);
                }
                else
                {
                    DataRow ndr = dt.NewRow();
                    foreach (var item1 in (IDictionary<string, object>)item)
                    {
                        if (dt.Columns[item1.Key].DataType != item1.Value.GetType())
                            dt.Columns[item1.Key].DataType = item1.Value.GetType();
                        ndr[item1.Key] = item1.Value;
                    }
                    dt.Rows.Add(ndr);
                }
            }

        }

        [Benchmark]
        public void XmlRead()
        {
            string path = Path.Combine(resource, "test.xlsx");
            ReadExcelKit excelKit = new ReadExcelKit(path);
            excelKit.AddConfig(config =>
            {
                config.SheetIndex = 1;
                config.ColumnNameRow = 1;
                config.StartRow = 2;
            });
            DataTable dt = excelKit.XmlReadDataTable();
        }
    }
}
