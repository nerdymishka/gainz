

using Mettle;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NerdyMishka.EfCore;
using NerdyMishka.EfCore.Identity;

namespace Tests 
{




    public class IdentityDbContextSchemaTests
    {
        [UnitTest]
        public void Test(IAssert assert)
        {
            var collection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            collection.AddDbContext<IdentityDb>((builder) => {
                builder.UseInMemoryDatabase("DataSource=:memory:");
                builder.UseModelConfiguration(
                    Microsoft.EntityFrameworkCore.InMemory.Metadata.Conventions.InMemoryConventionSetBuilder.Build(),
                    (mb) => {
                        var module = new IdentityEfCoreConfiguration(null, null);
                        module.Configure(mb.Entity<User>());
                        module.Configure(mb.Entity<Permission>());
                        module.Configure(mb.Entity<Role>());
                        module.Configure(mb.Entity<Organization>());
                        module.Configure(mb.Entity<PasswordPolicy>());
                        module.Configure(mb.Entity<PasswordLogin>());
                        module.Configure(mb.Entity<ApiKey>());
                        module.Configure(mb.Entity<Domain>());
                        module.Configure(mb.Entity<EmailAddress>());
                        module.Configure(mb.Entity<ApiKeyRole>());
                        module.Configure(mb.Entity<RolePermission>());
                        module.Configure(mb.Entity<UserRole>());
                        module.Configure(mb.Entity<UserClaim>());
                        module.Configure(mb.Entity<UserToken>());
                        module.Configure(mb.Entity<UserLogin>());
                        module.Configure(mb.Entity<RoleClaim>()); 

                        SeedData.ApplyUsers(mb);
                }); 
            });

            var sp = collection.BuildServiceProvider();
            var db = sp.GetService<IdentityDb>();
            assert.NotNull(db);

            // if there are any issues, this will throw
            db.Database.EnsureCreated();
        }

        public class IdentityDb : DbContext 
        {
            
            public IdentityDb(DbContextOptions options) : base(options) {
                
            }
        }
    }
}