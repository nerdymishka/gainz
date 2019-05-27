using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Humanizer;
using System;
using System.Linq.Expressions;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace NerdyMishka.EfCore.Metadata
{
    public static class NerdyMishkaModelBuilderExtensions
    {

        public static ModelBuilder ApplyNamingConventions(this ModelBuilder builder, IConstraintConventions conventions)
        {
            if(conventions == null)
                return builder;

                
            // first loop is to set all the table and column names
            // if this is set, you don't have to format them twice and
            // since the implementation of humanizer may change and
            // it may not use stringbuilder under the cover or allows you
            // to chain conversions, its best to keep this minimal.
            foreach (var et in builder.Model.GetEntityTypes())
            {
                var entityType = et.Relational();
                entityType.TableName = conventions.GetDefaultTableName(et);
                if(!string.IsNullOrWhiteSpace(entityType.Schema))
                    entityType.Schema = conventions.GetDefaultSchemaName(et);

                foreach (var p in et.GetProperties())
                {
                    var property = p.Relational();
                    property.ColumnName = conventions.GetDefaultName(p);
                }
            }

            foreach(var et in builder.Model.GetEntityTypes())
            {
                foreach (var ix in et.GetIndexes())
                {
                    ix.Relational().Name = conventions.GetDefaultName(ix);
                }

                foreach(var pk in et.GetKeys())
                {
                    pk.Relational().Name = conventions.GetDefaultName(pk);
                }

                foreach (var p in et.GetProperties())
                {
                    foreach (var fk in et.FindForeignKeys(p))
                    {
                        fk.Relational().Name = conventions.GetDefaultName(fk);
                    }
                }
            }

            return builder;
        }
    }
}