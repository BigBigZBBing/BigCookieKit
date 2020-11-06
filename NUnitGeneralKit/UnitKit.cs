using GeneralKit;
using NUnit.Framework;
using System;

namespace NUnitGeneralKit
{
    public class UnitKit
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void VerifyUnit()
        {
            TestModel testModel = new TestModel();
            testModel.Id = 100;//不填通不过
            //testModel.Name = "张";//通不过
            //testModel.Name = "张三";//通过
            //testModel.Name = "张as";//通不过
            //testModel.Name = "张三三";//通过
            //testModel.Name = "张三三三三";//通不过
            testModel.Old = 1;//通过
            //testModel.Old = 12;//通不过
            testModel.Old1 = 10;//通过
            //testModel.Old1 = 8;//通不过
            testModel.Old2 = 8;//通过
            testModel.Old2 = 7;//通不过

            testModel.ModelValidation();
            Assert.IsTrue(true);
        }
    }

    public class TestModel : ICheckVerify
    {
        public TestModel() { }

        private int? id;
        private string name;
        private long old1;
        private long old2;
        private string adress;
        private DateTime? time;

        [Rule("ID", AllowEmpty = false, Error = "{0}不能为空")]
        public int? Id { get => id; set => id = value; }

        [Rule("姓名", MinLength = 2, MaxLength = 4, ExpType = ExpType.Chinese, Error = "{2}类型必须为中文")]
        public string Name { get => name; set => name = value; }

        [Rule("年龄", Greater = 9, Error = "{0}不能大于9")]
        public int? Old { get; set; }

        [Rule("年龄", Less = 9, Error = "{0}不能小于9")]
        public long Old1 { get => old1; set => old1 = value; }

        [Rule("年龄", Equal = 7, Error = "{0}不能等于7")]
        public long Old2 { get => old2; set => old2 = value; }

        [Rule("地址", MinLength = 1, MaxLength = 10, Error = "{2}长度必须在1至10位")]
        public string Adress { get => adress; set => adress = value; }

        [Rule("时间")]
        public DateTime? Time { get => time; set => time = value; }

    }
}