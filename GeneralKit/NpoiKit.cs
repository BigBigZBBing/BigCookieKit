using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace BigCookieKit
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
        /// 异常日志
        /// </summary>
        public List<string> ErrorLog { get; set; } = new List<string>();

        /// <summary>
        /// 工作簿
        /// </summary>
        private IWorkbook workbook { get; set; }

        /// <summary>
        /// Excel配置
        /// </summary>
        private ExcelConfig config { get; set; }

        /// <summary>
        /// 所有单元格直接获取公式结果
        /// </summary>
        public bool CellForceFormula { get; set; }

        /// <summary>
        /// 导出使用的构造函数
        /// </summary>
        /// <param name="type"></param>
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

        /// <summary>
        /// 导入使用的构造函数
        /// </summary>
        /// <param name="excelPath"></param>
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
        [Obsolete("使用另一种替换方案")]
        public CellValue ForceEvaluatorCell(ICell cell)
        {
            return workbook.GetCreationHelper().CreateFormulaEvaluator().Evaluate(cell);
        }

        #region Read

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
        DataTable SheetToDataTable(ISheet sheet)
        {
            DataTable dt = new DataTable(sheet.SheetName);
            IRow row = sheet.GetRow((config.ColumnNameRow - 1) ?? sheet.FirstRowNum);
            Dictionary<int, string> cellDic = new Dictionary<int, string>();
            int startCell = config.StartColumnIndex ?? row.FirstCellNum;
            int lastCell = config.EndColumnIndex + 1 ?? row.LastCellNum;
            int startRow = ((config.StartRow - 1) ?? sheet.FirstRowNum);
            int endRow = ((config.EndRow - 1) ?? sheet.LastRowNum);

            for (int i = startCell; i < lastCell; i++)
            {
                if (config.ColumnNameRow.IsNull())
                {
                    var dc = new DataColumn();
                    dt.Columns.Add(dc);
                    cellDic.Add(i, dc.ColumnName);
                }
                else
                {
                    try
                    {
                        ICell cell = row.GetCell(i);
                        if (cell.CellType == CellType.Formula)
                        {
                            cell.SetCellType(cell.CachedFormulaResultType);
                        }
                        var dc = new DataColumn(cell.StringCellValue);
                        dt.Columns.Add(new DataColumn(cell.StringCellValue));
                        cellDic.Add(i, dc.ColumnName);
                    }
                    catch (System.Exception ex)
                    {
                        var dc = new DataColumn();
                        dt.Columns.Add(dc);
                        cellDic.Add(i, dc.ColumnName);
                        ErrorLog.Add($"生成列发生错误:第{ExcelConfig.IndexToColumn(i)}列,错误信息:{ex.Message}");
                    }
                }
            }

            for (int i = startRow; i <= endRow; i++)
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
                            dr[cellDic[t]] = DBNull.Value;
                        }
                        else
                        {
                            if (cell.CellType == CellType.Formula && CellForceFormula)
                            {
                                cell.SetCellType(cell.CachedFormulaResultType);
                            }
                            if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
                            {
                                dr[cellDic[t]] = cell.DateCellValue;
                            }
                            else if (cell.CellType == CellType.Numeric)
                            {
                                dr[cellDic[t]] = cell.NumericCellValue;
                            }
                            else if (cell.CellType == CellType.Blank)
                            {
                                dr[cellDic[t]] = DBNull.Value;
                            }
                            else
                            {
                                dr[cellDic[t]] = cell.StringCellValue;
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        dr[cellDic[t]] = DBNull.Value;
                        ErrorLog.Add($"列赋值发生错误:第{i}行 第{ExcelConfig.IndexToColumn(t)}列,错误信息:{ex.Message}");
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        #endregion

        #region Write

        /// <summary>
        /// 读取DataTable到Sheet中
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
        /// 读取DataSet到Sheet中
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
        bool DataTableToSheet(ISheet sheet, DataTable dt)
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

        #endregion

        /// <summary>
        /// 创建Excel配置
        /// </summary>
        /// <param name="callback"></param>
        public void CreateConfig(Action<ExcelConfig> callback)
        {
            config = new ExcelConfig();
            callback?.Invoke(config);
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
        /// 自动单元格格式
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="type"></param>
        void AuthCellType(ICell cell, Type type, object value = null)
        {
            if (typeof(DateTime) == type)
            {
                cell.CellStyle = DateTimeStyle;
                if (value != null) cell.SetCellValue(Convert.ToDateTime(value));
            }
            else if (typeof(int) == type)
            {
                if (value != null) cell.SetCellValue(Convert.ToInt32(value));
            }
            else if (typeof(short) == type)
            {
                if (value != null) cell.SetCellValue(Convert.ToInt16(value));
            }
            else if (typeof(long) == type)
            {
                if (value != null) cell.SetCellValue(Convert.ToInt64(value));
            }
            else if (typeof(double) == type)
            {
                if (value != null) cell.SetCellValue(Convert.ToDouble(value));
            }
            else if (typeof(decimal) == type)
            {
                if (value != null) cell.SetCellValue(Convert.ToDouble(value));
            }
            else if (typeof(float) == type)
            {
                if (value != null) cell.SetCellValue(Convert.ToSingle(value));
            }
            else if (typeof(string) == type)
            {
                if (value != null) cell.SetCellValue(Convert.ToString(value));
            }
        }

        #region Format

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

        #endregion
    }
}
