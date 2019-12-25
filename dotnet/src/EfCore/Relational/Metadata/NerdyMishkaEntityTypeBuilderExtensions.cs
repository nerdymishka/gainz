
using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NerdyMishka.EfCore.Metadata
{

    public static class NerdyMishkaEntityTypeBuilderExtensions
    {

        public static EntityTypeBuilder<TModel> SetSchema<TModel>(
            this EntityTypeBuilder<TModel> builder,
            string schemaName 
        ) where TModel: class { 
            builder.Metadata.SetSchema(schemaName);
           
            return builder;
        }


        /// <summary>
        /// Defines a dependant one-to-one relationship where the primary key
        /// 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TRelatedEntity"></typeparam>
        public static ReferenceReferenceBuilder<TModel, TRelatedEntity> HasDependant<TModel, TRelatedEntity>(
                this EntityTypeBuilder<TModel> builder,
                Expression<Func<TModel, TRelatedEntity>> dependant,
                Expression<Func<TRelatedEntity, TModel>> parent,
                Expression<Func<TRelatedEntity, object>> fkExpression = null
            ) where TRelatedEntity : class 
              where TModel: class {
                
                if (builder == null)
                {
                    throw new ArgumentNullException(nameof(builder));
                }

                return builder
                    .HasOne(dependant)
                    .WithOne(parent)
                    .HasForeignKey(fkExpression)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            }
    }
}