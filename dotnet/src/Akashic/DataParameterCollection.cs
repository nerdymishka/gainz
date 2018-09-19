using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.Data
{
    public class DataParameterCollection : ICollection<DbParameter>, ICollection
    {
        private DbParameterCollection collection;
        private IDataCommand command;

        public DataParameterCollection(DbParameterCollection collection, IDataCommand command)
        {
            this.command = command;
            this.collection = collection;
        }

        public int Count => this.collection.Count;

        public bool IsReadOnly => false;

        public object SyncRoot => ((ICollection)this.collection).SyncRoot;

        public bool IsSynchronized => ((ICollection)this.collection).IsSynchronized;

        public void AddRange(IEnumerable<DbParameter> set) => this.collection.AddRange(set.ToArray());


        public DbParameter this[string parameterName]
        {
            get { return this.collection[parameterName]; }
            set { this.collection[parameterName] = value; }
        }

        public DbParameter this[int i]
        {
            get { return this.collection[i]; }
            set { this.collection[i] = value; }
        }

        public DbParameter Add(object value)
        {
            DbParameter p = null;
            if (value is DbParameter)
            {
                p = (DbParameter)value;
                this.collection.Add(value);
                return p;
            }

            p = this.command.CreateParameter();
            p.ParameterName = AkashicSettings.ParameterPrefix + this.collection.Count.ToString();
            p.Value = value;

            return p;
        }

        public T Add<T>(string name, object value) where T: DbParameter
        {
            return (T)this.Add(name, value);
        }

        public DbParameter Add(string name, object value)
        {
            var p = this.command.CreateParameter();
            p.ParameterName = name;
            p.Value = value;

            this.collection.Add(p);

            return p;
        }

        public DbParameter Add(string name, DbType type, int precision, int scale)
        {
            var p = this.command.CreateParameter();
            p.ParameterName = name;
            p.DbType = type;

#if !NET45
            p.Precision = (byte)precision;
            p.Scale = (byte)scale;
#endif 
            this.collection.Add(p);
            return p;
        }


        public DbParameter Add(string name, DbType type, int? size = null)
        {
            var p = this.command.CreateParameter();
            p.ParameterName = name;
            p.DbType = type;

            if (size.HasValue)
                p.Size = size.Value;

            this.collection.Add(p);
            return p;
        }

        public void Add(DbParameter item) => this.collection.Add(item);

        public void Clear() => this.collection.Clear();

        public bool Contains(DbParameter item) => this.collection.Contains(item);

        public void CopyTo(DbParameter[] array, int arrayIndex) => this.collection.CopyTo(array, arrayIndex);


        public IEnumerator<DbParameter> GetEnumerator()
        {
            foreach (DbParameter parameter in this.collection)
                yield return parameter;
        }

        public bool Remove(DbParameter item)
        {
            var l = this.collection.Count;
            this.collection.Remove(item);
            return l != this.collection.Count;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (DbParameter parameter in this.collection)
                yield return parameter;
        }

        public void CopyTo(Array array, int index) => this.collection.CopyTo(array, index);
    }
}
