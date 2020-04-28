using System;
using System.Collections.Generic;
using System.Text;

namespace GeneralKit.Attributes
{
    public abstract class BasicsAttribute : Attribute
    {
        internal BasicsAttribute() { }

        private string name = "";
        /// <summary>
        /// 字段名称
        /// </summary>
        public string Name { get => name; set => name = value; }
    }


}
