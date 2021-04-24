using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Communication
{
    /// <summary>
    /// 封包消息类型
    /// </summary>
    public enum MessageMode
    {
        None = 0,
        MessageShort = 1,
        MessageInt = 2,
        Disconect = 0xFF,
        Reconnect = 0xFE,
    }
}
