using BenchmarkDotNet.Attributes;
using BigCookieKit.Reflect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UnitConsole
{
    public class Model
    {
        public string field1 { get; set; }
        public int? field2 { get; set; }
        public decimal? field3 { get; set; }
        public DateTime? field4 { get; set; }
    }

    public class ReflectCompare
    {
        public static Func<Model> tree { get; set; }
        public static Func<Model> emit { get; set; }

        [Benchmark(Description = "ExpressionTree")]
        public void ExpressionTree()
        {
            if (tree != null)
            {
                tree?.Invoke();
                return;
            }
            var labelTarget = Expression.Label(typeof(Model));
            var variable = Expression.Variable(typeof(Model));
            var entity = Expression.Assign(variable, Expression.New(typeof(Model).GetConstructor(Type.EmptyTypes)));
            var field1 = typeof(Model).GetProperty("field1");
            var field1Prop = Expression.Property(variable, field1);
            var assign1 = Expression.Assign(field1Prop, Expression.Constant("测试数据"));
            var field2 = typeof(Model).GetProperty("field2");
            var field2Prop = Expression.Property(variable, field2);
            var assign2 = Expression.Assign(field2Prop, Expression.Constant(1000, typeof(int?)));
            var field3 = typeof(Model).GetProperty("field3");
            var field3Prop = Expression.Property(variable, field3);
            var assign3 = Expression.Assign(field3Prop, Expression.Constant(13165M, typeof(decimal?)));
            var field4 = typeof(Model).GetProperty("field4");
            var field4Prop = Expression.Property(variable, field4);
            var assign4 = Expression.Assign(field4Prop, Expression.Constant(DateTime.Now, typeof(DateTime?)));
            var block = Expression.Block(
                new ParameterExpression[] { variable },
                entity,
                assign1,
                assign2,
                assign3,
                assign4,
                variable);
            var deleg = Expression.Lambda<Func<Model>>(block);
            tree = deleg.Compile();
            tree?.Invoke();
        }

        [Benchmark(Description = "EmitTool")]
        public void EmitTool()
        {
            if (emit != null)
            {
                emit?.Invoke();
                return;
            }
            emit = SmartBuilder.DynamicMethod<Func<Model>>(string.Empty, il =>
            {
                var model = il.NewEntity<Model>();
                model["field1"] = il.String("测试数据");
                model["field2"] = il.NewInt32(1000).AsNullable();
                model["field3"] = il.NewDecimal(13165M).AsNullable();
                model["field4"] = il.NewDateTime().AsNullable();
                model.Output();
                il.Return();
            });
            emit?.Invoke();
        }
    }
}
