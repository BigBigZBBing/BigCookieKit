using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace BigCookieKit.Reflect
{
    public sealed class FastDynamic : IEnumerable
    {
        private FastDynamic()
        {
        }

        public Object this[String Name]
        {
            get
            {
                return this.Properties[Name].Get();
            }
            set
            {
                this.Properties[Name].Set(value);
            }
        }

        public IDictionary<String, FastProperty> Properties { get; internal set; }

        internal Object Instance { get; set; }


        
        public String ToJson()
        {
            //return JsonConvert.SerializeObject(this.Instance);
            return "";
        }


        
        public String ToXml()
        {
            XDocument doc = new XDocument();
            XElement classNode = new XElement(Instance.GetType().Name);
            foreach (var value in Properties.Values)
            {
                var prop = new XElement(value.PropertyName, value.Get());
                prop.Add(new XAttribute("Type", value.PropertyType));
                classNode.Add(prop);
            }
            doc.Add(classNode);
            return doc.ToString();
        }

        
        public static FastDynamic GetFastDynamic<T>(T entity) where T : class, new()
        {
            return new FastDynamic()
            {
                Properties = new ConcurrentDictionary<String, FastProperty>(ManagerGX.GetProps(entity.GetType().GetProperties(), entity)),
                Instance = entity
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in Properties)
            {
                yield return item;
            }
        }
    }
}
