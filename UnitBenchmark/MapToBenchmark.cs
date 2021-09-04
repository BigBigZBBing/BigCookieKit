using AutoMapper;
using BenchmarkDotNet.Attributes;
using BigCookieKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitBenchmark
{
    public class MapToBenchmark
    {
        public enum TestEnum
        {
            None,
            True,
            False
        }

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

        DeepCopyModel model { get; set; }

        IMapper mapper { get; set; }

        public MapToBenchmark()
        {
            model = new DeepCopyModel();
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

            // AutoMapper没有缓存机制
            var config = new MapperConfiguration(cfg => cfg.CreateMap<DeepCopyModel, DeepCopyModel>());
            mapper = config.CreateMapper();
        }

        [Benchmark]
        public void AutoMapper()
        {
            var newmodel = mapper.Map<DeepCopyModel>(model);
        }

        [Benchmark]
        public void BigCookieMapTo()
        {
            var newmodel = model.MapTo<DeepCopyModel, DeepCopyModel>();
        }
    }
}
