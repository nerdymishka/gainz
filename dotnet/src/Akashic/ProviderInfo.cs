using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.Data
{
    public class ProviderInfo 
    {
        private string name;

        /// <summary>
        /// The string format for the Assembly Name.
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// The name of the assembly.
        /// </summary>
        public string Name
        {
            get
            {
                if (this.name != null)
                    return this.name;
                
                return string.Format(this.Pattern, this.Version);
            }
            set { this.name = value; }
        }

        /// <summary>
        /// The version of the assembly.
        /// </summary>
        public string Version { get; set; }

        public string ClassName { get; set; }

        public string FieldName { get; set; }

        public DbProviderFactory Instance { get; set; }

        public SqlDialect Dialect { get; set; }
    }
}
