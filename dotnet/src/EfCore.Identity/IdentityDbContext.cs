using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NerdyMishka.EfCore.Metadata;

namespace NerdyMishka.EfCore.Identity
{
    public class IdentityDbContext : DbContext
    {

        public IdentityDbContext(DbContextOptions options) : base(options) {
            
        }

        protected NerdyMishkaOptionsExtension Options { get; set; }

        public DbSet<ApiKey> ApiKey { get; set; }

        public DbSet<ApiKeyRole> ApiKeyRoles { get; set; }

        public DbSet<EmailAddress> EmailAddresses { get; set; }

        public DbSet<Phone> Phones { get; set; }

        public DbSet<PasswordLogin> PasswordLogins { get; set; }

        public DbSet<PasswordPolicy> PasswordPolicies { get; set;}

        public DbSet<Domain> Domains { get; set; }

        public DbSet<Organization> Organizations { get; }

        public DbSet<OrganizationUser> OrganizationUsers { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<RolePermission> RolePermissions { get; set;}

        public DbSet<MultiFactorPolicy> MfaPolicies { get; set; }

        public DbSet<Role> Roles { get; set;}

        public DbSet<User> Users { get; set; }

        public DbSet<UserRole> UserRoles  {get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNerdyMishkaEfCore(x => {
                x.UseNerdyMishkaConventions();
                x.SetDefaultSchemaName("nerdymishka");
                x.SetMigrationsHistoryTable("IdentityMigration", "nerdymishka");
            });
            this.Options = NerdyMishkaOptionsExtension.Extract(optionsBuilder.Options);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var configuration = new IdentityEfCoreConfiguration();
            configuration.Apply(modelBuilder);

            var conventions = new List<IEfCoreConvention>();
            conventions.AddRange(this.Options.NamingConventions.Conventions);
            
            modelBuilder.ApplyNerdyMishkaConventions(conventions);
        }
    }
}