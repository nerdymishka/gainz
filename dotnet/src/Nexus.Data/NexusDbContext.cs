using System;
using Microsoft.EntityFrameworkCore;

namespace Nexus.Data
{
    public class NexusDbContext : DbContext
    {
        public DbSet<UserRecord> Users { get; set; }

        public DbSet<UserApiKeyRecord> UserApiKeys { get; set; }

        public DbSet<ConfigurationSetRecord> ConfigurationSets { get; set; }

        public DbSet<ConfigurationFileRecord> ConfigurationFiles { get; set; }

        public DbSet<OperationalEnvironmentRecord> OperationalEnvironments { get; set;}

        public DbSet<ResourceRecord> Resources { get; set;}

        public DbSet<ResourceKindRecord> ResourceKinds { get; set;}

        public DbSet<ProtectedBlobVaultRecord> Vaults { get; set; }

        public DbSet<ProtectedBlobRecord> ProtectedBlobs  {get; set; }

        public DbSet<PublicKeyRecord> PublicKeys { get; set ;}

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        { 
            var builder = new Nexus.Data.DefaultModelBuilder();

        }

        public NexusDbContext(DbContextOptions options):base(options)
        {
            
        }
    }

}
