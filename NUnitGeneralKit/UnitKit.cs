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
        public void Test1()
        {
            Assert.Pass();
        }
    }

    public class Model : ICheckVerify
    {
        public Model() { }

        private int? id;
        private string name;
        private long old1;
        private string adress;
        private DateTime? time;

        [Rule("ID", AllowEmpty = false, Error = "{0}不能为空")]
        public int? Id { get => id; set => id = value; }

        [Rule("姓名", ExpType = ExpType.Chinese, Error = "{0}类型必须为中文")]
        public string Name { get => name; set => name = value; }

        [Rule("年龄", MaxLength = 1, Error = "{2}只能为个位数")]
        public int? Old { get; set; }

        [Rule("地址", MinLength = 1, MaxLength = 10, Error = "{2}长度必须在1至10位")]
        public string Adress { get => adress; set => adress = value; }

        [Rule("时间")]
        public DateTime? Time { get => time; set => time = value; }

        public long Old1 { get => old1; set => old1 = value; }
    }
}