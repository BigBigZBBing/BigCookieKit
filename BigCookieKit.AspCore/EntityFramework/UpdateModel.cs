using System;
using System.ComponentModel.DataAnnotations;

namespace BigCookieKit.AspCore.EntityFramework
{
    public class UpdateModel
    {
        [Key]
        public long Id { get; set; }

        public long? UpdateUserId { get; set; }

        public string UpdateUserName { get; set; }

        public DateTime? UpdateTime { get; set; }
    }
}
