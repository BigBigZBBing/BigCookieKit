using BigCookieKit.XML;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace BigCookieKit.Office
{
    /// <summary>
    /// Excal读写 只适用xlsx(性能是NPOI的几倍)
    /// </summary>
    public class ReadExcelKit
    {
        /// <summary>
        /// 执行Log
        /// </summary>
        public List<string> ExecuteLog { get; set; }

        /// <summary>
        /// Excel分解成zip文件
        /// </summary>
        private ZipArchive zip { get; set; }

        /// <summary>
        /// 共享字符串数组
        /// </summary>
        private List<string> sharedStrings { get; set; }

        /// <summary>
        /// 单元格样式对应的数据格式
        /// </summary>
        private List<string> cellXfs { get; set; }

        /// <summary>
        /// 数值类型格式
        /// </summary>
        private Dictionary<string, string> numFmts { get; set; }

        /// <summary>
        /// 工作簿
        /// </summary>
        private List<WorkBook> wookbooks { get; set; }

        /// <summary>
        /// Excel配置
        /// </summary>
        private List<ExcelConfig> configs { get; set; }

        /// <summary>
        /// 当前配置
        /// </summary>
        private ExcelConfig current { get; set; }

        private Dictionary<string, XDocument> cache { get; set; }
        private Dictionary<string, XmlPacket> xmlcache { get; set; }

        public ReadExcelKit(string filePath)
        {
            cache = new Dictionary<string, XDocument>();

            xmlcache = new Dictionary<string, XmlPacket>();

            configs = new List<ExcelConfig>();

            if (string.IsNullOrEmpty(filePath)) throw new FileNotFoundException(nameof(filePath));

            ExecuteLog = new List<string>();

            zip = ZipFile.OpenRead(filePath);

            LoadShareString();

            LoadWorkBook();

            LoadNumberFormat();

            LoadCellStyle();
        }

        /// <summary>
        /// 创建Excel配置(DataTable用)
        /// </summary>
        /// <param name="callback"></param>
        [Obsolete("Please invoke AddConfig")]
        public void CreateConfig(Action<ExcelConfig> callback)
        {
            var config = new ExcelConfig();
            config.StartColumnIndex = 0;
            config.StartRow = 0;
            callback.Invoke(config);
            configs.Add(config);
        }

        /// <summary>
        /// 增加配置
        /// </summary>
        /// <param name="callback"></param>
        public void AddConfig(Action<ExcelConfig> callback)
        {
            var config = new ExcelConfig();
            config.StartColumnIndex = 0;
            config.StartRow = 0;
            callback.Invoke(config);
            configs.Add(config);
        }

        /// <summary>
        /// 加载共享字符串
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LoadShareString()
        {
            var entry = zip.GetEntry("xl/sharedStrings.xml");
            if (entry != null)
            {
                var doc = XDocument.Load(entry.Open());

                var elements = doc.Root.Elements();

                if (elements.IsNull()) return;

                sharedStrings = new List<string>();

                foreach (var element in elements)
                {
                    sharedStrings.Add(element.Value);
                }
            }
        }

        /// <summary>
        /// 加载数值类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LoadNumberFormat()
        {
            var entry = zip.GetEntry("xl/styles.xml");
            if (entry != null)
            {
                var doc = XDocument.Load(entry.Open());

                var elements = doc.Root.Elements().FirstOrDefault(x => x.Name.LocalName == "numFmts")?.Elements();

                if (elements.IsNull()) return;

                numFmts = new Dictionary<string, string>();

                foreach (var element in elements)
                {
                    numFmts.Add(element.Attribute("numFmtId").Value,
                      element.Attribute("formatCode").Value);
                }
            }
        }

        /// <summary>
        /// 获取单元格样式
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LoadCellStyle()
        {
            var entry = zip.GetEntry("xl/styles.xml");
            if (entry != null)
            {
                var doc = XDocument.Load(entry.Open());

                var elements = doc.Root.Elements().FirstOrDefault(x => x.Name.LocalName == "cellXfs")?.Elements();

                if (elements.IsNull()) return;

                cellXfs = new List<string>();

                foreach (var element in elements)
                {
                    cellXfs.Add(element.Attribute("numFmtId").Value);
                }
            }
        }

        /// <summary>
        /// 获取工作簿
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LoadWorkBook()
        {
            var entry = zip.GetEntry("xl/workbook.xml");
            if (entry != null)
            {
                var doc = XDocument.Load(entry.Open());

                wookbooks = new List<WorkBook>();

                var elements = doc.Root.Elements().FirstOrDefault(e => e.Name.LocalName == "sheets").Elements();

                if (elements.IsNull()) return;

                foreach (var element in elements)
                {
                    wookbooks.Add(new WorkBook()
                    {
                        SheetId = Int32.Parse(element.Attribute("sheetId").Value),
                        SheetName = element.Attribute("name").Value,
                    });
                }
            }
        }

        /// <summary>
        /// 获取所有工作簿
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<WorkBook> GetWorkBook()
        {
            return wookbooks;
        }

        /// <summary>
        /// 读取数据表
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataTable ReadDataTable()
        {
            try
            {
                DataTable dt = new DataTable();

                current = current ?? configs.FirstOrDefault();

                var sheet = $"sheet{current.SheetIndex}";

                var entry = zip.GetEntry($"xl/worksheets/{sheet}.xml");

                XDocument doc = null;
                if (!cache.ContainsKey(sheet))
                {
                    XmlReader xmlReader = XmlReader.Create(entry.Open());
                    doc = XDocument.Load(xmlReader);
                    cache.Add(sheet, doc);
                }
                else doc = cache[sheet];

                var rows = doc.Root.Elements().FirstOrDefault(e => e.Name.LocalName.Equals("sheetData", StringComparison.OrdinalIgnoreCase));

                bool isBuildColumn = false;

                foreach (var row in rows.Elements())
                {
                    if (!isBuildColumn)
                    {
                        //Excel的行索引
                        var rowIndex = int.Parse(row.Attribute("r")?.Value);
                        //列数字索引
                        int colIndex = current.StartColumnIndex ?? 0;

                        //如果没设置列头行 就取开始行的列数
                        if (current.ColumnNameRow.IsNull() && rowIndex != current.StartRow)
                            continue;

                        foreach (var col in row.Elements())
                        {
                            //列数字索引
                            var colNumIndex = colIndex++;

                            //过滤开始列的索引
                            if (current.StartColumnIndex > colNumIndex)
                                continue;

                            //过滤结束列的索引
                            if (current.EndColumnIndex.NotNull() && current.EndColumnIndex < colNumIndex)
                                continue;

                            //生成列头
                            if (current.ColumnNameRow.IsNull())
                            {
                                isBuildColumn = true;
                                dt.Columns.Add(new DataColumn());
                            }
                            else if (current.ColumnNameRow.NotNull() && current.ColumnNameRow == rowIndex)
                            {
                                isBuildColumn = true;
                                var type = col.Attribute("t")?.Value;
                                dt.Columns.Add(FormColumn(col, type));
                            }
                        }
                    }
                    else
                        break;
                }

                foreach (var row in rows.Elements())
                {
                    //Excel的行索引
                    var rowIndex = int.Parse(row.Attribute("r")?.Value);

                    //过滤开始行的索引
                    if (current.StartRow > rowIndex)
                        continue;

                    //过滤结束行的索引
                    if (current.EndRow.NotNull() && current.EndRow < rowIndex)
                        continue;

                    //列数字索引
                    int colIndex = current.StartColumnIndex ?? 0;

                    List<object> Set = new List<object>();
                    foreach (var col in row.Elements())
                    {
                        //列英文索引
                        var position = col.Attribute("r").Value;
                        var colEnIndex = ExcelHelper.ColumnToIndex(string.Join("", CellPosition(position)));

                        //前面出现其他列 则跳过
                        if (colEnIndex < colIndex) continue;

                        //列数字索引
                        var colNumIndex = colIndex++;

                        //过滤开始列的索引
                        if (current.StartColumnIndex > colNumIndex)
                            continue;

                        //过滤结束列的索引
                        if (current.EndColumnIndex.NotNull() && current.EndColumnIndex < colNumIndex)
                            continue;

                        var type = col.Attribute("t")?.Value;
                        var value = FromValue(col, type);

                        Set.Add(value);
                    }
                    dt.Rows.Add(Set.ToArray());
                }

                return dt;
            }
            catch (Exception ex)
            {
                ExecuteLog.Add($"[ReadDataTable]:[{ex.Message}]");
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataSet ReadDataSet()
        {
            DataSet dataSet = new DataSet();
            foreach (var config in configs)
            {
                current = config;
                try
                {
                    dataSet.Tables.Add(ReadDataTable());
                }
                catch (Exception ex)
                {
                    ExecuteLog.Add($"[ReadDataTable]:[{ex.Message}]");
                }
            }
            return dataSet;
        }

        /// <summary>
        /// 读取数据集合
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete("请使用 XmlReaderSet 读取得到更佳的性能")]
        public IEnumerable<object[]> ReadSet()
        {
            DataTable dt = new DataTable();

            current = current ?? configs.FirstOrDefault();

            var sheet = $"sheet{current.SheetIndex}";

            var entry = zip.GetEntry($"xl/worksheets/{sheet}.xml");

            XDocument doc = null;
            if (!cache.ContainsKey(sheet))
            {
                XmlReader xmlReader = XmlReader.Create(entry.Open());
                doc = XDocument.Load(xmlReader);
                cache.Add(sheet, doc);
            }
            else doc = cache[sheet];

            var rows = doc.Root.Elements().FirstOrDefault(e => e.Name.LocalName == "sheetData");

            foreach (var row in rows.Elements())
            {
                var Set = new List<object>();

                //Excel的行索引
                var rowIndex = int.Parse(row.Attribute("r").Value);

                //过滤开始行的索引
                if (current.StartRow > rowIndex)
                    continue;

                //过滤结束行的索引
                if (current.EndRow.NotNull() && current.EndRow < rowIndex)
                    continue;

                //列数字索引
                int colIndex = current.StartColumnIndex ?? 0;

                foreach (var col in row.Elements())
                {
                    //列数字索引
                    var colNumIndex = colIndex++;

                    //过滤开始列的索引
                    if (current.StartColumnIndex > colNumIndex)
                        continue;

                    //过滤结束列的索引
                    if (current.EndColumnIndex.NotNull() && current.EndColumnIndex < colNumIndex)
                        continue;

                    var type = col.Attribute("t")?.Value;
                    Set.Add(FromValue(col, type));
                }

                yield return Set.ToArray();
            }
        }

        /// <summary>
        /// 获取表头
        /// </summary>
        /// <param name="col"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        DataColumn FormColumn(XElement col, string type)
        {
            try
            {
                var value = FromValue(col, type);
                if (value == DBNull.Value)
                {
                    return new DataColumn();
                }
                return new DataColumn(value.ToString(), value.GetType());
            }
            catch (Exception ex)
            {
                ExecuteLog.Add($"[FormColumn]:[{ex.Message}]");
                return new DataColumn();
            }
        }

        /// <summary>
        /// 根据数据类型获取数据
        /// </summary>
        /// <param name="col"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object FromValue(XElement col, string type)
        {
            try
            {
                //共享字符串
                if (type == "s")
                    return sharedStrings[int.Parse(GetV(col))];
                //字符串
                if (type == "str")
                    return GetV(col);

                //索引数值类型格式
                var style = col.Attribute("s")?.Value;
                if (style.NotNull())
                {
                    var numFmt = cellXfs[int.Parse(style)];
                    var fixedValue = FixedFormat(col, numFmt);
                    //未命中固定样式
                    if (fixedValue.IsNull())
                    {
                        var format = numFmts?[numFmt];
                        return ParseFormat(col, format);
                    }
                    else
                    {
                        return fixedValue;
                    }
                }
            }
            catch (System.Exception ex)
            {
                ExecuteLog.Add($"[FromValue]:[{ex.Message}]");
            }

            return DBNull.Value;
        }

        /// <summary>
        /// 固定的数值格式类型
        /// </summary>
        /// <param name="numFmt"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object FixedFormat(XElement col, string numFmt)
        {
            try
            {
                if (ExcelSSF.FixedNumFmt.ContainsKey(numFmt))
                {
                    var type = ExcelSSF.FixedNumFmt[numFmt];
                    if (type == typeof(DateTime))
                    {
                        return fmtParseDateTime(GetV(col));
                    }
                    else if (GetV(col).TryParse(type, out object value))
                    {
                        return value;
                    }
                }
            }
            catch (System.Exception ex)
            {
                ExecuteLog.Add($"[FixedFormat]:[{ex.Message}]");
            }

            return null;
        }

        /// <summary>
        /// 转换数值格式类型
        /// </summary>
        /// <param name="col"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object ParseFormat(XElement col, string format)
        {
            try
            {
                //判断是否是浮点数
                if (format.AllOwn("0", "0."))
                {
                    if (GetV(col).TryParse<decimal>(out object value))
                    {
                        return value;
                    }
                }
                //判断是否是整数型
                if (format.AllOwn("0"))
                {
                    if (GetV(col).TryParse<int>(out object value1))
                    {
                        return value1;
                    }
                    if (GetV(col).TryParse<long>(out object value2))
                    {
                        return value2;
                    }
                }
                //判断是否是日期格式
                if (format.AllOwn("y", "m") || format.AllOwn("m", "d"))
                {
                    return fmtParseDateTime(GetV(col));
                }
                //判断是否是时间格式
                if (format.AllOwn("h", "m"))
                {
                    if (GetV(col).TryParse<TimeSpan>(out object value))
                    {
                        return value;
                    }
                }
            }
            catch (System.Exception ex)
            {
                ExecuteLog.Add($"[ParseFormat]:[{ex.Message}]");
            }

            return DBNull.Value;
        }

        /// <summary>
        /// 获取列英文索引
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerable<char> CellPosition(string position)
        {
            for (int i = 0; i < position.Length; i++)
            {
                if (position[i] >= 'A' && position[i] <= 'Z')
                    yield return position[i];
            }
        }

        /// <summary>
        /// 获取结果值
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        string GetV(XElement element)
        {
            return element.Element(XName.Get("v", "http://schemas.openxmlformats.org/spreadsheetml/2006/main"))?.Value;
        }

        #region XmlReader方案

        /// <summary>
        /// 读取数据表
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataTable XmlReadDataTable()
        {
            try
            {
                DataTable dt = new DataTable();

                current = current ?? configs.FirstOrDefault();

                var sheet = $"sheet{current.SheetIndex}";

                var entry = zip.GetEntry($"xl/worksheets/{sheet}.xml");

                XmlPacket packet = null;
                if (!cache.ContainsKey(sheet))
                {
                    XmlReadKit xmlReadKit = new XmlReadKit(entry.Open());
                    packet = xmlReadKit.Read("sheetData");
                    xmlcache.Add(sheet, packet);
                }
                else packet = xmlcache[sheet];

                bool isBuildColumn = false;

                foreach (var row in packet.Node)
                {
                    if (!isBuildColumn)
                    {
                        //Excel的行索引
                        var rowIndex = int.Parse(row.GetAttr("r").Text);
                        //列数字索引
                        int colIndex = current.StartColumnIndex ?? 0;

                        //如果没设置列头行 就取开始行的列数
                        if (current.ColumnNameRow.IsNull() && rowIndex != current.StartRow)
                            continue;

                        foreach (var col in row.Node)
                        {
                            //列英文索引
                            //var position = col.Attribute("r").Value;
                            //var colEnIndex = CellPosition(position).ToString();

                            //列数字索引
                            var colNumIndex = colIndex++;

                            //过滤开始列的索引
                            if (current.StartColumnIndex > colNumIndex)
                                continue;

                            //过滤结束列的索引
                            if (current.EndColumnIndex.NotNull() && current.EndColumnIndex < colNumIndex)
                                continue;

                            //生成列头
                            if (current.ColumnNameRow.IsNull())
                            {
                                isBuildColumn = true;
                                dt.Columns.Add(new DataColumn());
                            }
                            else if (current.ColumnNameRow.NotNull() && current.ColumnNameRow == rowIndex)
                            {
                                isBuildColumn = true;
                                var type = col.GetAttr("t").Text;
                                dt.Columns.Add(FormColumn(col, type));
                            }
                        }
                    }
                    else
                        break;
                }

                foreach (var row in packet.Node)
                {
                    //Excel的行索引
                    var rowIndex = int.Parse(row.GetAttr("r").Text);

                    //过滤开始行的索引
                    if (current.StartRow > rowIndex)
                        continue;

                    //过滤结束行的索引
                    if (current.EndRow.NotNull() && current.EndRow < rowIndex)
                        continue;

                    //列数字索引
                    int colIndex = current.StartColumnIndex ?? 0;

                    List<object> Set = new List<object>();
                    foreach (var col in row.Node)
                    {
                        //列英文索引
                        var position = col.GetAttr("r").Text;
                        var colEnIndex = ExcelHelper.ColumnToIndex(string.Join("", CellPosition(position)));

                        //前面出现其他列 则跳过
                        if (colEnIndex < colIndex) continue;

                        //列数字索引
                        var colNumIndex = colIndex++;

                        //过滤开始列的索引
                        if (current.StartColumnIndex > colNumIndex)
                            continue;

                        //过滤结束列的索引
                        if (current.EndColumnIndex.NotNull() && current.EndColumnIndex < colNumIndex)
                            continue;

                        var type = col.GetAttr("t").Text;
                        var value = FromValue(col, type);

                        Set.Add(value);
                    }
                    dt.Rows.Add(Set.ToArray());
                }

                return dt;
            }
            catch (Exception ex)
            {
                ExecuteLog.Add($"[ReadDataTable]:[{ex.Message}]");
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<object[]> XmlReaderSet()
        {
            DataTable dt = new DataTable();

            current = current ?? configs.FirstOrDefault();

            var sheet = $"sheet{current.SheetIndex}";

            var entry = zip.GetEntry($"xl/worksheets/{sheet}.xml");

            XmlPacket packet = null;
            if (!cache.ContainsKey(sheet))
            {
                XmlReadKit xmlReadKit = new XmlReadKit(entry.Open());
                packet = xmlReadKit.Read("sheetData");
                xmlcache.Add(sheet, packet);
            }
            else packet = xmlcache[sheet];

            foreach (var row in packet.Node)
            {
                var Set = new List<object>();

                //Excel的行索引
                var rowIndex = int.Parse(row.GetAttr("r").Text);

                //过滤开始行的索引
                if (current.StartRow > rowIndex)
                    continue;

                //过滤结束行的索引
                if (current.EndRow.NotNull() && current.EndRow < rowIndex)
                    continue;

                //列数字索引
                int colIndex = current.StartColumnIndex ?? 0;

                foreach (var col in row.Node)
                {
                    //列数字索引
                    var colNumIndex = colIndex++;

                    //过滤开始列的索引
                    if (current.StartColumnIndex > colNumIndex)
                        continue;

                    //过滤结束列的索引
                    if (current.EndColumnIndex.NotNull() && current.EndColumnIndex < colNumIndex)
                        continue;

                    var type = col.GetAttr("t").Text;
                    Set.Add(FromValue(col, type));
                }

                yield return Set.ToArray();
            }
        }

        /// <summary>
        /// 获取表头
        /// </summary>
        /// <param name="col"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        DataColumn FormColumn(XmlPacket col, string type)
        {
            try
            {
                var value = FromValue(col, type);
                if (value == DBNull.Value)
                {
                    return new DataColumn();
                }
                return new DataColumn(value.ToString(), value.GetType());
            }
            catch (Exception ex)
            {
                ExecuteLog.Add($"[FormColumn]:[{ex.Message}]");
                return new DataColumn();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object FromValue(XmlPacket col, string type)
        {
            try
            {
                //共享字符串
                if (type == "s")
                    return sharedStrings[int.Parse(GetV(col))];
                //字符串
                if (type == "str")
                    return GetV(col);

                //索引数值类型格式
                var style = col.GetAttr("s").Text;
                if (style.NotNull())
                {
                    var numFmt = cellXfs[int.Parse(style)];
                    var fixedValue = FixedFormat(col, numFmt);
                    //未命中固定样式
                    if (fixedValue.IsNull())
                    {
                        var format = numFmts?[numFmt];
                        return ParseFormat(col, format);
                    }
                    else
                    {
                        return fixedValue;
                    }
                }
            }
            catch (System.Exception ex)
            {
                ExecuteLog.Add($"[FromValue]:[{ex.Message}]");
            }

            return DBNull.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object FixedFormat(XmlPacket col, string numFmt)
        {
            try
            {
                if (ExcelSSF.FixedNumFmt.ContainsKey(numFmt))
                {
                    var type = ExcelSSF.FixedNumFmt[numFmt];
                    if (type == typeof(DateTime))
                    {
                        return fmtParseDateTime(GetV(col));
                    }
                    else if (GetV(col).TryParse(type, out object value))
                    {
                        return value;
                    }
                }
            }
            catch (System.Exception ex)
            {
                ExecuteLog.Add($"[FixedFormat]:[{ex.Message}]");
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object ParseFormat(XmlPacket col, string format)
        {
            try
            {
                //判断是否是浮点数
                if (format.AllOwn("0", "0."))
                {
                    if (GetV(col).TryParse<decimal>(out object value))
                    {
                        return value;
                    }
                }
                //判断是否是整数型
                if (format.AllOwn("0"))
                {
                    if (GetV(col).TryParse<int>(out object value1))
                    {
                        return value1;
                    }
                    if (GetV(col).TryParse<long>(out object value2))
                    {
                        return value2;
                    }
                }
                //判断是否是日期格式
                if (format.AllOwn("y", "m") || format.AllOwn("m", "d"))
                {
                    return fmtParseDateTime(GetV(col));
                }
                //判断是否是时间格式
                if (format.AllOwn("h", "m"))
                {
                    return fmtParseDateTime(GetV(col));
                }
            }
            catch (System.Exception ex)
            {
                ExecuteLog.Add($"[ParseFormat]:[{ex.Message}]");
            }

            return DBNull.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        string GetV(XmlPacket element)
        {
            return element.Node.FirstOrDefault(x => x.Info.Name.Equals("v", StringComparison.OrdinalIgnoreCase)).Info.Text;
        }

        #endregion

        #region SSF处理

        DateTime excelDateTime = DateTime.Parse("1900-01-01");
        /// <summary>
        /// Excel的时间处理
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        DateTime fmtParseDateTime(string value)
        {
            return excelDateTime.AddDays(double.Parse(value));
        }

        #endregion
    }
}