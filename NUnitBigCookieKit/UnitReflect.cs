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

        [Test]
        public void ExtendObject()
        {
            FastExtend canExtend = new FastExtend();
            canExtend.AddMember("Name1", typeof(string));
            canExtend.AddMember("Name2", typeof(string));
            canExtend.AddMember("Name3", typeof(string));
            canExtend["Name1"] = "123";
            canExtend.AddMember("Name4", typeof(string));
            canExtend["Name4"] = "123";
        }
    }
}
