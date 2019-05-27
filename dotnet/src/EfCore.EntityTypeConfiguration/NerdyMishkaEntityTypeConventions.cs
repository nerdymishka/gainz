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

namespace NerdyMishka.EfCore
{
    public class NerdyMishkaEntityTypeConventions<TModel> where TModel : class
    {
        private string tableName;

        private string primaryKey;

        private string[] primaryKeys;

        private NerdyMishkaEntityTypeConfiguration configuration;

        private EntityTypeBuilder<TModel> builder;


        public NerdyMishkaEntityTypeConventions(
            string tableName, 
            NerdyMishkaEntityTypeConfiguration configuration, 
            EntityTypeBuilder<TModel> builder) 
        {
            this.tableName = this.GetTableName(tableName);
            this.configuration = configuration;
            this.builder = builder;
        }

         public NerdyMishkaEntityTypeConventions(
             NerdyMishkaEntityTypeConfiguration configuration, 
             EntityTypeBuilder<TModel> builder) : this(null, configuration, builder)
        {
            this.tableName = this.GetTableName(builder.Metadata.Name);
        }

        public virtual string GetPrimaryKeyConstraintName() => $"pk_{this.tableName}";

        public virtual string GetPrimaryKeyConstraintName(string tableName) => $"pk_{this.GetTableName(tableName)}";

        public virtual string GetCustomForeignKeyName(string name) => $"fk_{name}";

        public NerdyMishkaEntityTypeConventions<TModel> SetString(
            Expression<Func<TModel, String>> propertyExpression,
            int? maxLength = null,
            Action<PropertyBuilder<string>> chain = null)
        {
            var p = this.Property(propertyExpression);
            if(maxLength.HasValue)
                p = p.HasMaxLength(maxLength.Value);

            if(chain != null)
                chain(p);

            return this;
        }

    

        public PropertyBuilder<TProperty> Property<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression)
        {
            var p = this.builder.Property(propertyExpression);
            p = p.HasColumnName(this.GetColumnName(p.Metadata.Name));

            return p;
        }

        public NerdyMishkaEntityTypeConventions<TModel> UpdateProperty<TProperty>(
            Expression<Func<TModel, TProperty>> propertyExpression,
            Action<PropertyBuilder<TProperty>> chain = null) {
        
            var p = this.Property(propertyExpression);

            if(chain != null)
                chain(p);

            return this;
        }

        public virtual string GetForeignKeyConstraintName(
            Type entity, 
            string columnName = null) {

             
                
            return this.GetForeignKeyConstraintName(
                entity.Name,
                columnName);
        }

        public virtual string GetForeignKeyConstraintName(
            PropertyBuilder entity, 
            PropertyBuilder column = null) {
                
            string columnName = null;
            if(column != null)
                columnName = column.Metadata.Name;

            return this.GetForeignKeyConstraintName(
                entity.Metadata.PropertyInfo.PropertyType.Name,
                columnName);
        }
      

        public string GetForeignKeyConstraintName(string foreignTableName, params string[] columnNames)
        {
            foreignTableName = this.GetTableName(foreignTableName);

            if(columnNames == null || columnNames.Length == 0)
                return $"fk_{this.tableName}_{foreignTableName}";

            string columns = string.Join("_", columnNames.Select(o => this.GetColumnName(o)));

            return $"fk_{this.tableName}_{foreignTableName}_{columns}";
        }

        public virtual string GetTableName(string name)
        {
            var prefix = this.configuration.TablePrefix;
            var tableName = name;
            if(tableName.Contains("."))
                tableName = tableName.Substring(tableName.LastIndexOf(".") + 1);
            
            tableName = tableName.Pluralize().Underscore();
            return string.IsNullOrWhiteSpace(prefix) ? name : $"{prefix}_{name}";
        }

        public virtual string GetColumnName(string name) => name.Underscore();

        public NerdyMishkaEntityTypeConventions<TModel> UpdateTable()
        {
            
            if(this.configuration.SupportsSchema)
                this.builder.ToTable(tableName, this.configuration.Schema);
            else 
                this.builder.ToTable(tableName);

            return this;
        }
        
