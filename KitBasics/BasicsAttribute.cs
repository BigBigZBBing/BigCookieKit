using System;

namespace KitBasics
{
    public abstract class BasicsAttribute : Attribute
    {
        protected BasicsAttribute() { }

        private string name = "";
        /// <summary>
        /// 字段名称
        /// </summary>
        public string Name { get => name; set => name = value; }
    }
}
