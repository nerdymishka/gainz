using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NerdyMishka.Data 
{
    public class DbSchema : DbObject
    {
        private ConcurrentBag<DbTable> tables = new ConcurrentBag<DbTable>();
        public DbSchema(string name, Database database)
        {
            this.Name = name;
            this.Database = database;
        }

        public string DatabaseName { get; internal protected set; }

        public IEnumerable<DbTable> Tables => this.tables;

        public Database Database { get; set; }

        public void Add(DbTable table)
        {
            if(this.tables.Any(o => o.Name == table.Name))
                throw new System.Exception($"Table {table.Name} already exists in schema {this.Name}");

            this.Database.Add(table);
            table.Schema = this;
            tables.Add(table);
        }
    }
}