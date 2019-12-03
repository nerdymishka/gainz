using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Data
{
    public class RdbmsSqlTypeMapping
    {

        public RdbmsSqlTypeMapping(
         )
        {

        }

        public RdbmsSqlTypeMapping(
            string sqlType,
            Type clrType,
            AkashicDbType dbType,
            long? defaultLimit = null,
            long? maxLimit = null,
            string maxValue = null)
        {

            if (maxLimit.HasValue && maxValue == null)
                maxValue = maxLimit.Value.ToString();


        }

        public AkashicDbType DbType { get; set; }

        public string SqlType { get; set; }

        public Type ClrType { get; set; }

        public long? MaxLimit { get; set; }

        public long? DefaultLimit { get; set; }

        public string MaxValue { get; set; }  
    } 
}
