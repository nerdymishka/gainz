using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NerdyMishka.Data 
{
    public class DbPermission : DbObject
    {
        private ConcurrentBag<string> permissions = new ConcurrentBag<string>();

        public DbObject Object { get; internal protected set; }

        public DbObject Assignment { get; internal protected set; }

        public IEnumerable<string> Permissions => this.permissions;
    }

}