using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;

namespace BigCookieKit.AspCore.Apm
{
    public abstract class ApmDbContext : DbContext
    {
        public ApmDbContext() { }

        public ApmDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public static Dictionary<Type, Action<DbContextOptionsBuilder>> _OnConfiguring = new Dictionary<Type, Action<DbContextOptionsBuilder>>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _OnConfiguring[GetType()]?.Invoke(optionsBuilder);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
