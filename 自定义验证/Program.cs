using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GeneralKit;

namespace 自定义验证
{
    class Program
    {
        static void Main(string[] args)
        {
            //模型验证
            Model model = new Model();
            //model.Id = 1;
            model.Name = "123213";
            model.Old = 27;
            model.Adress = "";
            model.ModelValidation();

            //获取字段字符串
            string filed = model.Key(d => d.Old1);
            //钱转中文大写
            decimal dec = 8463341836481.7M;
            string temp = dec.MoneyUpper();
            //获取备注
            ExpType.Mail.Remark();
            Console.WriteLine("Hello World!");
        }
    }
}
