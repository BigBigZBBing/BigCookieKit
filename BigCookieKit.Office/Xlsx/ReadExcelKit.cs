using BigCookieKit.Reflect;
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

namespace BigCookieKit.Office.Xlsx
{
    /// <summary>
    /// Excal读写 只适用xlsx(性能是NPOI的几倍)
    /// </summary>
    public class ReadExcelKit : XmlReadKit
    {
        /// <summary>
        /// 执行Log
        /// </summary>
        public List<string> ExecuteLog { get; set; } = new List<string>();

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
        private List<ExcelConfig> configs { get; set; } = new List<ExcelConfig>();

        /// <summary>
        /// 当前配置
        /// </summary>
        private ExcelConfig current { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        private ReadExcelKit()
        {
            LoadShareString();

            LoadWorkBook();

            LoadNumberFormat();

            LoadCellStyle();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filePath">Excel地址</param>
        public ReadExcelKit(string filePath) : base()
        {
            if (string.IsNullOrEmpty(filePath)) throw new FileNotFoundException(nameof(filePath));

            zip = ZipFile.OpenRead(filePath);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stream">Excel文件流</param>
        public ReadExcelKit(Stream stream) : base()
        {
            stream.Seek(0, SeekOrigin.Begin);
            zip = new ZipArchive(stream);
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
        /// 获取列英文索引
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private IEnumerable<char> CellPosition(string position)
        {
            for (int i = 0; i < position.Length; i++)
            {
                if (position[i] >= 'A' && position[i] <= 'Z')
                    yield return position[i];
            }
        }

        /// <summary>
        /// 获取所有工作簿
        /// </summary>
        /// <returns></returns>
        public List<WorkBook> GetWorkBook()
        {
            return wookbooks;
        }

        /// <summary>
        /// 获取数据表集合
        /// </summary>
        /// <returns></returns>
        public DataSet XmlReadDataSet()
        {
            DataSet dataSet = new DataSet();
            foreach (var config in configs)
            {
                current = config;
                try
                {
                    dataSet.Tables.Add(XmlReadDataTable());
                }
                catch (Exception ex)
                {
                    ExecuteLog.Add($"[ReadDataTable]:[{ex.Message}]");
                }
            }
            return dataSet;
        }

        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <returns></returns>
        public DataTable XmlReadDataTable()
        {
            current = current ?? configs.FirstOrDefault();

            var sheet = $"sheet{current.SheetIndex}";

            var entry = zip.GetEntry($"xl/worksheets/{sheet}.xml");

            DataTable dt = new DataTable();
            DataRow ndr = default;
            IDictionary<int, string> Columns = new Dictionary<int, string>();

            var changing = new List<Action<DataRowChangeEventArgs>>();
            if (current.ColumnSetting != null)
            {
                foreach (var item in current.ColumnSetting)
                {
                    var dc = dt.Columns.Add(item.ColumnName, item.ColumnType);
                    if (item.Column != null)
                        Columns.Add(item.ColumnIndex.Value, item.ColumnName);
                    dc.AllowDBNull = item.IsAllowNull;
                    switch (item.NormalType)
                    {
                        case ColumnNormal.Guid:
                            dc.DefaultValue = Guid.NewGuid();
                            changing.Add(e => { dc.DefaultValue = Guid.NewGuid(); });
                            break;
                        case ColumnNormal.NowDate:
                            dc.DefaultValue = DateTime.Now;
                            break;
                        case ColumnNormal.Increment:
                            int increment = item.DefaultValue == null ? 0 : Convert.ToInt32(item.DefaultValue);
                            dc.DefaultValue = increment;
                            changing.Add(e => { dc.DefaultValue = ++increment; });
                            break;
                        case ColumnNormal.Default:
                            if (item.DefaultValue != null) dc.DefaultValue = item.DefaultValue;
                            break;
                    }
                }
                dt.RowChanging += RowChanging;
            }

            void RowChanging(object sender, DataRowChangeEventArgs e)
            {
                if (e.Action == DataRowAction.Add)
                {
                    foreach (var item in changing)
                    {
                        item(e);
                    }
                }
            }

            XmlReadKit xmlReadKit = new XmlReadKit(entry.Open());
            bool readColumns = false;
            bool readData = false;
            bool isValue = false;

            int rowIndex = default;
            int colIndex = default;
            string dataType = default;
            string xfs = default;
            xmlReadKit.XmlReadXlsx("sheetData", (node, attrs, content) =>
                {
                    switch (node)
                    {
                        case "end":
                            if (ndr != null) dt.Rows.Add(ndr);
                            break;
                        case "row":
                            rowIndex = int.Parse(attrs.SingleOrDefault(x => x.Name.Equals("r", StringComparison.OrdinalIgnoreCase)).Text);
                            if (current.StartRow <= current.ColumnNameRow) throw new XlsxRowConfigException();
                            if (current.ColumnSetting == null
                            && rowIndex == current.ColumnNameRow)
                                readColumns = true;
                            if (rowIndex > current.EndRow)
                                return false;
                            if (rowIndex >= current.StartRow)
                            {
                                readColumns = false;
                                readData = true;
                                if (ndr != null) dt.Rows.Add(ndr);
                                ndr = dt.NewRow();
                            }
                            break;
                        case "c":
                            if (readColumns || readData)
                            {
                                string colEn = new string(CellPosition(attrs.SingleOrDefault(x => x.Name.Equals("r", StringComparison.OrdinalIgnoreCase)).Text).ToArray());
                                colIndex = ExcelHelper.ColumnToIndex(colEn).Value;
                                dataType = attrs.FirstOrDefault(x => x.Name.Equals("t", StringComparison.OrdinalIgnoreCase)).Text;
                                xfs = attrs.FirstOrDefault(x => x.Name.Equals("s", StringComparison.OrdinalIgnoreCase)).Text;
                            }
                            break;
                        case "v":
                            if (colIndex >= current.StartColumnIndex
                            && (current.EndColumnIndex != null ? colIndex <= current.EndColumnIndex : true))
                                isValue = true;
                            break;
                        case "f":
                            break;
                        case "text":
                            if (isValue)
                            {
                                if (readColumns)
                                {
                                    if (dataType == "s")
                                    {
                                        dt.Columns.Add(sharedStrings[int.Parse(content)], typeof(string));
                                        Columns.Add(colIndex, sharedStrings[int.Parse(content)]);
                                    }
                                    else
                                    {
                                        dt.Columns.Add(content, typeof(string));
                                        Columns.Add(colIndex, content);
                                    }
                                }
                                if (readData)
                                {
                                    if (Columns.ContainsKey(colIndex))
                                        if (dataType == "s")
                                            ndr[Columns[colIndex]] = sharedStrings[int.Parse(content)];
                                        else
                                            ndr[Columns[colIndex]] = content;
                                }
                                isValue = false;
                            }
                            break;
                        default:
                            isValue = false;
                            break;
                    }
                    return true;
                });

            dt.RowChanging -= RowChanging;

            return dt;
        }

        /// <summary>
        /// 获取数据对象集合
        /// </summary>
        /// <returns></returns>
        public IEnumerable<object[]> XmlReaderSet()
        {
            current = current ?? configs.FirstOrDefault();

            var sheet = $"sheet{current.SheetIndex}";

            var entry = zip.GetEntry($"xl/worksheets/{sheet}.xml");

            List<object[]> list = new List<object[]>();
            List<object> temp = null;

            XmlReadKit xmlReadKit = new XmlReadKit(entry.Open());
            bool readData = false;
            bool isValue = false;

            int rowIndex = default;
            int colIndex = default;
            string dataType = default;
            string xfs = default;
            xmlReadKit.XmlReadXlsx("sheetData", (node, attrs, content) =>
            {
                switch (node)
                {
                    case "end":
                        if (temp != null) list.Add(temp.ToArray());
                        break;
                    case "row":
                        rowIndex = int.Parse(attrs.SingleOrDefault(x => x.Name.Equals("r", StringComparison.OrdinalIgnoreCase)).Text);
                        if (rowIndex > current.EndRow)
                            return false;
                        if (rowIndex >= current.StartRow)
                        {
                            readData = true;
                            if (temp != null) list.Add(temp.ToArray());
                            temp = new List<object>();
                        }
                        break;
                    case "c":
                        if (readData)
                        {
                            string colEn = new string(CellPosition(attrs.SingleOrDefault(x => x.Name.Equals("r", StringComparison.OrdinalIgnoreCase)).Text).ToArray());
                            colIndex = ExcelHelper.ColumnToIndex(colEn).Value;
                            dataType = attrs.FirstOrDefault(x => x.Name.Equals("t", StringComparison.OrdinalIgnoreCase)).Text;
                            xfs = attrs.FirstOrDefault(x => x.Name.Equals("s", StringComparison.OrdinalIgnoreCase)).Text;
                        }
                        break;
                    case "v":
                        if (colIndex >= current.StartColumnIndex
                        && (current.EndColumnIndex != null ? colIndex <= current.EndColumnIndex : true))
                            isValue = true;
                        break;
                    case "f":
                        break;
                    case "text":
                        if (isValue)
                        {
                            if (readData)
                            {
                                if (dataType == "s")
                                    temp.Add(sharedStrings[int.Parse(content)]);
                                else
                                    temp.Add(content);
                            }
                            isValue = false;
                        }
                        break;
                    default:
                        isValue = false;
                        break;
                }
                return true;
            });

            return list;
        }

        /// <summary>
        /// 获取数据字典集合
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IDictionary<string, object>> XmlReaderDictionary()
        {
            current = current ?? configs.FirstOrDefault();

            var sheet = $"sheet{current.SheetIndex}";

            var entry = zip.GetEntry($"xl/worksheets/{sheet}.xml");

            List<IDictionary<string, object>> dicList = new List<IDictionary<string, object>>();
            IDictionary<int, string> Columns = new Dictionary<int, string>();
            IDictionary<string, object> temp = null;

            XmlReadKit xmlReadKit = new XmlReadKit(entry.Open());
            bool readColumns = false;
            bool readData = false;
            bool isValue = false;

            int rowIndex = default;
            int colIndex = default;
            string dataType = default;
            string xfs = default;
            xmlReadKit.XmlReadXlsx("sheetData", (node, attrs, content) =>
            {
                switch (node)
                {
                    case "end":
                        if (temp != null) dicList.Add(temp);
                        break;
                    case "row":
                        rowIndex = int.Parse(attrs.SingleOrDefault(x => x.Name.Equals("r", StringComparison.OrdinalIgnoreCase)).Text);
                        if (current.StartRow <= current.ColumnNameRow) throw new XlsxRowConfigException();
                        if (rowIndex == current.ColumnNameRow)
                            readColumns = true;
                        if (rowIndex > current.EndRow)
                            return false;
                        if (rowIndex >= current.StartRow)
                        {
                            readColumns = false;
                            readData = true;
                            if (temp != null) dicList.Add(temp);
                            temp = new Dictionary<string, object>();
                        }
                        break;
                    case "c":
                        if (readColumns || readData)
                        {
                            string colEn = new string(CellPosition(attrs.SingleOrDefault(x => x.Name.Equals("r", StringComparison.OrdinalIgnoreCase)).Text).ToArray());
                            colIndex = ExcelHelper.ColumnToIndex(colEn).Value;
                            dataType = attrs.FirstOrDefault(x => x.Name.Equals("t", StringComparison.OrdinalIgnoreCase)).Text;
                            xfs = attrs.FirstOrDefault(x => x.Name.Equals("s", StringComparison.OrdinalIgnoreCase)).Text;
                        }
                        break;
                    case "v":
                        if (colIndex >= current.StartColumnIndex
                        && (current.EndColumnIndex != null ? colIndex <= current.EndColumnIndex : true))
                            isValue = true;
                        break;
                    case "f":
                        break;
                    case "text":
                        if (isValue)
                        {
                            if (readColumns)
                            {
                                if (dataType == "s")
                                {
                                    Columns.Add(colIndex, sharedStrings[int.Parse(content)]);
                                }
                                else
                                {
                                    Columns.Add(colIndex, content);
                                }
                            }
                            if (readData)
                            {
                                if (dataType == "s")
                                {
                                    temp.Add(Columns[colIndex], sharedStrings[int.Parse(content)]);
                                }
                                else
                                {
                                    temp.Add(Columns[colIndex], content);
                                }
                            }
                            isValue = false;
                        }
                        break;
                    default:
                        isValue = false;
                        break;
                }
                return true;
            });

            return dicList;
        }

    }
}