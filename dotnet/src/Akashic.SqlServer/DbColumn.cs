using System;
using System.Linq;

namespace NerdyMishka.Data 
{
    public class DbColumn : DbObject
    {
        public virtual DbTable Table { get; internal protected set; }

       
        public virtual Type ClrType  { get; internal protected set; }

        public virtual string SqlType { get; internal protected set; }

        public virtual bool IsKey => this.PrimaryKey != null;

        public virtual bool IsUnique => this.Unique != null;

        protected virtual ReferenceConstraint Reference { get; set; }

        protected virtual PrimaryKeyConstraint PrimaryKey { get; set; }

        protected virtual UniqueConstraint Unique { get; set; }

        public virtual string DefaultValue { get; internal protected set; }

        public virtual bool Increment { get;  internal protected set; }

        public virtual bool IsNull { get; internal protected set; }

        public virtual int? Length { get; internal protected set; }

        public virtual int? Precision { get; internal protected set; }

        public virtual int? Scale { get; internal protected set; }

        public void Rename(string newName)
        {

        }

        public void Alter(
            int? length = null, 
            int? precision = null, 
            int? scale = null, 
            string defaultValue = null, 
            bool? isUnique = null,
            string uniqueName = null,
            bool? isNull = null)
        {
            var state = new DbColumn() {
                Name = this.Name,
                Length = this.Length,
                Scale = this.Scale,
                ClrType = this.ClrType,
                SqlType = this.SqlType,
                IsCreated = this.IsCreated,
                Unique = this.Unique,
                PrimaryKey = this.PrimaryKey,
                Increment = this.Increment,
                Reference = this.Reference
            };

            if(length.HasValue && length != this.Length) {
                state.Length = length;
                state.IsDirty = true;
            }

            if(scale.HasValue && scale != this.Scale) {
                state.Scale = scale;
                state.IsDirty = true;
            }

            if(precision.HasValue && precision != this.Precision) {
                state.Precision = precision;
                state.IsDirty = true;
            }

            if(isNull.HasValue && isNull.Value != this.IsNull) {
                state.IsNull = isNull.Value;
                state.IsDirty = true;
            }

            if(defaultValue != null && defaultValue != this.DefaultValue)
            {
                state.DefaultValue = defaultValue;
                state.IsDirty = true;
            }

            if(isUnique != null && isUnique != this.IsUnique)
            {
                state.Unique = new UniqueConstraint(uniqueName, this);
                state.IsDirty = true;
            }
        }
       

        public void AddReference(string schemaName, string tableName, string columnName, 
            ReferenceAction update = ReferenceAction.NoAction,
            ReferenceAction delete = ReferenceAction.NoAction) {

            var t = this.Table.Database.Tables.SingleOrDefault(o => o.Schema.Name == schemaName && o.Name == tableName);
            if(t == null) 
                throw new Exception($"table {t.Name} does not exist");
            

            var c = t.Columns.SingleOrDefault(o => o.Name == columnName);
            if(c == null) 
                throw new Exception($"table {c.Name} does not exist");

            this.AddReference(c, update, delete);
        }

        public void AddReference(string tableName, string columnName, 
            ReferenceAction update = ReferenceAction.NoAction,
            ReferenceAction delete = ReferenceAction.NoAction) {

            this.AddReference(null, tableName, columnName, update, delete);
        }

        public void AddReference(DbColumn column, 
            ReferenceAction update = ReferenceAction.NoAction,
            ReferenceAction delete = ReferenceAction.NoAction)
        {
            this.Reference = new ReferenceConstraint() {
                Owner = this,
                ForeignColumn = column,
                Update = update,
                Delete = delete
            };
        }
    }

}