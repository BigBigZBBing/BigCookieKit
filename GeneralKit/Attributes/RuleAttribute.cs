using GeneralKit.Attributes;
using System;

namespace GeneralKit
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class RuleAttribute : BasicsAttribute
    {
        public RuleAttribute(string Name)
        {
            base.Name = Name;
        }

        /// <summary>
        /// 最大长度
        /// </summary>
        internal int? maxLength;

        /// <summary>
        /// 最小长度
        /// </summary>
        internal int? minLength;

        /// <summary>
        /// 错误消息
        /// </summary>
        internal string error;

        /// <summary>
        /// 是否可空
        /// </summary>
        internal bool? allowEmpty;

        /// <summary>
        /// 大于
        /// <code/>用于数值类型
        /// </summary>
        internal decimal? greater;

        /// <summary>
        /// 小于
        /// <code/>用于数值类型
        /// </summary>
        internal decimal? less;

        /// <summary>
        /// 小于
        /// <code/>用于数值类型
        /// </summary>
        internal decimal? equal;

        /// <summary>
        /// 自定义正则表达式
        /// </summary>
        internal string regExp;

        /// <summary>
        /// 正则表达式类型
        /// </summary>
        internal ExpType? expType;

        public int MaxLength { get => maxLength ?? 0; set => maxLength = value; }
        public int MinLength { get => minLength ?? 0; set => minLength = value; }
        public string Error { get => error; set => error = value; }
        public bool AllowEmpty { get => allowEmpty ?? true; set => allowEmpty = value; }
        public decimal Greater { get => greater ?? 0; set => greater = value; }
        public decimal Less { get => less ?? 0; set => less = value; }
        public decimal Equal { get => equal ?? 0; set => equal = value; }
        public string RegExp { get => regExp; set => regExp = value; }
        public ExpType ExpType { get => expType ?? ExpType.None; set => expType = value; }
    }
}
