﻿using BigCookieKit.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Office.Xlsx
{
    public class WriteExcelKit : IDisposable
    {
        private Dictionary<string, string> fixedTemplate => new Dictionary<string, string>()
        {
            { @"_rels/.rels", StreamToString(Common.GetXlsxResource(".rels")) },
            { @"xl/styles.xml", StreamToString(Common.GetXlsxResource("styles.xml")) },
        };

        private readonly static UTF8Encoding _utf8WithBom = new System.Text.UTF8Encoding(true);
        private FileStream fileStream;
        private ZipArchive zipArchive;
        private bool disposedValue;

        public WriteExcelKit(string path)
        {
            fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Create);
        }

        public void Save(DataSet ds)
        {
            if (ds.NotExist()) throw new ArgumentNullException();

            var template = new Dictionary<string, string>(fixedTemplate);

            Dictionary<string, string> sheets = new Dictionary<string, string>();
            int index = 1;
            StringBuilder builder = new StringBuilder();

            foreach (DataTable dt in ds.Tables)
            {
                builder.Clear();
                builder.Append($@"<?xml version=""1.0"" encoding=""utf-8""?>");
                builder.Append($@"<worksheet xmlns=""http://schemas.openxmlformats.org/spreadsheetml/2006/main"" xmlns:r=""http://schemas.openxmlformats.org/officeDocument/2006/relationships"" xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006"" xmlns:x14ac=""http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac"" mc:Ignorable=""x14ac"">");
                builder.Append($@"<dimension ref=""A1:{ExcelHelper.IndexToColumn(dt.Columns.Count - 1)}{dt.Rows.Count}""/><sheetData>");
                sheets.Add("sheet" + index, dt.TableName);

                for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                {
                    DataRow dr = dt.Rows[rowIndex];
                    builder.Append($"<row r=\"{(rowIndex + 1)}\">");
                    for (int colIndex = 0; colIndex < dt.Columns.Count; colIndex++)
                    {
                        DataColumn dc = dt.Columns[colIndex];
                        builder.Append($"<c r=\"{ExcelHelper.IndexToColumn(colIndex)}{(rowIndex + 1)}\" t=\"str\">");
                        builder.Append($"<v>{dr[dc.ColumnName]?.ToString()}");
                        builder.Append($"</v>");
                        builder.Append($"</c>");
                    }
                    builder.Append($"</row>");
                }
                builder.Append("</sheetData></worksheet>");
                template.Add($"xl/worksheets/sheet{index}.xml", builder.ToString());
                index++;
            }

            var template1 = StreamToString(Common.GetXlsxResource("[Content_Types].xml"));
            string dynamicTemplate1 = "";
            foreach (var item in sheets)
            {
                string contenttypes = $"<Override ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\" PartName=\"/xl/worksheets/{item.Key}.xml\" />";
                dynamicTemplate1 += contenttypes;
            }
            template.Add("[Content_Types].xml", template1.RulesFormat(dynamicTemplate1));

            int rId = 1;
            var template2 = StreamToString(Common.GetXlsxResource("workbook.xml.rels"));
            var template3 = StreamToString(Common.GetXlsxResource("workbook.xml"));
            string dynamicTemplate2 = "";
            string dynamicTemplate3 = "";
            foreach (var item in sheets)
            {
                string workbook = $"<sheet name=\"{item.Value}\" sheetId=\"{rId}\" r:id=\"rId{rId}\"/>";
                string workbookrels = $"<Relationship Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"/xl/worksheets/{item.Key}.xml\" Id=\"rId{rId}\" />";
                dynamicTemplate2 += workbookrels;
                dynamicTemplate3 += workbook;
                rId++;
            }
            dynamicTemplate2 += $"<Relationship Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles\" Target=\"/xl/styles.xml\" Id=\"rId{rId}\" />";
            template.Add("xl/_rels/workbook.xml.rels", template2.RulesFormat(dynamicTemplate2));
            template.Add("xl/workbook.xml", template3.RulesFormat(dynamicTemplate3));

            foreach (var item in template)
            {
                var entry = zipArchive.CreateEntry(item.Key, CompressionLevel.NoCompression);
                using var stream = entry.Open();
                using StreamWriter writer = new StreamWriter(stream, _utf8WithBom);
                writer.Write(item.Value);
            }
        }

        private string StreamToString(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(bytes, 0, bytes.Length);
            return Kit.BitToString(bytes, Encoding.UTF8);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    zipArchive?.Dispose();
                    fileStream?.Close();
                    fileStream?.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并替代终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~WriteExcelKit()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
