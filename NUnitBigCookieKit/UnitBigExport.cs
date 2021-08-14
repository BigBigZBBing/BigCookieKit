using BigCookieKit.Office;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnitBigCookieKit
{
    public class UnitBigExport
    {
        DataSet bigData { get; set; }

        [SetUp]
        public void Setup()
        {
            bigData = new DataSet();

            DataTable dt = new DataTable();

            dt.Columns.Add("Field1");
            dt.Columns.Add("Field2");
            dt.Columns.Add("Field3");
            dt.Columns.Add("Field4");
            dt.Columns.Add("Field5");
            dt.Columns.Add("Field6");
            dt.Columns.Add("Field7");
            dt.Columns.Add("Field8");

            for (int i = 0; i < 1000000; i++)
            {
                dt.Rows.Add(new object[] { "value1", "value2", "value3", "value4", "value5", "value6", "value7", "value8" });
            }

            bigData.Tables.Add(dt);
        }


        [Test]
        public void WriteExcelBigUnit()
        {
            string path1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WriteExcelBigUnit.xlsx");
            using (WriteExcelKit writeExcel = new WriteExcelKit(path1))
            {
                writeExcel.Save(bigData);
            }
        }
    }
}
