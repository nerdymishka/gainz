

using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace NerdyMishka.EfCore
{
    public static class DbContextOptionsBuilderExtenions
    {
        
        /// <summary>
        /// Replaces the Migration Table.  
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="migrationTableName"></param>
        /// <param name="migrationSchemaName"></param>
        /// <typeparam name="TProviderRespository"></typeparam>
        /// <typeparam name="TProviderReplacement"></typeparam>
        /// <returns></returns>
        public static void UseHistoryRespository<TProviderRespository, TProviderReplacement>(
            this DbContextOptionsBuilder builder,
            string migrationTableName = null, 
            string migrationSchemaName = null)
            where TProviderReplacement: TProviderRespository
            where TProviderRespository: IHistoryRepository
        {
            if(!string.IsNullOrWhiteSpace(migrationTableName))
            {
                var opts = RelationalOptionsExtension.Extract(builder.Options);
                opts.WithMigrationsHistoryTableName(migrationTableName);

                if(!string.IsNullOrWhiteSpace(migrationSchemaName))
                    opts.WithMigrationsHistoryTableSchema(migrationSchemaName);
            }

            builder.ReplaceService<TProviderRespository, TProviderReplacement>();
        }


        /// <summary>
        /// Hoists the ModelConfiguration to service configration. 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T"></typeparam>
        public static void UseModelConfiguration<T>(this DbContextOptionsBuilder<T> builder, 
            Action<ModelBuilder> configure) where T: DbContext
        {
            var configurationSet = new Microsoft.EntityFrameworkCore.Metadata.Conventions.ConventionSet();
            var mb = new ModelBuilder(configurationSet);
            if(configure != null)
                configure(mb);
                
            builder.UseModel(mb.FinalizeModel());
        }


        public static void UseModelConfiguration(this DbContextOptionsBuilder builder, 
            ConventionSet conventionSet,  Action<ModelBuilder> configure) 
        {
            // each provider has it's own convention set with a static method "Build"
            // Most are located under  Microsoft.EntityFrameworkCore.Metadata.Conventions
            // e.g.
            //     Microsoft.EntityFrameworkCore.Metadata.Conventions.SqliteConventionSetBuilder.Build()
            // InMemory's is slightly different e.g.
            //     Microsoft.EntityFrameworkCore.InMemory.Metadata.Conventions.InMemoryConventionSetBuilder.Build()

            var mb = new ModelBuilder(conventionSet);
            if(configure != null)
                configure(mb);
                
            builder.UseModel(mb.FinalizeModel());
        }
    }
}