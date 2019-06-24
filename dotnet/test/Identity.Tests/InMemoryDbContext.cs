using Microsoft.EntityFrameworkCore;

namespace NerdyMishka.Identity.Tests
{
    public class InMemoryDbContext : NerdyMishka.EfCore.Identity.IdentityDbContext
    {

        public InMemoryDbContext(DbContextOptions options) : base(options) {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("Identity_Memory_Test");
            base.OnConfiguring(optionsBuilder);
        }

    }
}