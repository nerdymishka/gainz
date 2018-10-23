using System.Collections.ObjectModel;
using System.Collections.Concurrent;

namespace NerdyMishka.Data 
{
    public class DbRole : DbObject
    {
        private ConcurrentBag<DbUser> users = new ConcurrentBag<DbUser>();
        
        private Database database;

        
    }

}