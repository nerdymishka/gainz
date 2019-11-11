using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace DankInventory.Data
{
    public class DankDbContext : DbContext
    {
       
        public DankDbContext([NotNullAttribute] DbContextOptions options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new DankDbConfiguration().Apply(modelBuilder);
        }
    }
}