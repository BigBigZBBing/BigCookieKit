using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace GeneralKit
{
    /// <summary>
    /// NPOI工具箱
    /// </summary>
    public class NpoiKit
    {
        /// <summary>
        /// Excal类型
        /// </summary>
        public enum ExcalType
        {
            /// <summary>
            /// 2003版本
            /// </summary>
            XLS,
            /// <summary>
            /// 2007版本
            /// </summary>
            XLSX
        }

        /// <summary>
        /// 数值类型
        /// </summary>
        public enum NumericClassfiy
        {
            /// <summary>
            /// 整数
            /// </summary>
            Integer,
            /// <summary>
            /// 数值
            /// </summary>
            Numeric,
            /// <summary>
            /// 货币
            /// </summary>
            Money,
            /// <summary>
            /// 会计专用
            /// </summary>
            Accountant
        }

        /// <summary>
        /// 26个序列英文字母
        /// </summary>
        private char[] sequence = new[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        /// <summary>
        /// 异常日志
        /// </summary>
        public List<string> ErrorLog { get; set; } = new List<string>();

        /// <summary>
        /// 工作簿
        /// </summary>
        private IWorkbook workbook { get; set; }

        /// <summary>
        /// 不适用Excel的列名 自动列名 例:Colume1 Colume2
        /// </summary>
        public bool AuthColumnName { get; set; }

        /// <summary>
        /// 开始的行 跟Excal左边对应
        /// </summary>
        public int? StartRow { get; set; } = 1;

        /// <summary>
        /// 结束的行
        /// </summary>
        public int? EndRow { get; set; }

        /// <summary>
        /// 开始的列 跟Excal上边对应
        /// </summary>
        private string _StartColumn;
        public string StartColumn
        {
            get
            {
                return _StartColumn;
            }
            set
            {
                StartColumnIndex = ColumnToIndex(value).GetValueOrDefault();
                _StartColumn = value;
            }
        }

        /// <summary>
        /// 开始的列索引
        /// </summary>
        public int StartColumnIndex { get; set; }

        /// <summary>
        /// 结束的列
        /// </summary>
        private string _EndColumn;
        public string EndColumn
        {
            get
            {
                return _EndColumn;
            }
            set
            {
                EndColumnIndex = ColumnToIndex(value).GetValueOrDefault();
                _EndColumn = value;
            }
        }

        /// <summary>
        /// 结束的列索引
        /// </summary>
        public int EndColumnIndex { get; set; }

        /// <summary>
        /// 配置Excal转DataTable的矩阵
        /// </summary>
        public string ImportMatrix { get; set; }

        /// <summary>
        /// 配置DataTable转Excal的矩阵
        /// </summary>
        public string ExportMatrix { get; set; }

        /// <summary>
        /// 所有单元格直接获取公式结果
        /// </summary>
        public bool CellForceFormula { get; set; }

        public NpoiKit(ExcalType type)
        {
            switch (type)
            {
                case ExcalType.XLS:
                    workbook = new HSSFWorkbook();
                    break;
                case ExcalType.XLSX:
                    workbook = new XSSFWorkbook();
                    break;
            }
        }

        public NpoiKit(string excelPath)
        {
            string fileType = Path.GetExtension(excelPath);
            using (var fs = new FileStream(excelPath, FileMode.Open, FileAccess.Read))
            {
                switch (fileType)
                {
                    case ".xls":
                        workbook = new HSSFWorkbook(fs);
                        break;
                    case ".xlsx":
                        workbook = new XSSFWorkbook(fs);
                        break;
                    default:
                        throw new Exception();
                }
            }
        }

        /// <summary>
        /// 获取所有的Sheet
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ISheet> GetSheetAll()
        {
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                var name = workbook.GetSheetName(i);
                yield return workbook.GetSheet(name);
            }
        }

        /// <summary>
        /// 根据名称获取Sheet
        /// </summary>
        /// <param name="name">Sheet名称</param>
        /// <returns></returns>
        public ISheet GetSheet(string name)
        {
            return workbook.GetSheet(name);
        }

        /// <summary>
        /// 强制刷新公式结果
        /// </summary>
        public void ForceEvaluatorAll()
        {
            workbook.GetCreationHelper().CreateFormulaEvaluator().EvaluateAll();
        }

        /// <summary>
        /// 获取单元格值
        /// </summary>
        /// <param name="cell">单元格</param>
        /// <returns></returns>
        public CellValue ForceEvaluatorCell(ICell cell)
        {
            return workbook.GetCreationHelper().CreateFormulaEvaluator().Evaluate(cell);
        }

        /// <summary>
        /// 读取全部Sheet成DataSet
        /// </summary>
        public DataSet ToDataSet()
        {
            DataSet ds = new DataSet();
            foreach (ISheet sheet in GetSheetAll())
            {
                if (sheet == null) continue;
                if (CellForceFormula) sheet.ForceFormulaRecalculation = true;
                ds.Tables.Add(SheetToDataTable(sheet));
            }
            return ds;
        }

        /// <summary>
        /// 读取Sheet成DataTable
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public DataTable ToDataTable(ISheet sheet)
        {
            return SheetToDataTable(sheet);
        }

        /// <summary>
        /// 读取Sheet到DataTable
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        private DataTable SheetToDataTable(ISheet sheet)
        {
            DataTable dt = new DataTable(sheet.SheetName);
            IRow row = sheet.GetRow((StartRow - 1) ?? 0);
            int startCell = ColumnToIndex(StartColumn) ?? row.FirstCellNum;
            int lastCell = ColumnToIndex(EndColumn) ?? row.LastCellNum;
            for (int i = startCell; i < lastCell; i++)
            {
                if (AuthColumnName)
                {
                    dt.Columns.Add(new DataColumn());
                }
                else
                {
                    try
                    {
                        ICell cell = row.GetCell(i);
                        if (cell.CellType == CellType.Formula)
                        {
                            if (CellForceFormula)
                            {
                                dt.Columns.Add(new DataColumn(ForceEvaluatorCell(cell).StringValue));
                            }
                        }
                        else
                        {
                            dt.Columns.Add(new DataColumn(cell.StringCellValue));
                        }
                    }
                    catch (System.Exception ex)
                    {
                        dt.Columns.Add(new DataColumn());
                        ErrorLog.Add($"生成列发生错误:第{i}列,错误信息:{ex.Message}");
                    }
                }
            }

            for (int i = ((StartRow - 1) ?? sheet.FirstRowNum); i < ((EndRow - 1) ?? sheet.LastRowNum); i++)
            {
                row = sheet.GetRow(i);
                DataRow dr = dt.NewRow();
                for (int t = startCell; t < lastCell; t++)
                {
                    ICell cell = row.GetCell(t);
                    try
                    {
                        if (cell == null)
                        {
                            dr[t] = DBNull.Value;
                        }
                        else
                        {
                            if (cell.CellType == CellType.Formula && CellForceFormula)
                            {
                                cell.SetCellType(cell.CachedFormulaResultType);
                            }

                            if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
                            {
                                dr[t] = cell.DateCellValue;
                            }
                            else if (cell.CellType == CellType.Numeric)
                            {
                                dr[t] = cell.NumericCellValue;
                            }
                            else if (cell.CellType == CellType.Blank)
                            {
                                dr[t] = DBNull.Value;
                            }
                            else
                            {
                                dr[t] = cell.StringCellValue;
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        dr[t] = DBNull.Value;
                        ErrorLog.Add($"列赋值发生错误:第{i}行 第{IndexToColumn(t)}列,错误信息:{ex.Message}");
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        /// 获取Excal 列坐标的对应的索引
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private int? ColumnToIndex(string column)
        {
            if (column == null) return null;
            if (column.Length == 1)
            {
                return System.Array.IndexOf(sequence, column[0]);
            }
            int temp = (column.Length - 1) * 26;
            return System.Array.IndexOf(sequence, column[column.Length - 1]) + temp;
        }

        /// <summary>
        /// 根据索引获取Excal 列坐标
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private string IndexToColumn(int index)
        {
            int head = index / 26;
            int foot = index % 26;
            string temp = "";
            temp += sequence[foot];
            temp = temp.PadLeft(head + 1, 'A');
            return temp;
        }

        /// <summary>
        /// 读取DataTable
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool Load(DataTable dt)
        {
            if (workbook.GetSheet(dt.TableName) != null)
            {
                return false;
            }
            return DataTableToSheet(workbook.CreateSheet(dt.TableName), dt);
        }

        /// <summary>
        /// 读取DataSet
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public bool Load(DataSet ds)
        {
            foreach (DataTable dt in ds.Tables)
            {
                if (workbook.GetSheet(dt.TableName) != null)
                {
                    return false;
                }
            }
            foreach (DataTable dt in ds.Tables)
            {
                if (!DataTableToSheet(workbook.CreateSheet(dt.TableName), dt))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 读取DataTable到Sheet
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        private bool DataTableToSheet(ISheet sheet, DataTable dt)
        {
            IRow headRow = sheet.CreateRow(0);
            int colNum = dt.Columns.Count;
            int rowNum = dt.Rows.Count;
            for (int i = 0; i < colNum; i++)
            {
                ICell cell = headRow.CreateCell(i);
                AuthCellType(cell, dt.Columns[i].DataType, dt.Columns[i].ColumnName);
            }
            for (int i = 1; i < (rowNum + 1); i++)
            {
                IRow row = sheet.CreateRow(i);
                for (int t = 0; t < colNum; t++)
                {
                    ICell cell = row.CreateCell(t);
                    AuthCellType(cell, dt.Columns[t].DataType, dt.Rows[i - 1][t]);
                }
            }
            return true;
        }

        /// <summary>
        /// 保存Excal文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool Save(string filePath)
        {
            try
            {
                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fs);
                }
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 自动单元格格式
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="type"></param>
        private void AuthCellType(ICell cell, Type type, object value = null)
        {
            if (typeof(DateTime) == type)
            {
                cell.CellStyle = DateTimeStyle;
                if (value != null) cell.SetCellValue(Convert.ToDateTime(value));
            }
            else if (typeof(int) == type)
            {
                //cell.CellStyle = IntStyle;
                if (value != null) cell.SetCellValue(Convert.ToInt32(value));
            }
            else if (typeof(short) == type)
            {
                //cell.CellStyle = IntStyle;
                if (value != null) cell.SetCellValue(Convert.ToInt16(value));
            }
            else if (typeof(long) == type)
            {
                //cell.CellStyle = IntStyle;
                if (value != null) cell.SetCellValue(Convert.ToInt64(value));
            }
            else if (typeof(double) == type)
            {
                //cell.CellStyle = DoubleStyle;
                if (value != null) cell.SetCellValue(Convert.ToDouble(value));
            }
            else if (typeof(decimal) == type)
            {
                //cell.CellStyle = DoubleStyle;
                if (value != null) cell.SetCellValue(Convert.ToDouble(value));
            }
            else if (typeof(float) == type)
            {
                //cell.CellStyle = DoubleStyle;
                if (value != null) cell.SetCellValue(Convert.ToSingle(value));
            }
            else if (typeof(string) == type)
            {
                if (value != null) cell.SetCellValue(Convert.ToString(value));
            }
        }

        /// <summary>
        /// 设置全局日期格式 例:yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string GlobalDataTimeFormat { get; set; }

        /// <summary>
        /// 创建日期格式
        /// </summary>
        /// <returns></returns>
        private ICellStyle CreateDateTimeStyle()
        {
            IDataFormat format = workbook.CreateDataFormat();
            ICellStyle style = workbook.CreateCellStyle();
            style.DataFormat = format.GetFormat(GlobalDataTimeFormat ?? "yyyy-MM-dd HH:mm:ss");
            return style;
        }

        /// <summary>
        /// 单例字段
        /// </summary>
        private ICellStyle _DateTimeStyle;

        /// <summary>
        /// 日期格式样式
        /// </summary>
        public ICellStyle DateTimeStyle
        {
            get
            {
                if (_DateTimeStyle == null)
                {
                    _DateTimeStyle = CreateDateTimeStyle();
                }
                return _DateTimeStyle;
            }
            set
            {
                _DateTimeStyle = value;
            }
        }

        /// <summary>
        /// 单例字段
        /// </summary>
        private ICellStyle _IntStyle;

        /// <summary>
        /// 整数格式样式
        /// </summary>
        public ICellStyle IntStyle
        {
            get
            {
                if (_IntStyle == null)
                {
                    IDataFormat format = workbook.CreateDataFormat();
                    ICellStyle style = workbook.CreateCellStyle();
                    style.DataFormat = format.GetFormat("0_");
                    _IntStyle = style;
                }
                return _IntStyle;
            }
            set
            {
                _IntStyle = value;
            }
        }

        /// <summary>
        /// 单例字段
        /// </summary>
        private ICellStyle _DoubleStyle;

        /// <summary>
        /// 数值格式样式
        /// </summary>
        public ICellStyle DoubleStyle
        {
            get
            {
                if (_DoubleStyle == null)
                {
                    IDataFormat format = workbook.CreateDataFormat();
                    ICellStyle style = workbook.CreateCellStyle();
                    style.DataFormat = format.GetFormat("0.00_);[红色](0.00)");
                    _DoubleStyle = style;
                }
                return _DoubleStyle;
            }
            set
            {
                _DoubleStyle = value;
            }
        }
    }
}
