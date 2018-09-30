using System;
using Microsoft.EntityFrameworkCore;

namespace Nexus.Data
{
    public class NexusDbContext : DbContext
    {
        public DbSet<GroupRecord> Groups { get; set; }

        public DbSet<GroupUserRecord> GroupUsers { get; set; }

        public DbSet<UserRecord> Users { get; set; }

        public DbSet<UserApiKeyRecord> UserApiKeys { get; set; }

        public DbSet<UserApiKeyRoleRecord> UserApiKeyRoles {get; set; }

        public DbSet<RoleRecord> Roles { get; set; }
        
        public DbSet<RoleGroupRecord> RoleGroups { get; set; }

        public DbSet<RoleResourceRecord> RoleResources { get; set; }

        public DbSet<RoleUserRecord> RoleUsers { get; set; }

        public DbSet<OperationalEnvironmentRecord> OperationalEnvironments { get; set;}

        public DbSet<OperationalEnvironmentResourceRecord> OperationalEnvironmentResource {get; set;}

        public DbSet<ResourceRecord> Resources { get; set;}

        public DbSet<ConfigurationFileRecord> ConfigurationFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        { 
            var builder = new Nexus.Data.DefaultModelBuilder();
            builder.Configure(modelBuilder.Entity<GroupUserRecord>());
            builder.Configure(modelBuilder.Entity<OperationalEnvironmentResourceRecord>());
            builder.Configure(modelBuilder.Entity<RoleGroupRecord>());
            builder.Configure(modelBuilder.Entity<RoleResourceRecord>());
            builder.Configure(modelBuilder.Entity<RoleUserRecord>());
            builder.Configure(modelBuilder.Entity<UserApiKeyRoleRecord>());
        }

        public NexusDbContext(DbContextOptions options):base(options)
        {
            
        }
    }

}
