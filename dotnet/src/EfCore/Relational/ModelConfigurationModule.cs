
using System;
using Microsoft.EntityFrameworkCore;

namespace NerdyMishka.EfCore
{
    public class ModelConfigurationModule : IDisposable
    {
        protected string SchemaName { get; set; }

        private Action<ModelBuilder> seedData;
        private bool configurationCalled = false;
        private bool seedDataCalled = false;

        private bool disposed = false;

        public ModelConfigurationModule(
            string schemaName = null, 
            Action<ModelBuilder> seedData = null)
        {
            this.SchemaName = schemaName;
            this.seedData = seedData;
        }

        public virtual void Apply(ModelBuilder builder)
        {
            this.ApplyConfiguration(builder);
            this.ApplySeedData(builder);
        }

        public virtual void ApplyConfiguration(ModelBuilder builder)
        {
            if(this.configurationCalled)
                return;

            this.OnModelCreating(builder);

            this.configurationCalled = true;
        }

        protected virtual void OnModelCreating(ModelBuilder builder)
        {

        }

        public virtual void ApplySeedData(ModelBuilder builder)
        {
            if(this.seedDataCalled)
                return;

            if(this.seedData != null)
                this.seedData(builder);

            this.seedDataCalled = true;
        }

        public void Dispose()
        {
            if(this.disposed)
                return;

            this.Dispose(true);
            this.disposed = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            
        }
    }
}