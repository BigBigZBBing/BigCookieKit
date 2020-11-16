using System;
using System.Collections.Generic;
using System.Text;

namespace GeneralKit
{
    public enum ExpType
    {
        None,
        /// <summary>
        /// 邮件格式
        /// </summary>
        [Remark(@"\w[-\w.+]*@([A-Za-z0-9][-A-Za-z0-9]+\.)+[A-Za-z]{2,14}")]
        Mail,
        /// <summary>
        /// Url格式
        /// </summary>
        [Remark(@"^((https|http|ftp|rtsp|mms)?:\/\/)[^\s]+")]
        Url,
        /// <summary>
        /// 手机格式
        /// </summary>
        [Remark("0?(13|14|15|17|18|19)[0-9]{9}")]
        Mobile,
        /// <summary>
        /// 小数格式
        /// </summary>
        [Remark("^([1-9]+|0{1}).[1-9]{1,}$")]
        Decimal,
        /// <summary>
        /// 时间格式
        /// </summary>
        [Remark(@"^(\d{2,4}(\.|\/| |\-)?\d{1,2}(\.|\/| |\-)?\d{1,2}( ?)(\d{1,2}:?\d{1,2}(:?\d{1,2})?)?)$")]
        DateTime,
        /// <summary>
        /// 整数格式
        /// </summary>
        [Remark(@"^[1-9]\d*$")]
        Int,
        /// <summary>
        /// IP格式
        /// </summary>
        [Remark(@"(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)\.(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)\.(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)\.(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)")]
        Ip,
        /// <summary>
        /// QQ格式
        /// </summary>
        [Remark(@"[1-9]([0-9]{5,11})")]
        QQ,
        /// <summary>
        /// 身份证格式
        /// </summary>
        [Remark(@"\d{17}[\d|x]|\d{15}")]
        IdentityCard,
        /// <summary>
        /// 中文格式
        /// </summary>
        [Remark(@"^[\u4e00-\u9fa5]+$")]
        Chinese
    }

}
