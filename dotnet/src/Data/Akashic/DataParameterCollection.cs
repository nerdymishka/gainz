using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.Data
{
    public class DataParameterCollection : ICollection<IDbDataParameter>, ICollection
    {
        private IDataParameterCollection collection;
        private IDataCommand command;

        public DataParameterCollection(System.Data.IDataParameterCollection collection, IDataCommand command)
        {
            this.command = command;
            this.collection = collection;
        }

        public int Count => this.collection.Count;

        public bool IsReadOnly => false;

        public object SyncRoot => ((ICollection)this.collection).SyncRoot;

        public bool IsSynchronized => ((ICollection)this.collection).IsSynchronized;

        public void AddRange(IEnumerable<IDbDataParameter> set) 
        {
            foreach(var item in set)
                this.Add(item);
        }


        public IDbDataParameter this[string parameterName]
        {
            get { return (IDbDataParameter) this.collection[parameterName]; }
            set { this.collection[parameterName] = value; }
        }

        public IDbDataParameter this[int i]
        {
            get { return (IDbDataParameter)this.collection[i]; }
            set { this.collection[i] = value; }
        }

        public IDbDataParameter Add(object value)
        {
            IDbDataParameter p = null;
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

        public IDbDataParameter Add(string name, object value)
        {
            var p = this.command.CreateParameter();
            p.ParameterName = name;
            p.Value = value;

            this.collection.Add(p);

            return p;
        }

        public IDbDataParameter Add(string name, DbType type, int precision, int scale)
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


        public IDbDataParameter Add(string name, DbType type, int? size = null)
        {
            var p = this.command.CreateParameter();
            p.ParameterName = name;
            p.DbType = type;

            if (size.HasValue)
                p.Size = size.Value;

            this.collection.Add(p);
            return p;
        }

        public void Add(IDbDataParameter item) => this.collection.Add(item);

        public void Clear() => this.collection.Clear();

        public bool Contains(IDbDataParameter item) => this.collection.Contains(item);

        public void CopyTo(IDbDataParameter[] array, int arrayIndex) => this.collection.CopyTo(array, arrayIndex);


        public IEnumerator<IDbDataParameter> GetEnumerator()
        {
            foreach (IDbDataParameter parameter in this.collection)
                yield return parameter;
        }

        public bool Remove(IDbDataParameter item)
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
