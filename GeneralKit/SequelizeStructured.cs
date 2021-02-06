using ILWheatBread;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace GeneralKit
{
    /// <summary>
    /// 结构化动态类型
    /// </summary>
    public static class SequelizeStructured
    {
        /// <summary>
        /// 层解析结构
        /// </summary>
        private class Tier
        {
            public string Prev { get; set; }
            public string Table { get; set; }
            public string Field { get; set; }
        }

        static List<string> Fields { get; set; }
        static bool First { get; set; }
        static string MainTable { get; set; }
        static Dictionary<string, Type> props { get; set; }
        static SmartBuilder builder { get; set; }
        static IEnumerable<Tier> Table { get; set; }
        static DataTable DataSource { get; set; }
        static Dictionary<string, string> UniqueMapping { get; set; }
        static Dictionary<string, DataTable> CacheDistinctTable { get; set; }

        /// <summary>
        /// 结构化配置
        /// </summary>
        /// <param name="config"></param>
        /// <param name="con"></param>
        /// <param name="parameter"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public static JArray SequelizeConfig(object config, MySqlConnection con, object parameter = null, MySqlTransaction tran = null)
        {
            JArray result = new JArray();
            var commandText = SequelizeToSql(config);
            if (con.State != ConnectionState.Open) con.Open();
            using (var command = con.CreateCommand())
            {
                command.CommandText = commandText;
                if (tran.NotNull()) command.Transaction = tran;
                using (var abapter = new MySqlDataAdapter(command))
                {
                    DataTable tmpDt = new DataTable(MainTable);
                    abapter.Fill(tmpDt);
                    result = tmpDt.SequelizeDynamic();
                }
            }
            return result;
        }

        /// <summary>
        /// 使用SequelizeORM方式解析SQL
        /// </summary>
        /// <param name="config"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static string SequelizeToSql(object config, string sql = "", string prev = "")
        {
            // 判断是否第一次 [2021-1-24 zhangbingbin]
            First = string.IsNullOrEmpty(sql);
            if (First)
            {
                // 初始化 [2021-1-24 zhangbingbin]
                Fields = new List<string>();
                UniqueMapping = new Dictionary<string, string>();
                CacheDistinctTable = new Dictionary<string, DataTable>();
            }
            List<string> cache = new List<string>();

            // 显示的字段名(必须带主键) [2021-1-25 zhangbingbin]
            var fieldsProp = config.GetType().GetProperty("fields");
            if (fieldsProp.IsNull()) throw new ArgumentNullException("fields不可为空 请使用数组格式展示字段");

            // 表名 [2021-1-25 zhangbingbin]
            var modelProp = config.GetType().GetProperty("model");
            if (modelProp.IsNull()) throw new ArgumentNullException("model不可为空 请输入表名");

            // 查询条件 [2021-1-25 zhangbingbin]
            var where = config.GetType().GetProperty("where");

            // 是否左外连接 [2021-1-25 zhangbingbin]
            var required = config.GetType().GetProperty("required");

            // 联合查询配置 [2021-1-25 zhangbingbin]
            var include = config.GetType().GetProperty("include");

            // 限制查询结果数量 [2021-2-6 zhangbingbin]
            var limit = config.GetType().GetProperty("limit");

            // 获取表名参数 [2021-1-24 zhangbingbin]
            string modelStr = modelProp.GetValue(config).ToString();
            if (First) MainTable = modelStr;

            // 获取结果显示字段 [2021-1-23 zhangbingbin]
            var fields = (object[])fieldsProp.GetValue(config);
            foreach (var item in fields)
            {
                string origin = "";
                string show = "";
                if (item is string[] asfield)
                {
                    origin = asfield[0];
                    show = asfield[1];
                }
                else if (item is string field)
                {
                    origin = field;
                    show = field;
                }
                if (First)
                {
                    Fields.Add($"`_{modelStr}`." + $"`{origin}` AS `{show}`");
                }
                else
                {
                    Fields.Add($"`_{modelStr}`." + $"`{origin}` AS `({prev}){modelStr}->{show}`");
                }
            }

            // 获取查询条件 [2021-1-23 zhangbingbin]
            if (where.NotNull())
            {
                cache = SequelizeCriteria(where.GetValue(config), modelStr);
            }

            // 第一次递归 [2021-1-23 zhangbingbin]
            if (First)
            {
                sql = $"SELECT {{0}} FROM `{modelStr}` AS `_{modelStr}`";
            }

            // 关联查询 [2021-1-23 zhangbingbin]
            else
            {
                // 联合关联字段 [2021-1-26 zhangbingbin]
                var join = config.GetType().GetProperty("join");
                if (join.IsNull()) throw new ArgumentNullException("关联模式必须填关联字段");

                var joinObj = join.GetValue(config);
                var keyOn = "";
                foreach (var item in joinObj.GetType().GetProperties())
                {
                    if (keyOn != "") keyOn += "=";
                    var value = item.GetValue(joinObj);
                    keyOn += $"`_{item.Name}`.`{value}`";
                    //缓存每个表对应的Key
                    if (!UniqueMapping.ContainsKey(item.Name))
                        UniqueMapping.Add(item.Name, value.ToString());
                }
                // 如果存在条件又不设置 则转成内连接 [2021-1-24 zhangbingbin]
                if (where.IsNull() || (required.NotNull() && !(bool)required.GetValue(config)))
                {
                    sql += $" LEFT OUTER JOIN `{modelStr}` AS `_{modelStr}` ON {keyOn} ";
                    if (cache.Count > 0) sql += "AND " + string.Join(" AND ", cache);
                }
                else
                {
                    sql += $" INNER JOIN `{modelStr}` AS `_{modelStr}` ON {keyOn} ";
                    if (cache.Count > 0) sql += "AND " + string.Join(" AND ", cache);
                }
            }

            if (include.NotNull())
            {
                var join = include.GetValue(config);
                if (join is Array includes)
                {
                    foreach (var item in includes)
                    {
                        sql = SequelizeToSql(item, sql, modelStr);
                    }
                }
            }
            sql = string.Format(sql, string.Join(",", Fields));

            if (prev == "" && cache.Count > 0)
                sql += " WHERE " + string.Join(" AND ", cache);

            if (limit.NotNull())
            {
                sql += $" LIMIT {limit.GetValue(config)} ";
            }

            return sql;
        }

        /// <summary>
        /// 解析条件
        /// </summary>
        /// <param name="m_where"></param>
        /// <param name="modelStr"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static List<string> SequelizeCriteria(object m_where, string modelStr)
        {
            List<string> cache = new List<string>();
            // 等于 [2021-1-28 zhangbingbin]
            var eq = m_where.GetType().GetProperty("eq");
            if (eq != null) cache.Add(SequelizeWhere(eq.GetValue(m_where), modelStr, "="));
            // 不等于 [2021-1-28 zhangbingbin]
            var ne = m_where.GetType().GetProperty("ne");
            if (ne != null) cache.Add(SequelizeWhere(ne.GetValue(m_where), modelStr, "!="));
            // 大于 [2021-1-28 zhangbingbin]
            var gt = m_where.GetType().GetProperty("gt");
            if (gt != null) cache.Add(SequelizeWhere(gt.GetValue(m_where), modelStr, ">"));
            // 大于等于 [2021-1-28 zhangbingbin]
            var gte = m_where.GetType().GetProperty("gte");
            if (gte != null) cache.Add(SequelizeWhere(gte.GetValue(m_where), modelStr, ">="));
            // 小于 [2021-1-28 zhangbingbin]
            var lt = m_where.GetType().GetProperty("lt");
            if (lt != null) cache.Add(SequelizeWhere(lt.GetValue(m_where), modelStr, "<"));
            // 小于等于 [2021-1-28 zhangbingbin]
            var lte = m_where.GetType().GetProperty("lte");
            if (lte != null) cache.Add(SequelizeWhere(lte.GetValue(m_where), modelStr, "<="));
            // 包含 [2021-1-28 zhangbingbin]
            var In = m_where.GetType().GetProperty("In");
            if (In != null) cache.Add(SequelizeIn(In.GetValue(m_where), modelStr));
            // 不包含 [2021-1-28 zhangbingbin]
            var notIn = m_where.GetType().GetProperty("notIn");
            if (notIn != null) cache.Add(SequelizeNotIn(notIn.GetValue(m_where), modelStr));
            // 与 [2021-1-28 zhangbingbin]
            var and = m_where.GetType().GetProperty("and");
            if (and != null) cache.Add($"({string.Join(" AND ", SequelizeCriteria(and.GetValue(m_where), modelStr))})");
            // 或 [2021-1-28 zhangbingbin]
            var or = m_where.GetType().GetProperty("or");
            if (or != null) cache.Add($"({string.Join(" OR ", SequelizeCriteria(or.GetValue(m_where), modelStr))})");
            // 模糊匹配 [2021-1-28 zhangbingbin]
            var like = m_where.GetType().GetProperty("like");
            if (like != null) cache.Add(SequelizeLike(like.GetValue(m_where), modelStr, "LIKE"));
            // 反向模糊 [2021-1-28 zhangbingbin]
            var notlike = m_where.GetType().GetProperty("notlike");
            if (notlike != null) cache.Add(SequelizeLike(notlike.GetValue(m_where), modelStr, "NOT LIKE"));

            var not = m_where.GetType().GetProperty("not");
            var between = m_where.GetType().GetProperty("between");
            var notBetween = m_where.GetType().GetProperty("notBetween");

            return cache;
        }

        /// <summary>
        /// 解析比较符
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="model"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static string SequelizeWhere(object obj, string model, string symbol = "")
        {
            string result = "";
            var condition = obj.GetType().GetProperties();
            foreach (var item in condition)
            {
                if (result != "") result += " AND ";
                var value = item.GetValue(obj);
                var param = "";
                if (value == null) param = "NULL";
                else if (value is string str) param = $"'{str}'";
                else if (value is int m_int) param = $"{m_int}";
                else if (value is DateTime date) param = $"{date}";
                else param = "NULL";
                if (symbol == "=" && param == "NULL")
                    result += $"`_{model}`.`{item.Name}` IS NULL";
                else if (symbol == "!=" && param == "NULL")
                    result += $"`_{model}`.`{item.Name}` IS NOT NULL";
                else
                    result += $"`_{model}`.`{item.Name}` {symbol} {param}";
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static string SequelizeLike(object obj, string model, string symbol = "")
        {
            string result = "";
            var condition = obj.GetType().GetProperties();
            foreach (var item in condition)
            {
                if (result != "") result += " AND ";
                var value = item.GetValue(obj);
                var param = "";
                if (value == null) param = "NULL";
                else if (value is string
                    && value.ToString().Length > 0
                    && value.ToString()[0] == '@') param = $"{value}";
                else if (value is string str) param = $"'%{str}%'";
                else param = "NULL";
                result += $"`_{model}`.`{item.Name}` {symbol} {param}";
            }
            return result;
        }

        /// <summary>
        /// 解析In
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static string SequelizeIn(object obj, string model)
        {
            string result = "";
            var condition = obj.GetType().GetProperties();
            foreach (var item in condition)
            {
                if (result != "") result += " AND ";
                var arr = item.GetValue(obj);
                var value = "";
                // 判断char类型 [2021-1-23 zhangbingbin]
                if (arr is string[] arr1)
                {
                    foreach (var item1 in arr1)
                    {
                        if (value != "") value += ",";
                        value += $"'{item1}'";
                    }
                }
                // 判断int类型 [2021-1-23 zhangbingbin]
                if (arr is int[] arr2)
                {
                    foreach (var item1 in arr2)
                    {
                        if (value != "") value += ",";
                        value += $"{item1}";
                    }
                }
                // 判断参数化 [2021-1-23 zhangbingbin]
                if (arr is string arr3)
                {
                    value += $"{arr3}";
                }
                if (First)
                    result += $"`_{model}`.`{item.Name}` IN ({value})";
                else
                    result += $"`_{model}`.`{model}->{item.Name}` IN ({value})";
            }

            return result;
        }

        /// <summary>
        /// 解析NotIn
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static string SequelizeNotIn(object obj, string model)
        {
            string result = "";
            var condition = obj.GetType().GetProperties();
            foreach (var item in condition)
            {
                if (result != "") result += " AND ";
                var arr = item.GetValue(obj);
                var value = "";
                // 判断char类型 [2021-1-23 zhangbingbin]
                if (arr is string[] arr1)
                {
                    foreach (var item1 in arr1)
                    {
                        if (value != "") value += ",";
                        value += $"'{item1}'";
                    }
                }
                // 判断int类型 [2021-1-23 zhangbingbin]
                if (arr is int[] arr2)
                {
                    foreach (var item1 in arr2)
                    {
                        if (value != "") value += ",";
                        value += $"{item1}";
                    }
                }
                // 判断参数化 [2021-1-23 zhangbingbin]
                if (arr is string arr3)
                {
                    value += $"{{{arr3}}}";
                }
                if (First)
                    result += $"`_{model}`.`{item.Name}` IN ({value})";
                else
                    result += $"`_{model}`.`{model}->{item.Name}` IN ({value})";
            }

            return result;
        }

        /// <summary>
        /// 生产Sequelize结构化类型
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static JArray SequelizeDynamic(this DataTable dt)
        {
            props = new Dictionary<string, Type>();
            DataSource = dt;

            foreach (DataColumn dc in dt.Columns)
                props.Add(dc.ColumnName, dc.DataType);

            builder = new SmartBuilder("CacheDynamic");
            builder.Assembly();
            builder.Class(dt.TableName);
            // 主架 [2021-1-23 zhangbingbin]
            var main = props.Keys.Where(x => x.IndexOf("->", StringComparison.OrdinalIgnoreCase) == -1);
            foreach (var item in main)
            {
                builder.CreateProperty(item, props[item]);
            }
            // 分割表级 [2021-1-23 zhangbingbin]
            Table = props.Keys.Where(x => x.IndexOf("->", StringComparison.OrdinalIgnoreCase) > -1).Select(x =>
            {
                Tier tier = new Tier();
                int start = x.IndexOf("(", StringComparison.OrdinalIgnoreCase);
                int end = x.IndexOf(")", StringComparison.OrdinalIgnoreCase);
                int talbe = x.IndexOf("->", StringComparison.OrdinalIgnoreCase);
                tier.Prev = x.Substring(start + 1, end - (start + 1));
                tier.Table = x.Substring(end + 1, talbe - (end + 1));
                tier.Field = x.Substring(talbe + 2, x.Length - (talbe + 2));
                return tier;
            });
            var Erji = Table.Where(x => x.Prev == dt.TableName);
            foreach (var item in Erji.Select(x => x.Table).Distinct())
            {
                CreateModel(Erji.Where(x => x.Table == item), builder);
            }
            builder.SaveClass();
            builder.Build();
            var instance = Activator.CreateInstance(typeof(List<>)
                .MakeGenericType(builder.Instance.GetType()));
            LoadDataSource(instance);
            return JArray.FromObject(instance);
        }

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <param name="cols"></param>
        /// <param name="level"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void CreateModel(IEnumerable<Tier> cols, SmartBuilder build)
        {
            var TableName = cols.FirstOrDefault().Table;
            SmartBuilder m_builder = new SmartBuilder("CacheDynamic");
            m_builder.Assembly();
            m_builder.Class("_" + TableName);

            foreach (var item in cols)
            {
                var toType = props[$"({item.Prev}){item.Table}->{item.Field}"];
                m_builder.CreateProperty(item.Field, toType);
            }

            // 是否有子查询 [2021-1-23 zhangbingbin]
            var next = Table.Where(x => x.Prev == TableName);
            foreach (var item in next.Select(x => x.Table).Distinct())
                CreateModel(next.Where(x => x.Table == item), m_builder);

            m_builder.SaveClass();
            m_builder.Build();
            var momeryType = m_builder.Instance.GetType();
            var ListType = typeof(List<>).MakeGenericType(momeryType);
            build.CreateProperty(TableName, ListType);
        }

        /// <summary>
        /// 加载数据源
        /// </summary>
        /// <param name="instance"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void LoadDataSource(object instance, string filter = "")
        {
            var type = (Type)instance.GetType();
            var className = type.Name;
            if (type.Name.Equals("List`1")) className = type.GenericTypeArguments[0].Name;
            PropertyInfo[] props = null;
            object listinstance = null;
            // 如果是链表结构 [2021-1-23 zhangbingbin]
            if (type.IsGenericType)
            {
                listinstance = instance;
                props = type.GetGenericArguments()[0].GetProperties();
            }
            else props = type.GetProperties();

            var cre = "";
            if (className[0].Equals('_')) cre = className.Substring(1) + "->";
            var related = props.Where(x =>
            {
                if (x.PropertyType.Name.Equals("List`1"))
                    return x.PropertyType.GenericTypeArguments[0].Name[0] != '_';
                return x.PropertyType.Name[0] != '_';
            }).Select(x => cre + x.Name);
            List<string> fields = new List<string>();
            if (className[0].Equals('_'))
            {
                foreach (DataColumn dc in DataSource.Columns)
                {
                    var value = related.Count(x => dc.ColumnName.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1);
                    if (value > 0) fields.Add(dc.ColumnName);
                }
            }
            else fields = related.ToList();

            // 获取对应的子集 [2021-2-6 zhangbingbin]
            DataTable table = null;
            if (!CacheDistinctTable.ContainsKey(className))
            {
                CacheDistinctTable.Add(className, DataSource.DefaultView.ToTable(true, fields.ToArray()));
                table = CacheDistinctTable[className];
            }
            else table = CacheDistinctTable[className];

            if (!string.IsNullOrEmpty(filter))
            {
                var drs = table.Select(filter);
                if (drs.Count() > 0)
                    table = drs.CopyToDataTable();
                else
                    table = table.Clone();
            }

            foreach (DataRow dr in table.Rows)
            {
                if (type.IsGenericType)
                    instance = Activator.CreateInstance(type.GetGenericArguments()[0]);

                foreach (var item in props)
                {
                    if (item.PropertyType.Name.Equals("List`1"))
                    {
                        item.SetValue(instance, Activator.CreateInstance(item.PropertyType));
                        filter = $" [({className}){item.Name}->{UniqueMapping[item.Name]}] = '{props.FirstOrDefault(x => x.Name == UniqueMapping[item.Name]).GetValue(instance)}' ";
                        LoadDataSource(item.GetValue(instance), filter);
                    }
                    else
                    {
                        if (className[0].Equals('_'))
                        {
                            var value = dr[fields.FirstOrDefault(x =>
                                            x.IndexOf(className.Substring(1) + "->" + item.Name, StringComparison.OrdinalIgnoreCase) > -1)];
                            item.SetValue(instance, value == DBNull.Value ? null : value);
                        }
                        else
                        {
                            item.SetValue(instance, dr[item.Name] == DBNull.Value ? null : dr[item.Name]);
                        }
                    }
                }
                if (type.IsGenericType && listinstance != null)
                    type.GetMethod("Add").Invoke(listinstance, new object[] { instance });
            }
        }
    }
}
