using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeneralKit
{
    /// <summary>
    /// Excal工具箱(性能是NPOI的几倍)
    /// </summary>
    public partial class ExcalKit
    {
        /// <summary>
        /// Excel分解成zip文件
        /// </summary>
        private ZipArchive zip { get; set; }

        /// <summary>
        /// 共享字符串数组
        /// </summary>
        private string[] sharedStrings { get; set; }

        /// <summary>
        /// 共享样式链表
        /// </summary>
        private List<cellXfs> sharedStyles { get; set; }

        public ExcalKit(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) throw new FileNotFoundException(nameof(filePath));

            zip = ZipFile.OpenRead(filePath);

            LoadShareString();


        }

        /// <summary>
        /// 加载共享字符串
        /// </summary>
        private void LoadShareString()
        {
            ZipArchiveEntry entry = zip.GetEntry("xl/sharedStrings.xml");
            if (entry != null)
            {
                var doc = XDocument.Load(entry.Open());

                var elements = doc.Root.Elements();

                sharedStrings = new string[elements.Count()];

                int index = 0;

                foreach (var element in elements)
                {
                    sharedStrings[index] = element.Value;
                }
            }
        }

        /// <summary>
        /// 读取共享样式表
        /// </summary>
        private void LoadCellStyle()
        {
            ZipArchiveEntry entry = zip.GetEntry("xl/styles.xml");
            if (entry != null)
            {
                var doc = XDocument.Load(entry.Open());

                IList<cellXfs> list = new List<cellXfs>();

                var elements = doc.Root.Elements();

                var xfs = elements.FirstOrDefault(e => e.Name.LocalName == "cellXfs");

                foreach (var element in elements)
                {
                    list.Add(new cellXfs()
                    {
                        numFmtId = element.Attribute("numFmtId").Value
                    });
                }

            }
        }
    }




}