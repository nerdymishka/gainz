using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace NerdyMishka.Data
{
    public class SqlUserCreateOptions 
    {

        public SqlUserCreateOptions(params string[] databases)
        {
            if(databases != null && databases.Length > 0)
                this.Databases = databases.ToArray();
        }

        public IList<string> Databases { get; set; }

        public string LoginPassword { get; set; }

        public string UserPassword { get; set; }

        public bool EnsureSuffix { get; set; } = true;

        public string Login { get; set; }

        public bool EnsureLogin { get; set; }

        public string DefaultDatabase { get; set; }

        public bool DropUserIfExists { get; set; }  = true;   

        public bool DropLoginIfExists { get; set; } = false;       
    }

}