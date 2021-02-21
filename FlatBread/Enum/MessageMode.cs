using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatBread.Enum
{
    /// <summary>
    /// 封包消息类型
    /// </summary>
    public enum MessageMode
    {
        None = 0,
        MessageByte = 1,
        MessageShort = 2,
        MessageInt = 3,
        Disconect = 0xFF,
        Reconnect = 0xFE,
    }
}
