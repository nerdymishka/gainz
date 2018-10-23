using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NerdyMishka.Data
{
    public class PrimaryKeyConstraint : DbConstraint
    {
        private ConcurrentBag<DbColumn> columns = new ConcurrentBag<DbColumn>();

        public PrimaryKeyConstraint()
        {
            
        }

        public PrimaryKeyConstraint(string name, params DbColumn[] columns) :this()
        {
            if(columns != null && columns.Length > 0 ) {
                foreach(var c in columns)
                    this.Add(c);
            }
        }

        public void Add(DbColumn column)
        {
            this.columns.Add(column);
            column.Table.Add(this);
        }

        public void Remove(DbColumn column)
        {
            this.columns.TryTake(out column);
           
        }

        public IEnumerable<DbColumn> Columns => this.columns;

        public IEnumerable<string> ColumnNames { get => this.columns.Select(o => o.Name); }
    }
}