using System;
using System.Collections.Generic;
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
            Async = true
        };

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public XmlPacket Read(string mainNode)
        {
            return XmlRead(mainNode);
        }

        #region 内存树方案
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private XmlPacket XmlRead(string Node)
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
                            ReadContentFrom();
                        }
                        #endregion

                        #region EOF方式读取
                        if (_curr != null)
                        {
                            if (_curr.State == PacketState.Start)
                            {
                                packet = new XmlPacket();
                                packet.Parent = _curr;
                                _curr = packet;
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
        #endregion

        #region 递归方案
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete("递归产生的调用函数产生的开销和异步回调产生的开销过大 此方案弃用")]
        private XmlPacket XmlReadRoot(string Node)
        {
            XmlPacket packet = new XmlPacket();
            bool switchover = false;
            while (switchover || _read.Read())
            {
                switch (_read.NodeType)
                {
                    case XmlNodeType.Element:
                        if (_read.Name.Equals(Node, StringComparison.OrdinalIgnoreCase))
                        {
                            ReadContentFrom();
                            packet.Node = XmlReadNode(ref switchover);
                        }
                        break;
                    case XmlNodeType.Text:
                        if (_read.HasValue)
                        {
                            _callback?.Invoke(_read.Value);
                            _callback = null;
                            return null;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if (_read.Name.Equals(Node, StringComparison.OrdinalIgnoreCase))
                        {
                            return packet;
                        }
                        break;
                }
                if (_read.EOF) throw new EndOfStreamException();
            }
            return packet;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete("递归产生的调用函数产生的开销和异步回调产生的开销过大 此方案弃用")]
        private XmlPacket[] XmlReadNode(ref bool switchover)
        {
            List<XmlPacket> packetSet = new List<XmlPacket>();
            XmlPacket packet = null;
            while (switchover || _read.Read())
            {
                //关闭轨道切换
                switchover = false;
                switch (_read.NodeType)
                {
                    case XmlNodeType.Element:
                        packet = new XmlPacket();
                        _callback = value =>
                        {
                            packet.Info.HasValue = true;
                            packet.Info.Text = value;
                        };
                        ReadContentFrom();
                        if (_read.IsEmptyElement)
                        {
                            if (packet.NotNull())
                            {
                                packetSet.Add(packet);
                                packet = null;
                            }
                        }
                        else
                        {
                            packet.Node = XmlReadNode(ref switchover);
                        }
                        break;
                    case XmlNodeType.Text:
                        if (_read.HasValue)
                        {
                            _callback?.Invoke(_read.Value);
                            _callback = null;
                            return null;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if (packet.NotNull())
                        {
                            packetSet.Add(packet);
                            packet = null;
                        }
                        else
                        {
                            //打开轨道切换
                            switchover = true;
                            return packetSet.ToArray();
                        }
                        break;
                }
                if (_read.EOF) throw new EndOfStreamException();
            }
            return packetSet.ToArray();
        }
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private XmlAttribute[] XmlReadNodeAttr()
        {
            if (!IsReadAttributes) return null;
            _curr.Info.Attributes = new XmlAttribute[_read.AttributeCount];
            for (int index = 0; index < _read.AttributeCount; index++)
            {
                _read.MoveToAttribute(index);
                _curr.Info.Attributes[index].Name = _read.Name;
                _curr.Info.Attributes[index].Text = _read.Value;
            }
            return xmlAttrs;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReadContentFrom()
        {
            _curr.Info.Name = _read.Name;
            if (_read.HasAttributes && _read.AttributeCount > 0)
            {
                XmlReadNodeAttr();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EndReadFrom()
        {

            if (_curr.Parent.Node == null)
                _curr.Parent.Node = new List<XmlPacket>();
            _curr.Parent.Node.Add(_curr);
            _curr.State = PacketState.End;
            _curr = _curr.Parent;
        }

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
