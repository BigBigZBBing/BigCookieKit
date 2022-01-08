using BigCookieKit;
using BigCookieKit.File;
using BigCookieKit.Office;
using BigCookieKit.Office.Xlsx;
using BigCookieKit.Resources;
using BigCookieKit.XML;
using BigCookieSequelize;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using NPOI.Util;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Xml.Linq;
using BigCookieKit.Algorithm;
using System.Security.Cryptography;

namespace NUnitBigCookieKit
{
    public class UnitKit
    {
        public readonly string resource = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");

        [SetUp]
        public void Setup()
        {

        }

        /// <summary>
        /// 模型类校验单元测试
        /// </summary>
        [Test]
        public void VerifyUnit()
        {
            VerifyModel model = new VerifyModel();
            //model.StrField = "jskdlfjlsdjlflsdlfjlsdlkf";
            model.IntField = 10;
            //model.DecimalField = 10.5264m;

            Assert.IsTrue(model.ModelValidation());
        }

        /// <summary>
        /// 空值断言单元测试
        /// </summary>
        [Test]
        public void NullAssertUnit()
        {
            //类型
            VerifyModel model = null;
            model.IsNull();
            model.NotNull();

            //引用类型
            string str = "";
            //默认值和NULL都为true
            str.IsNull();
            str.NotNull();
            str.IsNull(false);
            str.NotNull(false);

            //值类型
            int value = 0;
            //默认值和NULL都为true
            value.IsNull();
            value.IsNull(false);
            value.NotNull();
            value.NotNull(false);

            Assert.IsTrue(true);
        }

        /// <summary>
        /// 获取特性附带值单元测试
        /// </summary>
        [Test]
        public void EnumDisplayUnit()
        {
            TestEnum test = TestEnum.None;
            TestEnum test1 = TestEnum.True;
            TestEnum test2 = TestEnum.False;

            //""
            test.ToDisplay();
            //"正确"
            test1.ToDisplay();
            //"错误"
            test2.ToDisplay();

            Assert.IsTrue(true);
        }

        /// <summary>
        /// 集合断言单元测试
        /// </summary>
        [Test]
        public void ExistCountAssertUnit()
        {
            List<string> test = new List<string>();
            test.Exist();
            test.NotExist();

            ICollection<string> test1 = new List<string>();
            test1.Exist();
            test1.NotExist();

            IEnumerable<string> test2 = new List<string>();
            test2.Exist();
            test2.NotExist();

            Dictionary<string, string> test3 = new Dictionary<string, string>();
            test3.Exist();
            test3.NotExist();

            DataTable test4 = new DataTable();
            test4.Exist();
            test4.NotExist();

            DataSet test5 = new DataSet();
            test5.Exist();
            test5.NotExist();

            Assert.IsTrue(true);
        }

        /// <summary>
        /// DataRow安全取值赋值单元测试
        /// </summary>
        [Test]
        public void DataRowSetValueUnit()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Filed1", typeof(string)));
            dt.Columns.Add(new DataColumn("Filed2", typeof(decimal)));
            dt.Columns.Add(new DataColumn("Filed3", typeof(decimal)));

            DataRow dr = dt.NewRow();
            dr["Filed1"] = null;
            //dr["Filed2"] = null;//值类型赋值null会报错
            dr.CellSetValue("Filed2", null);//自动会解决问题

            dt.Rows.Add(dr);
        }

