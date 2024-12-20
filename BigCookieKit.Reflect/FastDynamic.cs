﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BigCookieKit.Reflect
{
    public sealed class FastDynamic : IEnumerable
    {
        private FastDynamic()
        {
        }

        public object this[string Name]
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

        public IDictionary<string, FastProperty> Properties { get; internal set; }

        internal object Instance { get; set; }

        public static FastDynamic GetFastDynamic<T>(T entity) where T : class, new()
        {
            return new FastDynamic()
            {
                Properties = new ConcurrentDictionary<string, FastProperty>(ManagerGX.GetProps(entity.GetType().GetProperties(), entity)),
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