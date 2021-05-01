using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace BigCookieKit.File
{
    public class IniMap
    {
        private FileInfo info { get; set; }

        public List<Section> Sections = new List<Section>();

        public IniMap(string path)
        {
            info = new FileInfo(path);
            if (!info.Exists) info.Create().Close();

            using StreamReader stream = info.OpenText();
            while (!stream.EndOfStream)
            {
                string line = stream.ReadLine().Trim();
                if (line.Length > 0)
                {
                    var section = Regex.Match(line, "(?<=^\\[).*(?=])");

                    if (section.Success)
                    {
                        Section nRoot = new Section();
                        nRoot.Name = section.Value;
                        Sections.Add(nRoot);
                        break;
                    }

                    if (line.IndexOf("=") > -1)
                    {
                        var kv = line.Split("=");
                        var last = Sections.Last();
                        last.Parameters.Add(new Parameter(kv[0], kv[1]));
                        break;
                    }
                }
            }
        }

        public Section this[string section]
        {
            get
            {
                var res = Sections.FirstOrDefault(x => x.Name.Equals(section, StringComparison.OrdinalIgnoreCase));
                if (res.IsNull())
                {
                    Section nRoot = new Section();
                    nRoot.Name = section;
                    nRoot.ToMap = e => Sections.Add(e);
                    return nRoot;
                }
                return res;
            }
        }

        public void Remove(string section)
        {
            Sections.Remove(Sections.FirstOrDefault(x => x.Name.Equals(section, StringComparison.OrdinalIgnoreCase)));
        }

        public void Save()
        {
            using StreamWriter sw = info.CreateText();
            foreach (var root in Sections)
            {
                sw.WriteLine($"[{root.Name}]");
                foreach (var node in root.Parameters)
                {
                    sw.WriteLine($"{node.Key}={node.Value}");
                }
            }
        }
    }

    public class Section
    {
        internal Action<Section> ToMap { get; set; }

        /// <summary>
        /// 大节点
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 小节点
        /// </summary>
        public List<Parameter> Parameters = new List<Parameter>();

        public string this[string key]
        {
            get
            {
                return Parameters.FirstOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase))?.Value;
            }
            set
            {
                var node = Parameters.FirstOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
                if (node.IsNull())
                {
                    Parameters.Add(new Parameter(key, value));
                    ToMap(this);
                    return;
                }
                node.Value = value;
            }
        }

        public void Remove(string key)
        {
            Parameters.Remove(Parameters.FirstOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase)));
        }
    }

    public class Parameter
    {
        internal Parameter(string Key, string Value)
        {
            this.Key = Key;
            this.Value = Value;
        }

        /// <summary>
        /// 键
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }
}
