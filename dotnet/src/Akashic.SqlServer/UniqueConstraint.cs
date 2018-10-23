using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace NerdyMishka.Data
{
    public class UniqueConstraint : DbConstraint
    {
        private Collection<DbColumn> columns = new Collection<DbColumn>();
        public UniqueConstraint() {
           
        }

        public UniqueConstraint(string name, params DbColumn[] columns)
        : this()
        {
            if(columns != null && columns.Length > 0 ) {
                foreach(var c in columns)
                    this.columns.Add(c);
            }
        }

        public IEnumerable<DbColumn> Columns => this.columns;


        public void Add(DbColumn column)
        {
            this.columns.Add(column);
        }
    }

}
