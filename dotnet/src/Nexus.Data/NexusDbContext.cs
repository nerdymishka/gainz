using System;
using Microsoft.EntityFrameworkCore;

namespace Nexus.Data
{
    public class NexusDbContext : DbContext
    {
        public DbSet<Group> Groups { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<UserApiKey> UserApiKeys { get; set; }

        public DbSet<UserApiKeyRole> UserApiKeyRoles {get; set; }

        public DbSet<Role> Roles { get; set; }
        
        public DbSet<RoleGroup> RoleGroups { get; set; }

        public DbSet<RoleResource> RoleResources { get; set; }

        public DbSet<RoleUser> RoleUsers { get; set; }

        public DbSet<OperationalEnvironment> OperationalEnvironments { get; set;}

        public DbSet<OperationalEnvironmentResource> OperationalEnvironmentResource {get; set;}

        public DbSet<Resource> Resources { get; set;}

        public DbSet<ConfigurationFile> ConfigurationFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        { 
            var builder = new Nexus.Data.DefaultModelBuilder();
            builder.Configure(modelBuilder.Entity<GroupUser>());
            builder.Configure(modelBuilder.Entity<OperationalEnvironmentResource>());
            builder.Configure(modelBuilder.Entity<RoleGroup>());
            builder.Configure(modelBuilder.Entity<RoleResource>());
            builder.Configure(modelBuilder.Entity<RoleUser>());
            builder.Configure(modelBuilder.Entity<UserApiKeyRole>());
        }

        public NexusDbContext(DbContextOptions options):base(options)
        {
            
        }
    }

}
