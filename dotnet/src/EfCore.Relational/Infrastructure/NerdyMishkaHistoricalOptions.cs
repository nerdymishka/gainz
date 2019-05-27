using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace NerdyMishka.EfCore.Infrastructure
{
    public class NerdyMishkaHistoricalOptions
    {
        public string Prefix { get; set; }

        public string SchemaName { get; set; } 

        public string TableName { get; set; } = "migration_history";

        public Func<string, string> FormatColumn { get; set; }

        internal protected string FormattedTableName 
        {
            get
            {
                if(string.IsNullOrWhiteSpace(this.Prefix))
                    return this.TableName;

                return $"{this.Prefix}{this.TableName}";
            }
        }

        public static NerdyMishkaHistoricalOptions Extract(IDbContextOptions options)
        {
            if(options == null)
                throw new ArgumentNullException(nameof(options));

            var historicalOptions
                = options.Extensions
                    .OfType<NerdyMishkaHistoricalOptions>()
                    .ToList();

            if (historicalOptions.Count == 0)
            {
                throw new InvalidOperationException("No provider configured");
            }

            if (historicalOptions.Count > 1)
            {
                throw new InvalidOperationException("Multiple providers configured");
            }

            return historicalOptions[0];
        }
    }
}