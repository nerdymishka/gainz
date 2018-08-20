using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.KeePass
{
    public interface IKeyPassEntryFields
    {
        string Title { get; set; }


        string Password { get; set; }

        string Url { get; set; }

        string UserName { get; set; }

        string Notes { get; set; }

        IList<string> Tags { get; set; }

        string UnprotectPassword();

      
    }
}
