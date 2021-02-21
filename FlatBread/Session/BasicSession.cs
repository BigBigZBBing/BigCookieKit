using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatBread.Session
{
    public abstract class BasicSession
    {
        /// <summary>
        /// 用户端的唯一编码
        /// </summary>
        public string UserCode { get; set; } = Guid.NewGuid().ToString("D");

        /// <summary>
        /// 用户端的地址
        /// </summary>
        public string UserHost { get; set; }

        /// <summary>
        /// 用户端的端口
        /// </summary>
        public int? UserPort { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime? OperationTime { get; set; }
    }
}