        /// <summary>
        /// Xml转成实体单元测试
        /// </summary>
        [Test]
        public void XmlToEntityUnit()
        {
            var ele = XElement.Parse($@"
<Root attr1=""attr1"" attr2=""attr2"">
	<Node attr3=""attr3"" attr4=""attr4"">
		<Field attr5=""attr5"" attr6=""attr6"" />
		<Field attr5=""attr5"" attr6=""attr6"" />
		<Field attr5=""attr5"" attr6=""attr6"" />
	</Node>
</Root>");

            XmlRoot test1 = ele.XmlToEntity<XmlRoot>();
        }

        /// <summary>
        /// 金额数字转大写单元测试
        /// </summary>
        [Test]
        public void MoneyToUpperUnit()
        {
            decimal test = 8436.44868M;

            string temp = test.MoneyUpper();
        }

        /// <summary>
        /// 尝试转换单元测试
        /// </summary>
        [Test]
        public void TryParseUnit()
        {
            string test = "15358";
            if (test.TryParse<int>(out var temp))
            {
            }

            if (test.TryParse<int>())
            {
            }
        }

        /// <summary>
        /// 基础类型断言单元测试
        /// </summary>
        [Test]
        public void TypeAssertUnit()
        {
            string classObj = "";
            bool test1 = classObj.IsClass();
            bool test2 = classObj.IsValue();
            bool test3 = classObj.IsStruct();
            bool test4 = classObj.IsEnum();
            bool test5 = classObj.IsCustomClass();

            int valueObj = 0;
            test1 = valueObj.IsClass();
            test2 = valueObj.IsValue();
            test3 = valueObj.IsStruct();
            test4 = valueObj.IsEnum();

            TestEnum enumObj = TestEnum.None;
            test1 = enumObj.IsClass();
            test2 = enumObj.IsValue();
            test3 = enumObj.IsStruct();
            test4 = enumObj.IsEnum();

            TestStruct structObj = new TestStruct();
            test1 = structObj.IsClass();
            test2 = structObj.IsValue();
            test3 = structObj.IsStruct();
            test4 = structObj.IsEnum();
        }

        [Test]
        public void SequelizeUnit()
        {
            using (MySqlConnection connection = new MySqlConnection("Server=localhost;Port=3306;Database=dbTest;Uid=root;Pwd=zhangbingbin5896;"))
            {
                var Obj = SequelizeStructured.SequelizeConfig(new
                {
                    fields = new[] { "UserId", "LogonName", "Password", "ExternalId", "CustomerId", "NickName", "FirstName", "LastName", "Phone", "Email", "Gender", "ActiveDate", "LastLogonTime", "InvitateCode", "ExpirationDate", "Source", "AccountType", "Status", "Disable", "CreateUserId", "CreateTime", "UpdateUserId", "UpdateTime", "Version" },
                    model = "cus_user",
                    where = new
                    {
                        eq = new { UserId = 135686 }
                    },
                    include = new[] {
                        new{
                            fields = new string[] { "UserFieldId","UserId","FieldId","DataValue","Disable","CreateTime","UpdateTime","CreateUserId","UpdateUserId","Version" },
                            model = "cus_userfield",
                            join = new{
                                cus_user = "UserId",
                                cus_userfield = "UserId"
                            }
                        }
                    }
                }, connection);
                string json = JsonConvert.SerializeObject(Obj);
            }
        }

        [Test]
        public void ZipUnit()
        {
            Kit.DirToFormZipPacket(@"C:\Users\zbb58\Desktop\test.zip", @"C:\Users\zbb58\Desktop\Program");
            //Kit.FileToFormZipPacket(@"C:\Users\zbb58\Desktop\test.zip", @"C:\Users\zbb58\Desktop\Excel测试文件\test.xlsx");
        }

        /// <summary>
        /// Xml流读取Excel2007转集合单元测试
        /// </summary>
        [Test]
        public void XmlReadSetUnit()
        {
            string path = Path.Combine(resource, "test2.xlsx");
            ReadExcelKit excelKit = new ReadExcelKit(path);
            excelKit.AddConfig(config =>
            {
                config.SheetIndex = 1;
                config.StartRow = 3;
                //config.EndRow = 4;
                config.StartColumn = "C";
                config.EndColumn = "D";
            });
            var rows = excelKit.XmlReaderSet();
        }

        /// <summary>
        /// Xml流读取Excel2007转字典单元测试
        /// </summary>
        [Test]
        public void XmlReadDictionaryUnit()
        {
            string path = Path.Combine(resource, "test.xlsx");
            ReadExcelKit excelKit = new ReadExcelKit(path);
            excelKit.AddConfig(config =>
            {
                config.SheetIndex = 1;
                config.ColumnNameRow = 1;
                config.StartRow = 2;
                //config.EndRow = 100;
                //config.StartColumn = "C";
                //config.EndColumn = "D";
            });
            var dics = excelKit.XmlReaderDictionary();
        }

        /// <summary>
        /// Xml流读取Excel2007转DataTable单元测试
        /// </summary>
        [Test]
        public void XmlReadDataTableUnit()
        {
            string path = Path.Combine(resource, "test.xlsx");
            ReadExcelKit excelKit = new ReadExcelKit(path);
            excelKit.AddConfig(config =>
            {
                config.SheetIndex = 1;
                config.ColumnNameRow = 1;
                config.StartRow = 2;
                //config.StartColumn = "C";
                //config.EndColumn = "D";
                //config.EndRow = 6;
                //config.ColumnSetting = new[] {
                //    new ColumnConfig(){ ColumnName="Id", ColumnType=typeof(int), NormalType= ColumnNormal.Increment },
                //    new ColumnConfig(){ ColumnName="UniqueNo", ColumnType=typeof(Guid), NormalType= ColumnNormal.Guid },
                //    new ColumnConfig(){ ColumnName="CreateTime", ColumnType=typeof(DateTime), NormalType= ColumnNormal.NowDate },
                //    new ColumnConfig(){ ColumnName="测试1", ColumnType=typeof(string), Column="A" },
                //    new ColumnConfig(){ ColumnName="测试2", ColumnType=typeof(string), Column="B" },
                //};
            });
            DataTable dt = excelKit.XmlReadDataTable();
        }

        /// <summary>
        /// Actor模型（并集线程）单元测试
        /// </summary>
        [Test]
        public async Task ActorModelUnit()
        {
            var batch = new ActorModel<string>(1, async strs =>
            {
                foreach (var item in strs)
                {
                    Console.WriteLine("生成Excel数据：{0}", item);
                    await Task.Delay(500);
                }
                await Task.CompletedTask;
            });
            for (int i = 0; i < 6; i++)
            {
                Console.WriteLine("读取数据……");
                await Task.Delay(500);
                batch.Post($"数据行_{i + 1}");
            }
            batch.Complete(true);
            Console.WriteLine("结束");
        }

        /// <summary>
        /// 阻塞并行单元测试
        /// </summary>
        [Test]
        public void ActionBlockUnit()
        {
            ThreadPool.SetMinThreads(100, 100);
            var block = new ActionBlock<int>(index =>
            {
                Console.WriteLine(index);
                Task.Delay(500);
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
        }

        /// <summary>
        /// 在一堆集合中找出一组连续性集合的位置单元测试
        /// </summary>
        [Test]
        public void BoyerMooreOfUnit()
        {
            string content = File.ReadAllText(Path.Combine(resource, "BoyerMooreUnit.txt"));
            string pattern = "kernel";

            var index = BoyerMoore.BoyerMooreFirstMatch(content.ToCharArray(), pattern.ToCharArray());
        }

        /// <summary>
        /// 深拷贝(性能飘逸)单元测试
        /// </summary>
        [Test]
        public void DeepCopyUnit()
        {
            var tt = typeof(List<DeepCopyModel>);

            //模型Copy
            DeepCopyModel model = new DeepCopyModel();
            model.Field1 = "klsdjflsdlflsdf";
            model.Field2 = 100;
            model.Field3 = 101;
            model.Field4 = 105;
            model.Field5 = 106;
            model.Field6 = 106.45f;
            model.Field7 = 106.46f;
            model.Field8 = 106.4646598d;
            model.Field9 = 106.4646599d;
            model.Field10 = 106.46465996548m;
            model.Field11 = 106.46465996549m;
            model.Field12 = DateTime.Now;
            model.Field13 = DateTime.Now.AddDays(1);
            model.Field14 = TimeSpan.FromSeconds(30);
            model.Field15 = TimeSpan.FromSeconds(40);
            model.Field16 = TestEnum.True;
            model.Field17 = TestEnum.False;
            model.Field18 = 'A';
            model.Field19 = 'B';
            model.model = new DeepCopyModel();
            model.Field20 = new byte[] { 0, 1, 2, 3, 4 };
            model.model.Field1 = "3333333333";
            model.model.Field2 = 9999;

            //var config = new MapperConfiguration(cfg => cfg.CreateMap<DeepCopyModel, DeepCopyModel>());
            //var mapper = config.CreateMapper();
            //var newmodel = mapper.Map<DeepCopyModel>(model);

            var newmodel = model.MapTo<DeepCopyModel, DeepCopyModel>();

            model.model.Field1 = "4444";
        }

        /// <summary>
        /// 使用命令行单元测试
        /// </summary>
        [Test]
        public void RunCmdUnit()
        {
            var str = Kit.RunShell("ipconfig");
        }

        /// <summary>
        /// 操作Ini文件
        /// </summary>
        [Test]
        public void IniMapUnit()
        {
            string path = Path.Combine(resource, "iniTest.ini");
            IniMap iniMap = new IniMap(path);
            iniMap["nember"]["Name"] = "张炳彬";
            iniMap["nember1"]["Name1"] = "张炳彬";
            iniMap.Remove("nember");
            iniMap.Remove("nember1");
            iniMap.Save();
        }

        [Test]
        public void CommonUnit()
        {
            var info = Common.GetXlsxResource("[Content_Types].xml");
        }

        /// <summary>
        /// 生成Excel
        /// </summary>
        [Test]
        public void WriteExcelUnit()
        {
            string path1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WriteExcelUnit.xlsx");
            using (WriteExcelKit writeExcel = new WriteExcelKit(path1))
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();

                dt.Columns.Add("Field1");
                dt.Columns.Add("Field2");
                dt.Columns.Add("Field3");
                dt.Columns.Add("Field4");

                dt.Rows.Add(new object[] { "value1", "value2", "value3", "value4" });
                dt.Rows.Add(new object[] { "value1", "value2", "value3", "value4" });
                dt.Rows.Add(new object[] { "value1", "value2", "value3", "value4" });
                dt.Rows.Add(new object[] { "value1", "value2", "value3", "value4" });
                dt.Rows.Add(new object[] { "value1", "value2", "value3", "value4" });

                ds.Tables.Add(dt);

                DataTable dt1 = new DataTable();

                dt1.Columns.Add("Field5");
                dt1.Columns.Add("Field6");
                dt1.Columns.Add("Field7");
                dt1.Columns.Add("Field8");

                dt1.Rows.Add(new object[] { "value5", "value6", "value7", "value8" });
                dt1.Rows.Add(new object[] { "value5", "value6", "value7", "value8" });
                dt1.Rows.Add(new object[] { "value5", "value6", "value7", "value8" });
                dt1.Rows.Add(new object[] { "value5", "value6", "value7", "value8" });
                dt1.Rows.Add(new object[] { "value5", "value6", "value7", "value8" });

                ds.Tables.Add(dt1);

                writeExcel.Save(ds);
            }
        }

        [Test]
        public void ListAndDataTableUnit()
        {
            List<ListModel> test1 = new List<ListModel>();
            test1.Add(new ListModel() { Name = "张炳彬", Old = 28, Time = DateTime.Parse("1993-08-31") });
            test1.Add(new ListModel() { Name = "安得权", Old = 33, Time = DateTime.Parse("1988-12-24") });
            test1.Add(new ListModel() { Name = "和百东", Old = 28, Time = DateTime.Parse("1990-03-4") });

            var dt = test1.ToDataTable();

            var list = dt.ToEnumerable<ListModel>();
        }

        [Test]
        public void Crc32Unit()
        {
            string data = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<Relationships xmlns=""http://schemas.openxmlformats.org/package/2006/relationships""><Relationship Id=""rId2"" Type=""http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties"" Target=""docProps/core.xml""/><Relationship Id=""rId3"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties"" Target=""docProps/app.xml""/><Relationship Id=""rId1"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument"" Target=""xl/workbook.xml""/></Relationships>";
            byte[] bytes = Encoding.Default.GetBytes(data);
            CRC32 cRC32 = new CRC32();

            var crc32_npoi = cRC32.StringCRC(data);
            var crc32_xlsx = CRCProvider.crc32_buf(data.ToCharArray(), 0);
            var crc32_standard = CRCProvider.Crc32Verify(bytes);
        }

        [Test]
        public void RSAProvider()
        {
            var provider = new RSAProvider<SHA256>();

            var t0 = provider.GetXmlSecret();
            provider.ImportXml(t0.Item1);
            provider.ImportXml(t0.Item2);

            var t1 = provider.GetBase64Secret();
            provider.ImportBase64(t1.Item1, t1.Item2);

            var t2 = provider.GetPemSecret();
            provider.ImportPem(t2.Item1, t2.Item2);
        }

        #region 性能比较单元测试

        [Test]
        public void MiniExcelUnit()
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

        #endregion

        public class ListModel
        {
            public string Name { get; set; }
            public int Old { get; set; }
            public DateTime Time { get; set; }

        }

        /// <summary>
        /// 断言单元测试模型
        /// </summary>
        public struct TestStruct
        {
            public string filed { get; set; }
        }

        /// <summary>
        /// Enum扩展单元测试模型
        /// </summary>
        public enum TestEnum
        {
            None,
            [Display("正确")]
            True,
            [Display("错误")]
            False
        }

        /// <summary>
        /// 校验单元测试模型
        /// </summary>
        public class VerifyModel
        {
            [RequiredRule("DateTimeFieldName", Message = "{0}{3}")]
            public DateTime? DateTimeField { get; set; }

            [StringRule("StrFieldName", MaxLength = 10, Message = "{1}的{3}", Required = true)]
            public string StrField { get; set; }

            [NumericRule("IntFieldName", Equal = 10, Message = "{1}的{3}")]
            public int? IntField { get; set; }

            [DecimalRule("DecimalFieldName", Equal = 10.526, Precision = 3, Message = "{1}的{3}错误")]
            public decimal? DecimalField { get; set; }
        }

        /// <summary>
        /// 实体转XML单元测试模型
        /// </summary>
        public class XmlRoot
        {
            public string attr1 { get; set; }
            public string attr2 { get; set; }
            public List<XmlNode> Node { get; set; }
        }

        /// <summary>
        /// 实体转XML单元测试模型
        /// </summary>
        public class XmlNode
        {
            public string attr3 { get; set; }
            public string attr4 { get; set; }
            public List<XmlField> Field { get; set; }
        }

        /// <summary>
        /// 实体转XML单元测试模型
        /// </summary>
        public class XmlField
        {
            public string attr5 { get; set; }
            public string attr6 { get; set; }
        }

        /// <summary>
        /// 深拷贝单元测试模型
        /// </summary>
        public class DeepCopyModel
        {
            public string Field1 { get; set; }
            public int Field2 { get; set; }
            public int? Field3 { get; set; }
            public long Field4 { get; set; }
            public long? Field5 { get; set; }
            public float Field6 { get; set; }
            public float? Field7 { get; set; }
            public double Field8 { get; set; }
            public double? Field9 { get; set; }
            public decimal Field10 { get; set; }
            public decimal? Field11 { get; set; }
            public DateTime Field12 { get; set; }
            public DateTime? Field13 { get; set; }
            public TimeSpan Field14 { get; set; }
            public TimeSpan? Field15 { get; set; }
            public TestEnum Field16 { get; set; }
            public TestEnum? Field17 { get; set; }
            public char Field18 { get; set; }
            public char? Field19 { get; set; }
            public byte[] Field20 { get; set; }
            public DeepCopyModel model { get; set; }
        }
    }
}