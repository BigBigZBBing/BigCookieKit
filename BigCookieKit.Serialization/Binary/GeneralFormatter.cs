using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace BigCookieKit.Serialization.Binary
{
    public class GeneralFormatter : FormatterBase
    {
        private Dictionary<Type, MethodInfo> ReadCache { get; set; }

        private Dictionary<Type, MethodInfo> WriteCache { get; set; }

        public Binary Host { get; set; }

        public int Priority { get; set; } = 1;

        public GeneralFormatter()
        {
            WriteCache = new Dictionary<Type, MethodInfo>();
            foreach (var item in typeof(BinaryWriter).GetMethods())
            {
                if (item.Name == "Write")
                {
                    var parameters = item.GetParameters();
                    if (parameters.Length == 1)
                    {
                        WriteCache.Add(parameters[0].ParameterType, item);
                    }
                }
            }

            ReadCache = new Dictionary<Type, MethodInfo>();
            foreach (var type in WriteCache.Keys)
            {
                var read = typeof(BinaryReader).GetMethod($"Read{type.Name.Replace("[]", "s")}");
                if (read != null)
                {
                    ReadCache.Add(type, read);
                }
            }
        }

        public object Read(Type type)
        {
            if (ReadCache.ContainsKey(type))
            {
                BinaryReader read = new BinaryReader(Host.Stream);
                return ReadCache[type].Invoke(read, null);
            }

            return Activator.CreateInstance(type);
        }

        public bool Write(object value)
        {
            var type = value.GetType();

            if (WriteCache.ContainsKey(type))
            {
                BinaryWriter writer = new BinaryWriter(Host.Stream);
                WriteCache[type].Invoke(writer, new object[] { value });
            }

            return false;
        }
    }
}
