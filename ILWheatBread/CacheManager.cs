using ILWheatBread.SmartEmit;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ILWheatBread
{
    internal static class CacheManager
    {
        internal static Dictionary<String, FastProperty[]> EntityCache => new Dictionary<String, FastProperty[]>();

        internal static Boolean retValue { get; set; }

        internal static FastProperty[] CachePropsManager(this Type type)
        {
            if (!EntityCache.ContainsKey(type.FullName))
            {
                EntityCache.Add(type.FullName, EnumerableProp(type).ToArray());
            }
            return EntityCache[type.FullName];
        }

        static IEnumerable<FastProperty> EnumerableProp(Type type)
        {
            foreach (var prop in type.GetProperties())
            {
                yield return new FastProperty(prop);
            }
        }
    }
}
