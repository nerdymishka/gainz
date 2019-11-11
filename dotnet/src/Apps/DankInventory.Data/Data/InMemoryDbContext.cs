using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace DankInventory.Data
{
    public class InMemoryDbContext : DankDbContext
    {
        public InMemoryDbContext([NotNull] DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseInMemoryDatabase("DankTesting");
        }
    }
}