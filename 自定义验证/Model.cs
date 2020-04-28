using GeneralKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace 自定义验证
{
    public class Model : ICheckVerify
    {
        public Model() { }

        private int? id;
        private string name;
        private int? old;
        private long old1;
        private string adress;
        private DateTime? time;

        [Mark("ID", AllowEmpty = false, Error = "{0}不能为空")]
        public int? Id { get => id; set => id = value; }

        [Mark("姓名", ExpType = ExpType.Chinese, Error = "{0}类型必须为中文")]
        public string Name { get => name; set => name = value; }

        [Mark("年龄", MaxLength = 1, Error = "{2}只能为个位数")]
        public int? Old { get => old; set => old = value; }

        [Mark("地址", MinLength = 1, MaxLength = 10, Error = "{2}长度必须在1至10位")]
        public string Adress { get => adress; set => adress = value; }

        [Mark("时间")]
        public DateTime? Time { get => time; set => time = value; }

        public long Old1 { get => old1; set => old1 = value; }
    }

    public enum Data
    {
        [Remark("默认")]
        None,
        [Remark("正常")]
        Normal,
        [Remark("错误")]
        Error
    }
}
