using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace BigCookieKit.Office.Csv
{
    public class CsvKit
    {
        /// <summary>
        /// 当前文件流
        /// </summary>
        private TextReader current { get; set; }


        public CsvKit(string filePath)
        {
            current = System.IO.File.OpenText(filePath);
        }

        public DataTable ReadDataTable(bool firstHeader)
        {
            DataTable dt = new DataTable();
            TextReader reader = current;
            var colLen = 0;

            while (reader.Peek() > 0)
            {
                var line = reader.ReadLine();
                var items = DataParse(line);
                if (firstHeader)
                {
                    foreach (var item in items)
                    {
                        dt.Columns.Add((string)item);
                    }
                    firstHeader = false;
                    colLen = dt.Columns.Count;
                }
                else
                {
                    var cells = items.ToArray();
                    if (colLen == 0)
                    {
                        foreach (var item in cells)
                        {
                            dt.Columns.Add((string)item);
                        }
                        colLen = dt.Columns.Count;
                        continue;
                    }
                    if (cells.Length > colLen)
                    {
                        cells = cells[..colLen].ToArray();
                    }
                    dt.Rows.Add(cells);
                }
            }

            return dt;
        }

        private IEnumerable<object> DataParse(string line)
        {
            bool cellStart = true; //是否单元格的开始
            bool escape = false; //是否转义单元格
            string cellStr = ""; //单元格文本

            for (int i = 0; i < line.Length; i++)
            {
                char item = line[i];

                if (cellStart && item == ',')
                {
                    yield return cellStr;
                    cellStr = "";
                    continue;
                }

                if (cellStart)
                {
                    if (item == '"') //判断是否单元格内有转义字符 
                    {
                        escape = true;
                    }
                    cellStart = false;
                    cellStr = "";

                    if (!escape) i -= 1;
                }
                else
                {
                    if (escape)
                    {
                        if (item == '"')
                        {
                            if ((i + 1) >= line.Length)
                            {
                                cellStart = true;
                                yield return cellStr;
                            }
                            else if (line[(i + 1)] == '"')
                            {
                                cellStr += "\"";
                                i += 1;
                                continue;
                            }
                            else if (line[(i + 1)] == ',')
                            {
                                cellStart = true;
                                continue;
                            }
                        }
                    }

                    cellStr += item;

                    if (!escape)
                    {
                        if ((i + 1) >= line.Length)
                        {
                            cellStart = true;
                            yield return cellStr;
                        }
                        else if (line[(i + 1)] == ',')
                        {
                            cellStart = true;
                            continue;
                        }
                    }
                }

            }
        }
    }
}
