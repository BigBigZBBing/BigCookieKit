using Microsoft.EntityFrameworkCore;

using System;

namespace BigCookieKit.AspCore.Apm
{
    public abstract class ApmDbContext : DbContext
    {
        public ApmDbContext() { }

        public ApmDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public static Action<DbContextOptionsBuilder> _OnConfiguring { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _OnConfiguring?.Invoke(optionsBuilder);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
