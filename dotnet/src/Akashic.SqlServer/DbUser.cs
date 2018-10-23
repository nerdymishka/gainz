using System.Collections.ObjectModel;
using System.Collections.Concurrent;

namespace NerdyMishka.Data 
{

    public class DbUser : DbObject
    {
        private ConcurrentBag<DbRole> users = new ConcurrentBag<DbRole>();
        
        private Database database;

        
    }

}