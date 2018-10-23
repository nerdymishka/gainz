using System;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NerdyMishka.Data 
{
    public class Database
    {
        private ConcurrentBag<DbTable> tables = new ConcurrentBag<DbTable>();
        private ConcurrentBag<DbSchema> schemas = new ConcurrentBag<DbSchema>();

        public string Name { get; set; }

        public IEnumerable<DbTable> Tables => this.tables;

        public IEnumerable<DbSchema> Schemas => this.schemas;

        public void Add(DbTable table)
        {
            this.tables.Add(table);
            table.Database = this;
        }
    }

}