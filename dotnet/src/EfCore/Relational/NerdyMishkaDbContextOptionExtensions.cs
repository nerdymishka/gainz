/*
using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using NerdyMishka.EfCore.Infrastructure;
using NerdyMishka.EfCore.Migrations;

namespace Microsoft.EntityFrameworkCore
{
    public static class NerdyMishkaDbContextOptionExtensions
    {
        public static DbContextOptionsBuilder UseNerdyMishkaEfCore(
            this DbContextOptionsBuilder optionsBuilder,
            Action<NerdyMishkaDbContextOptionsBuilder> nerdyMishkaOptionsAction = null)
        {
            if(optionsBuilder == null)
                throw new ArgumentNullException(nameof(optionsBuilder));


            var extension = (RelationalOptionsExtension)GetOrCreateExtension(optionsBuilder);
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            nerdyMishkaOptionsAction?.Invoke(new NerdyMishkaDbContextOptionsBuilder(optionsBuilder));

            optionsBuilder.ReplaceService<IHistoryRepository, NerdyMishkaHistoryRepository>();

            return optionsBuilder;
        }

         private static RelationalOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.Options.FindExtension<RelationalOptionsExtension>()
               ?? new RelationalOptionsExtension();
    }
   
}*/