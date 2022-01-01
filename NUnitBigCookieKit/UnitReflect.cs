using BigCookieKit;
using BigCookieKit.Reflect;

using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnitBigCookieKit
{
    public class UnitReflect
    {

        [SetUp]
        public void Setup()
        {

        }

        /// <summary>
        /// 高性能动态扩展对象
        /// </summary>
        [Test]
        public void ExtendObject()
        {
            FastExtend canExtend = new FastExtend();
            canExtend.AddMember("Name1", typeof(string));
            canExtend.AddMember("Name2", typeof(string));
            canExtend.AddMember("Name3", typeof(string));
            canExtend["Name1"] = "123";
            for (int i = 4; i < 20; i++)
            {
                var name = $"Name" + i;
                canExtend.AddMember(name, typeof(string));
                canExtend[name] = "123";
            }
            canExtend.RemoveMember("Name2");
        }
    }
}
