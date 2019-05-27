

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NerdyMishka.EfCore.Metadata;

namespace NerdyMishka.EfCore.Infrastructure
{
    public class NerdyMishkaDbContextOptionsBuilder

    {
        protected virtual DbContextOptionsBuilder OptionsBuilder { get; }

        public NerdyMishkaDbContextOptionsBuilder(DbContextOptionsBuilder builder)
        {
            OptionsBuilder = builder;
        }

        public virtual NerdyMishkaDbContextOptionsBuilder SetTablePrefix(string tablePrefix) 
            => WithOption(e => e.WithTablePrefix(tablePrefix));

        public virtual NerdyMishkaDbContextOptionsBuilder SetDefaultSchemaName(string schemaName) 
            => WithOption(e => e.WithDefaultSchemaName(schemaName));

        public virtual NerdyMishkaDbContextOptionsBuilder SetConventions(IConstraintConventions conventions)
            => WithOption(e => e.WithConventions(conventions));

        public virtual NerdyMishkaDbContextOptionsBuilder UseNerdyMishkaConventions()
            => WithOption(e => e.WithConventions(new NerdyMishkaConstraintConventions()));

        public virtual NerdyMishkaDbContextOptionsBuilder SetMigrationsHistoryTable(string tableName, string schemaName = null)
        {
            if(string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException(nameof(tableName));
            
            if(schemaName != null && schemaName.Trim().Length == 0)
                throw new ArgumentNullException(nameof(schemaName));

            return WithOption(e => e.WithMigrationTableName(tableName)
                .WithMigrationSchemaName(schemaName));
        }
        
        protected virtual NerdyMishkaDbContextOptionsBuilder WithOption(
            Func<NerdyMishkaOptionsExtension, NerdyMishkaOptionsExtension> setAction)
        {
            ((IDbContextOptionsBuilderInfrastructure)OptionsBuilder).AddOrUpdateExtension(
                setAction(OptionsBuilder.Options.FindExtension<NerdyMishkaOptionsExtension>() ?? new NerdyMishkaOptionsExtension()));

            return this;
        }
    }
}