using System.Collections.ObjectModel;
using System.Collections.Concurrent;

namespace NerdyMishka.Data 
{
    public class DbTable : DbObject
    {
        private ConcurrentBag<DbConstraint> constraints = new ConcurrentBag<DbConstraint>();
        private ConcurrentBag<DbColumn> columns = new ConcurrentBag<DbColumn>();

        public DbSchema Schema { get; set; }

        public Database Database { get; set; }

        public Collection<DbColumn> Columns { get; set; }

        internal protected void Add(DbConstraint constraint)
        {
            this.constraints.Add(constraint);
        }

        internal protected void Add(DbColumn column)
        {
            this.columns.Add(column);
            column.Table = this;
        }

        

        internal protected void Remove(DbColumn column)
        {
            this.columns.TryTake(out column);
            column.Table = null;
        }

        internal protected void Remove(DbConstraint constraint)
        {
            this.constraints.TryTake(out constraint);
        }
    }
}