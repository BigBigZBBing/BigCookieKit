using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitConsole
{
    public class BytesIndexOf
    {

        [Benchmark(Description = "IndexOf")]
        public void IndexOf()
        {

        }

        [Benchmark(Description = "FastIndexOf")]
        public void FastIndexOf()
        {

        }
    }
}
