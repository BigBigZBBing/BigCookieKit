using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Reflect
{
    public class FastExtend
    {
        private ConcurrentDictionary<string, Type> allMember;

        private bool isGenerate = false;

        private SmartBuilder globalBuilder;

        private int version = 1;

        private object instance;

        private FastDynamic _dynamic;

        public object Instance
        {
            get
            {
                CheckGenerate();
                return instance;
            }
        }

        public FastExtend()
        {
            allMember = new ConcurrentDictionary<string, Type>();
        }

        public object this[string name]
        {
            get
            {
                CheckGenerate();
                if (_dynamic != null)
                {
                    return _dynamic[name];
                }
                throw new NullReferenceException();
            }
            set
            {
                CheckGenerate();
                _dynamic[name] = value;
            }
        }

        private void CheckGenerate()
        {
            if (isGenerate)
            {
                globalBuilder = new SmartBuilder();
                globalBuilder.Class("anonymity" + version);
                foreach (var item in allMember)
                {
                    var prop = globalBuilder.Property(item.Key, item.Value);
                    if (_dynamic != null && _dynamic.Properties.ContainsKey(item.Key))
                        prop.Constant(_dynamic[item.Key]);
                }
                instance = globalBuilder.Generation();
                _dynamic = FastDynamic.GetFastDynamic(instance);
                version++;
                isGenerate = false;
            }
        }

        public void AddMember(string name, Type type)
        {
            if (!allMember.ContainsKey(name))
            {
                if (allMember.TryAdd(name, type))
                {
                    isGenerate = true;
                }
            }
        }

        public void RemoveMember(string name)
        {
            if (allMember.ContainsKey(name))
            {
                if (allMember.TryRemove(name, out _))
                {
                    isGenerate = true;
                }
            }
        }

    }
}