        /// <summary>
        /// Defines a dependant one-to-one relationship where the primary key
        /// 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TRelatedEntity"></typeparam>
        public virtual ReferenceReferenceBuilder<TModel, TRelatedEntity> HasDependant<TRelatedEntity>(
            Expression<Func<TModel, TRelatedEntity>> dependant,
            Expression<Func<TRelatedEntity, TModel>> parent,
            Expression<Func<TRelatedEntity, object>> fkExpression = null
        ) where TRelatedEntity : class {

            var p = this.builder
                .HasOne(dependant)
                .WithOne(parent)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName(this.GetCustomForeignKeyName(typeof(TRelatedEntity).Name));

            if(fkExpression == null)
            {
                var fk = typeof(TModel).Name + "Id";
                return p.HasForeignKey<TRelatedEntity>(fk);
            }

            return p.HasForeignKey<TRelatedEntity>(fkExpression);
        }

        public virtual NerdyMishkaEntityTypeConventions<TModel> SetPrimaryKey(Expression<Func<TModel, Object>> keyExpression, bool increment = true)
        {
            var k = this.builder.HasKey(keyExpression);
            var keyName = k.Metadata.Properties[0].Name;

            k.HasName(this.GetPrimaryKeyConstraintName());
            var p = this.builder.Property(keyName)
                .HasColumnName(keyName.Underscore());

            if(!increment)
               p =  p.ValueGeneratedNever();

            return this;
        }

     


        public virtual  NerdyMishkaEntityTypeConventions<TModel> HasOne(PropertyBuilder builder, params string[] referenceColumns) {
            var propInfo = builder.Metadata.PropertyInfo;

            if(this.configuration.SupportsForeignKeys) {
                this.builder.HasOne(propInfo.PropertyType, propInfo.Name)
                    .WithOne()
                    .HasForeignKey(typeof(TModel), referenceColumns);
            }

            return this;
        }

        public virtual  NerdyMishkaEntityTypeConventions<TModel> HasOneToOne(PropertyBuilder builder, string parentNavigationProperty, params string[] referenceColumns) {
            var propInfo = builder.Metadata.PropertyInfo;

            if(this.configuration.SupportsForeignKeys) {
                this.builder.HasOne(propInfo.PropertyType, propInfo.Name)
                    .WithOne(parentNavigationProperty)
                    .HasForeignKey(typeof(TModel), referenceColumns);
            }

            return this;
        }

        public virtual ReferenceReferenceBuilder<TModel, TRelatedEntity> HasOneToOne<TRelatedEntity>(
            Expression<Func<TModel, TRelatedEntity>> hasOne, 
            Expression<Func<TRelatedEntity, TModel>> withOne) where TRelatedEntity: class {
            
            string keyName = "Id";
            var key = this.builder.Metadata.GetKeys().FirstOrDefault();
            if(key != null && key.Properties != null && key.Properties.Count > 0)
                keyName = key.Properties[0].Name;
            
            var fk = typeof(TModel).Name + keyName;

            return this.builder.HasOne(hasOne)
                .WithOne(withOne)
                .HasForeignKey(typeof(TModel), fk)
                .HasConstraintName(this.GetForeignKeyConstraintName(typeof(TRelatedEntity)));
        }

        public class DateTimeUtcGenerator : ValueGenerator<DateTime>
        {
            public override bool GeneratesTemporaryValues => true;

            public override DateTime Next(EntityEntry entry)
            {
                return DateTime.UtcNow;
            }

            
        }

        public virtual NerdyMishkaEntityTypeConventions<TModel> UpdateModifiedBy()
        {
            this.builder.Property("UpdatedBy")
                .HasColumnName("updated_by");

            return this;
        }

         public virtual NerdyMishkaEntityTypeConventions<TModel> UpdateCreatedBy()
        {
            this.builder.Property("CreatedBy")
                .HasColumnName("created_by");

            return this;
        }

        public virtual NerdyMishkaEntityTypeConventions<TModel> UpdateModifiedAt()
        {
            this.builder.Property("UpdatedAt")
                .HasColumnName("updated_at")
                .ValueGeneratedOnUpdate()
                .HasValueGenerator<DateTimeUtcGenerator>();

            return this;
        }

        public virtual NerdyMishkaEntityTypeConventions<TModel> UpdateCreatedAt()
        {
            this.builder.Property("CreatedAt")
                .HasColumnName("created_at")
                .ValueGeneratedOnUpdate()
                .HasValueGenerator<DateTimeUtcGenerator>();

            return this;
        }

        public virtual NerdyMishkaEntityTypeConventions<TModel> UpdateAuditColumns()
        {
            return this.UpdateCreatedAt()
                .UpdateCreatedBy()
                .UpdateModifiedAt()
                .UpdateModifiedBy();
        }
    }
}