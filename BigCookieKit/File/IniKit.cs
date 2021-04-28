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

        public IniMap(string path)
        {
            info = new FileInfo(path);
            if (!info.Exists) info.Create().Close();

            using StreamReader stream = info.OpenText();
            while (!stream.EndOfStream)
            {
                string str = stream.ReadLine().Trim();
                if (str.Length > 0)
                {
                    var section = Regex.Match(str, "(?<=\\[).*(?=])");

                    if (section.Success)
                    {
                        Section nRoot = new Section();
                        nRoot.Name = section.Value;
                        Roots.Add(nRoot);
                    }

                    if (str.IndexOf("=") > -1)
                    {
                        var kv = str.Split("=");
                        var last = Roots.Last();
                        Node node = new Node();
                        node.Key = kv[0];
                        node.Value = kv[1];
                        last.Nodes.Add(node);
                    }
                }
            }
        }

        public List<Section> Roots = new List<Section>();

        public Section this[string section]
        {
            get
            {
                var res = Roots.FirstOrDefault(x => x.Name.Equals(section, StringComparison.OrdinalIgnoreCase));
                if (res.IsNull())
                {
                    Section nRoot = new Section();
                    nRoot.Name = section;
                    nRoot.ToMap = e => Roots.Add(e);
                    return nRoot;
                }
                return res;
            }
        }

        public void Remove(string section)
        {
            Roots.Remove(Roots.FirstOrDefault(x => x.Name.Equals(section, StringComparison.OrdinalIgnoreCase)));
        }

        public void Save()
        {
            using StreamWriter sw = info.CreateText();
            foreach (var root in Roots)
            {
                sw.WriteLine($"[{root.Name}]");
                foreach (var node in root.Nodes)
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
        public List<Node> Nodes = new List<Node>();

        public string this[string key]
        {
            get
            {
                return Nodes.FirstOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase))?.Value;
            }
            set
            {
                var node = Nodes.FirstOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
                if (node.IsNull())
                {
                    node = new Node();
                    node.Key = key;
                    node.Value = value;
                    Nodes.Add(node);
                    ToMap(this);
                    return;
                }
                node.Value = value;
            }
        }

        public void Remove(string key)
        {
            Nodes.Remove(Nodes.FirstOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase)));
        }
    }

    public class Node
    {
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
