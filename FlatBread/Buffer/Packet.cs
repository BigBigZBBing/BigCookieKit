using FlatBread.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FlatBread.Buffer
{
    public class Packet
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        internal MessageMode Mode { get; set; }

        /// <summary>
        /// 包头是否完整
        /// </summary>
        bool HasHeader { get { return HeadTargetLength > -1 && HeadTargetLength == HeadCurrentLength; } }

        /// <summary>
        /// 包头的目标长度
        /// </summary>
        int HeadTargetLength { get; set; }

        /// <summary>
        /// 包头的当前长度
        /// </summary>
        int HeadCurrentLength { get; set; }

        /// <summary>
        /// 包头缓存
        /// </summary>
        Memory<byte> HeadCache { get; set; }

        /// <summary>
        /// 包体是否完整
        /// </summary>
        bool HasBody { get { return BodyTargetLength > -1 && BodyTargetLength == BodyCurrentLength; } }

        /// <summary>
        /// 包体的目标长度
        /// </summary>
        int BodyTargetLength { get; set; }

        /// <summary>
        /// 包体的当前长度
        /// </summary>
        int BodyCurrentLength { get; set; }

        /// <summary>
        /// 封包所有内容
        /// </summary>
        Memory<byte> PacketContent { get; set; }

        internal Packet()
        {
            HeadTargetLength = -1;
            HeadCurrentLength = -1;
            BodyTargetLength = -1;
            BodyCurrentLength = -1;
        }

        /// <summary>
        /// 加载包头
        /// </summary>
        /// <param name="tmpStream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int LoadHead(Span<byte> stream)
        {
            Span<byte> tmpStream = stream;
            int Offset = 0;
            //如果包头不完整
            if (!HasHeader && tmpStream.Length > 0)
            {
                //如果大于-1 说明包头未加载完成
                if (HeadTargetLength > -1)
                {
                    //可以读完包头
                    if (tmpStream.Length >= (HeadTargetLength - HeadCurrentLength))
                    {
                        tmpStream.Slice(0, HeadTargetLength - HeadCurrentLength).CopyTo(HeadCache.Slice(HeadCurrentLength).Span);
                        HeadCurrentLength = HeadTargetLength;
                        Span<byte> lengthBit = HeadCache.Slice(1).Span;
                        Offset += lengthBit.Length;
                        switch (lengthBit.Length)
                        {
                            case 1: BodyTargetLength = lengthBit[0]; break;
                            case 2: BodyTargetLength = BitConverter.ToInt16(lengthBit); break;
                            case 3: BodyTargetLength = BitConverter.ToInt32(lengthBit); break;
                        }
                        BodyCurrentLength = 0;
                    }
                    else
                    {
                        tmpStream.Slice(0, tmpStream.Length).CopyTo(HeadCache.Slice(HeadCurrentLength).Span);
                        HeadCurrentLength += tmpStream.Length;
                        Offset += tmpStream.Length;
                    }
                }
                //包头未加载过首先获取包头类型
                else
                {
                    //创建包头缓存
                    var packetType = tmpStream[0];
                    switch (packetType)
                    {
                        case 1: //byte 1字节
                        case 255: //disconnect 1字节
                        case 254: //reconnect 1字节
                            HeadTargetLength = 2;
                            break;
                        case 2: //short 2字节
                            HeadTargetLength = 3;
                            break;
                        case 3: //int 4字节
                            HeadTargetLength = 5;
                            break;
                    }
                    HeadCache = new byte[HeadTargetLength];
                    Mode = (MessageMode)packetType;
                    //优先加载封包类型
                    HeadCache.Span[0] = packetType;
                    HeadCurrentLength = 1;
                    Offset += 1;
                    //递归执行未加载完成的问题
                    if (tmpStream.Length > 1)
                        Offset += LoadHead(tmpStream.Slice(1));
                }
            }
            return Offset;
        }

        /// <summary>
        /// 加载包体
        /// </summary>
        /// <param name="tmpStream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int LoadBody(Span<byte> stream)
        {
            Span<byte> tmpStream = stream;
            int Offset = 0;
            if (!HasBody && tmpStream.Length > 0)
            {
                //初始化包体
                if (PacketContent.IsEmpty)
                    PacketContent = new byte[BodyTargetLength];

                //可以一次性读完
                var residue = BodyTargetLength - BodyCurrentLength;
                if (tmpStream.Length >= residue)
                {
                    tmpStream.Slice(0, residue).CopyTo(PacketContent.Slice(BodyCurrentLength).Span);
                    BodyCurrentLength = BodyTargetLength;
                    Offset += residue;
                }
                else
                {
                    tmpStream.Slice(0, tmpStream.Length).CopyTo(PacketContent.Slice(BodyCurrentLength).Span);
                    BodyCurrentLength += tmpStream.Length;
                    Offset += tmpStream.Length;
                }
            }
            return Offset;
        }

        /// <summary>
        /// 封包是否接收完成
        /// </summary>
        /// <returns></returns>
        internal bool IsCompleted()
        {
            if (HasHeader && HasBody)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 重载隐式转换
        /// </summary>
        /// <param name="packet"></param>
        public static implicit operator byte[](Packet packet) => packet.PacketContent.ToArray();

        /// <summary>
        /// 内容长度
        /// </summary>
        public int? Length { get { return PacketContent.Length; } }
    }
}
