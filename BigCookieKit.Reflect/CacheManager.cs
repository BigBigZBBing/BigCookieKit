using System;
using System.Collections.Generic;
using System.Linq;

namespace BigCookieKit.Reflect
{
    internal static class CacheManager
    {
        internal static Dictionary<string, FastProperty[]> EntityCache => new Dictionary<string, FastProperty[]>();

        internal static FastProperty[] CachePropsManager(this Type type)
        {
            if (!EntityCache.ContainsKey(type.FullName))
            {
                EntityCache.Add(type.FullName, EnumerableProp(type).ToArray());
            }
            return EntityCache[type.FullName];
        }

        private static IEnumerable<FastProperty> EnumerableProp(Type type)
        {
            foreach (var prop in type.GetProperties())
            {
                yield return new FastProperty(prop);
            }
        }
    }
}