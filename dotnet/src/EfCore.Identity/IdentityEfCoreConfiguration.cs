using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using NerdyMishka.EfCore;

namespace NerdyMishka.EfCore.Identity
{

    public class IdentityEfCoreConfiguration : NerdyMishka.EfCore.NerdyMishkaEntityTypeConfiguration, 
        IEntityTypeConfiguration<User>, 
        IEntityTypeConfiguration<PasswordLogin>
    {
        public IdentityEfCoreConfiguration(string prefix, bool supportsSchema = true, string schema = "membership")
        {
            this.TablePrefix = prefix;
            this.SupportsSchema = true;
            this.Schema = schema;
        }

        public virtual void Configure(EntityTypeBuilder<User> builder)
        {
            var conventions = new NerdyMishkaEntityTypeConventions<User>(this, builder);


            conventions.HasDependant(
                o => o.PasswordLogin, 
                o => o.User, 
                o => o.UserId);
        }

        public virtual void Configure(EntityTypeBuilder<PasswordLogin> builder)
        {
            var conventions = new NerdyMishkaEntityTypeConventions<PasswordLogin>(this, builder);
            conventions
                .SetPrimaryKey(o => o.UserId, false)
                .UpdateTable()
                .UpdateProperty(o => o.PasswordExpiresAt)
                .UpdateProperty(o => o.FailureCount)
                .UpdateProperty(o => o.IsLockedOut)
                .UpdateProperty(o => o.LastIpAddress)
                .UpdateProperty(o => o.LastLoginAt)
                .UpdateProperty(o => o.LockOutStartedAt)
                .UpdateProperty(o => o.Password, (p) => p.HasMaxLength(500));
        }

       
    }
}