using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.AspCore.EntityFramework
{
    public class DbModel
    {
        [Key]
        public long Id { get; set; }

        public long? CreateUserId { get; set; }

        public string CreateUserName { get; set; }

        public DateTime? CreateTime { get; set; }

        public long? UpdateUserId { get; set; }

        public string UpdateUserName { get; set; }

        public DateTime? UpdateTime { get; set; }
    }
}
