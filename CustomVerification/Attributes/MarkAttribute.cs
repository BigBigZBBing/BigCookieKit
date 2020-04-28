using GeneralKit.Attributes;
using System;

namespace GeneralKit
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class MarkAttribute : BasicsAttribute
    {
        public MarkAttribute(string Name) 
        {
            base.Name = Name;
        }

        private int maxLength = -1;
        private int minLength = -1;
        private string error = "";
        private bool allowEmpty = true;
        private int greater = -1;
        private int less = -1;
        private int equal = -1;
        private string regExp = "";
        private ExpType expType;

        /// <summary>
        /// 最大长度
        /// </summary>
        public int MaxLength { get => maxLength; set => maxLength = value; }
        /// <summary>
        /// 最小长度
        /// </summary>
        public int MinLength { get => minLength; set => minLength = value; }
        /// <summary>
        /// 异常消息
        /// </summary>
        public string Error { get => error; set => error = value; }
        /// <summary>
        /// 是否可空
        /// </summary>
        public bool AllowEmpty { get => allowEmpty; set => allowEmpty = value; }
        /// <summary>
        /// 大于
        /// </summary>
        public int Greater { get => greater; set => greater = value; }
        /// <summary>
        /// 小于
        /// </summary>
        public int Less { get => less; set => less = value; }
        /// <summary>
        /// 等于
        /// </summary>
        public int Equal { get => equal; set => equal = value; }
        /// <summary>
        /// 正则表达式匹配
        /// </summary>
        public string RegExp { get => regExp; set => regExp = value; }
        /// <summary>
        /// 正则表达式类型
        /// </summary>
        public ExpType ExpType { get => expType; set => expType = value; }
    }
}
