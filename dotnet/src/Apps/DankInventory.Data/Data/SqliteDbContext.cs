using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace DankInventory.Data
{
    public class SqliteDbContext : DankDbContext
    {
        public SqliteDbContext([NotNull] DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            var cs = @"data source=:memory:";
            optionsBuilder.UseSqlite(cs);
        }
    }
}