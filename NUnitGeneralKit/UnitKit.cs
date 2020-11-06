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

        [Rule("ID", AllowEmpty = false, Error = "{0}����Ϊ��")]
        public int? Id { get => id; set => id = value; }

        [Rule("����", ExpType = ExpType.Chinese, Error = "{0}���ͱ���Ϊ����")]
        public string Name { get => name; set => name = value; }

        [Rule("����", MaxLength = 1, Error = "{2}ֻ��Ϊ��λ��")]
        public int? Old { get; set; }

        [Rule("��ַ", MinLength = 1, MaxLength = 10, Error = "{2}���ȱ�����1��10λ")]
        public string Adress { get => adress; set => adress = value; }

        [Rule("ʱ��")]
        public DateTime? Time { get => time; set => time = value; }

        public long Old1 { get => old1; set => old1 = value; }
    }
}