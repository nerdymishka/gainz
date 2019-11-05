

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace NerdyMishka.EfCore.Metadata 
{
    

    public class NerdyMishkaConstraintConventions : IConstraintConventions
    {
        public string ForeignKeyPrefix  { get; set; } = "fk_";

        public string IndexPrefix { get; set; } = "ix_";

        public string AlternateKeyPrefix { get; set; } = "ak_";

        public string PrimaryKeyPrefix { get; set;} = "pk_";

        private readonly List<IEfCoreConvention> conventions;

        public IEnumerable<IEfCoreConvention> Conventions => conventions;

        public class EntityTypeNamingConvention : IRelationalEntityTypeConvention
        {
            private IConstraintConventions conventions;
            public EntityTypeNamingConvention(IConstraintConventions conventions)
            {
                this.conventions = conventions;
            }

            public void Apply(RelationalEntityTypeAnnotations annotations)
            {
            
                annotations.TableName = conventions.FormatTableName(annotations.TableName);

                if(!string.IsNullOrWhiteSpace(annotations.Schema))
                    annotations.Schema = conventions.FormatSchemaName(annotations.Schema);
            }
        }

        public class PropertyNamingConvention : IRelationalPropertyConvention
        {
            private IConstraintConventions conventions;
            public PropertyNamingConvention(IConstraintConventions conventions)
            {
                this.conventions = conventions;
            }
            public void Apply(RelationalPropertyAnnotations annotations)
            {
                annotations.ColumnName = this.conventions.FormatColumnName(annotations.ColumnName);
            }
        }

        public class IndexNamingConvention : IIndexConvention
        {
            private IConstraintConventions conventions;
            public IndexNamingConvention(IConstraintConventions conventions)
            {
                this.conventions = conventions;
            }
            public void Apply(IMutableIndex index)
            {
                var r = index.Relational();
                r.Name = conventions.GetDefaultName(index);
            }
        }

        public class ForeignKeyNamingConvention : IForeignKeyConvention
        {
            private IConstraintConventions conventions;
            public ForeignKeyNamingConvention(IConstraintConventions conventions)
            {
                this.conventions = conventions;
            }
            public void Apply(IMutableForeignKey fk)
            {
                var r = fk.Relational();
                r.Name = conventions.GetDefaultName(fk);
            }
        }

        public class KeyNamingConvention : IKeyConvention
        {
            private IConstraintConventions conventions;
            public KeyNamingConvention(IConstraintConventions conventions)
            {
                this.conventions = conventions;
            }
            public void Apply(IMutableKey key)
            {
                var r = key.Relational();
                r.Name = conventions.GetDefaultName(key);
            }
        }

        public NerdyMishkaConstraintConventions()
        {
            this.conventions = new List<IEfCoreConvention>() {
                new EntityTypeNamingConvention(this),
                new PropertyNamingConvention(this),
                new KeyNamingConvention(this),
                new IndexNamingConvention(this),
                new ForeignKeyNamingConvention(this)
            };
        }

        public virtual string FormatTableName(string tableName)
        {
            if(tableName == null)
                return tableName;

            return tableName.Pluralize().Underscore();
        }

        public virtual string FormatSchemaName(string schemaName)
        {
            if(schemaName == null)
                return schemaName;

            return schemaName.Underscore();
        }

        public virtual string FormatColumnName(string columnName)
        {
            if(columnName == null)
                return columnName;

            return columnName.Underscore();
        }

        public string GetDefaultTableName(IEntityType entityType)
        {
            return FormatTableName(entityType.Relational().TableName);
        }

        public string GetDefaultSchemaName(IEntityType entityType)
        {
            var s = entityType.Relational().Schema;
            if(s == null)
                return s;

            return FormatSchemaName(entityType.Relational().Schema);
        }


        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string GetDefaultName(IForeignKey foreignKey)
        {
            var baseName = new StringBuilder()
                .Append(ForeignKeyPrefix)
                .Append(foreignKey.DeclaringEntityType.Relational().TableName)
                .Append("__")
                .Append(foreignKey.PrincipalEntityType.Relational().TableName)
                .Append("__")
                .AppendJoin(foreignKey.Properties.Select(p => p.Relational().ColumnName), "__")
                .ToString();

            return Truncate(baseName, null, foreignKey.DeclaringEntityType.Model.GetMaxIdentifierLength());
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string GetDefaultName(IIndex index)
        {
            var baseName = new StringBuilder()
                .Append(IndexPrefix)
                .Append(index.DeclaringEntityType.Relational().TableName)
                .Append("__")
                .AppendJoin(index.Properties.Select(p => p.Relational().ColumnName), "__")
                .ToString();

            return Truncate(baseName, null, index.DeclaringEntityType.Model.GetMaxIdentifierLength());
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string GetDefaultName(IKey key)
        {
            var sharedTablePrincipalPrimaryKeyProperty = key.Properties[0].FindSharedTableRootPrimaryKeyProperty();
            if (sharedTablePrincipalPrimaryKeyProperty != null)
            {
                return  sharedTablePrincipalPrimaryKeyProperty.GetContainingPrimaryKey().Relational().Name;
            }

            var builder = new StringBuilder();
            var tableName = key.DeclaringEntityType.Relational().TableName;

            if (key.IsPrimaryKey())
            {
                builder
                    .Append(PrimaryKeyPrefix)
                    .Append(tableName);
            }
            else
            {
                builder
                    .Append(AlternateKeyPrefix)
                    .Append(tableName)
                    .Append("_")
                    .AppendJoin(key.Properties.Select(p => p.Relational().ColumnName), "__");
            }

            return Truncate(builder.ToString(), null, key.DeclaringEntityType.Model.GetMaxIdentifierLength());
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string GetDefaultName(IProperty property)
        {
            var sharedTablePrincipalPrimaryKeyProperty = property.FindSharedTableRootPrimaryKeyProperty();
            if (sharedTablePrincipalPrimaryKeyProperty != null)
            {
                return sharedTablePrincipalPrimaryKeyProperty.Relational().ColumnName;
            }

            var entityType = property.DeclaringEntityType;
            StringBuilder builder = null;
            do
            {
                var ownership = entityType.GetForeignKeys().SingleOrDefault(fk => fk.IsOwnership);
                if (ownership == null)
                {
                    entityType = null;
                }
                else
                {
                    var ownerType = ownership.PrincipalEntityType;
                    var entityTypeAnnotations = entityType.Relational();
                    var ownerTypeAnnotations = ownerType.Relational();
                    if (entityTypeAnnotations.TableName == ownerTypeAnnotations.TableName
                        && entityTypeAnnotations.Schema == ownerTypeAnnotations.Schema)
                    {
                        if (builder == null)
                        {
                            builder = new StringBuilder();
                        }

                        builder.Insert(0, "_");
                        builder.Insert(0, FormatColumnName(ownership.PrincipalToDependent.Name));
                        entityType = ownerType;
                    }
                    else
                    {
                        entityType = null;
                    }
                }
            }
            while (entityType != null);

            var baseName = FormatColumnName(property.Name);
            if (builder != null)
            {
                builder.Append(property.Name);
                baseName = builder.ToString();
            }

            return Truncate(baseName, null, property.DeclaringEntityType.Model.GetMaxIdentifierLength());
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static string Truncate(string name, int? uniquifier, int maxLength)
        {
            var uniquifierLength = GetLength(uniquifier);
            var maxNameLength = maxLength - uniquifierLength;

            var builder = new StringBuilder();
            if (name.Length <= maxNameLength)
            {
                builder.Append(name);
            }
            else
            {
                builder.Append(name, 0, maxNameLength - 1);
                builder.Append("~");
            }

            if (uniquifier != null)
            {
                builder.Append(uniquifier.Value);
            }

            return builder.ToString();
        }

        private static int GetLength(int? number)
        {
            if (number == null)
            {
                return 0;
            }

            var length = 0;
            do
            {
                number /= 10;
                length++;
            }
            while (number.Value >= 1);

            return length;
        }
    }
}