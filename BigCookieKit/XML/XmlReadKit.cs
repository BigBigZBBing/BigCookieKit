using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BigCookieKit.XML
{
    public class XmlReadKit : IDisposable
    {
        private bool disposedValue;

        /// <summary>
        /// 当前文件流
        /// </summary>
        private Stream current { get; set; }

        #region 共享内存

        private XmlReader _read { get; set; }
        private Action<string> _callback { get; set; }
        private XmlPacket _curr { get; set; }
        private XmlAttribute[] xmlAttrs { get; set; }

        #endregion

        #region 用户配置
        public bool IsReadAttributes { get; set; } = true;
        #endregion

        /// <summary>
        /// 配置
        /// </summary>
        private readonly XmlReaderSettings settings = new XmlReaderSettings()
        {
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
            IgnoreWhitespace = true,
            XmlResolver = null,
        };

        public XmlReadKit() { }

        public XmlReadKit(Stream stream)
        {
            current = stream;
            _read = XmlReader.Create(current, settings);
        }

        public XmlReadKit(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            current = fs;
            _read = XmlReader.Create(current, settings);
        }

        #region 内存树方案
        public XmlPacket XmlRead(string Node)
        {
            XmlPacket packet = null;
            while (_read.Read())
            {
                switch (_read.NodeType)
                {
                    case XmlNodeType.Element:
                        #region 读取Root
                        if (_read.Name == Node)
                        {
                            packet = new XmlPacket();
                            _curr = packet;
                            packet.Parent = _curr;
                            bool isEmpty = _read.IsEmptyElement;
                            ReadContentFrom();
                            if (isEmpty)
                            {
                                _curr.State = PacketState.End;
                                return _curr;
                            }
                        }
                        #endregion

                        #region EOF方式读取
                        if (_curr != null)
                        {
                            if (_curr.State == PacketState.Start)
                            {
                                packet = new XmlPacket();
                                _curr = packet;
                                packet.Parent = _curr;
                                bool isEmpty = _read.IsEmptyElement;
                                ReadContentFrom();
                                if (isEmpty) EndReadFrom();
                            }
                            _curr.State = PacketState.Start;
                        }
                        #endregion
                        break;
                    case XmlNodeType.Text:
                        if (_read.HasValue)
                        {
                            _curr.Info.HasValue = true;
                            _curr.Info.Text = _read.Value;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        #region 返回Root
                        if (_read.Name == Node) return _curr;
                        #endregion

                        #region EOF结束
                        if (_curr != null && _curr.State == PacketState.Start)
                            EndReadFrom();
                        #endregion
                        break;
                }
                if (_read.EOF) throw new EndOfStreamException();
            }
            return packet;
        }

        public void XmlReadXlsx(string Node, Func<string, XmlAttribute[], string, bool> callback)
        {
            bool start = false;
            while (_read.Read())
            {
                switch (_read.NodeType)
                {
                    case XmlNodeType.Element:
                        if (_read.Name == Node) start = true;
                        if (start)
                        {
                            if (!callback(_read.Name, XmlReadAttr(), null))
                            {
                                callback("end", null, null);
                                return;
                            }
                        }
                        break;
                    case XmlNodeType.Text:
                        if (start && _read.HasValue)
                        {
                            if (!callback("text", null, _read.Value))
                            {
                                return;
                            }
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if (_read.Name == Node)
                        {
                            callback("end", null, null);
                            return;
                        }
                        break;
                }
            }
        }

        private void XmlReadNodeAttr()
        {
            if (!IsReadAttributes) return;
            _curr.Info.Attributes = XmlReadAttr();
        }

        public XmlAttribute[] XmlReadAttr()
        {
            if (_read.AttributeCount == 0) return null;
            var attrs = new XmlAttribute[_read.AttributeCount];
            for (int index = 0; index < _read.AttributeCount; index++)
            {
                _read.MoveToAttribute(index);
                attrs[index].Name = _read.Name;
                attrs[index].Text = _read.Value;
            }
            return attrs;
        }

        private void ReadContentFrom()
        {
            _curr.Info.Name = _read.Name;
            if (_read.HasAttributes && _read.AttributeCount > 0)
            {
                XmlReadNodeAttr();
            }
        }

        private void EndReadFrom()
        {
            if (_curr.Parent.Node == null)
                _curr.Parent.Node = new List<XmlPacket>();
            _curr.Parent.Node.Add(_curr);
            _curr.State = PacketState.End;
            _curr = _curr.Parent;
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    current.Close();
                    current.Dispose();
                    current = null;
                }

                // TODO: 释放未托管的资源(未托管的对象)并替代终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~XmlReadKit()
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
