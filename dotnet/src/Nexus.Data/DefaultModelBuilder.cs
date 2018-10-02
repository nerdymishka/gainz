using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Nexus.Data
{
    public class DefaultModelBuilder : 
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<ConfigurationFileRecord>,
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<OperationalEnvironmentRecord>,
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<OperationalEnvironmentResourceRecord>,
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<ResourceRecord>,
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<UserApiKeyRecord>
    {
       

        public void Configure(EntityTypeBuilder<ConfigurationFileRecord> builder)
        {
            
        }

        

        public void Configure(EntityTypeBuilder<OperationalEnvironmentRecord> builder)
        {
           
        }

        public void Configure(EntityTypeBuilder<OperationalEnvironmentResourceRecord> builder)
        {
            builder.HasKey("OperationalEnvironmentId", "ResourceId");
        }

        public void Configure(EntityTypeBuilder<ResourceRecord> builder)
        {
           
        }

       

        public void Configure(EntityTypeBuilder<UserApiKeyRecord> builder)
        {
            
        }

        
    }
}