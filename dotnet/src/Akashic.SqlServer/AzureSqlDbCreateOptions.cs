using System;
using NerdyMishka.Data;

namespace NerdyMishka.Data 
{

    public class AzureSqlDbCreateOptions
    {

        public string ElasticPoolName { get; set; }

        public string ServiceTier { get; set; }

        public string SourceDatabase { get; set; }

        public string MaxSize { get; set; }

        public string Edition  { get; set; }
    }
}