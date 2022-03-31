using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.AspCore.Standard
{
    public abstract class ApiPermission
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 所属租户
        /// </summary>
        public string[] Tenement { get; set; }

        /// <summary>
        /// 权限码
        /// </summary>
        public string[] PermissionCode { get; set; }
    }
}
