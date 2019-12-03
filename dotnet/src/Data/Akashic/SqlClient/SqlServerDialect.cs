

using System;
using System.Collections.Generic;

namespace NerdyMishka.Data.SqlClient
{
     public class SqlServerDialect : SqlDialect
    {
        private static List<RdbmsSqlTypeMapping> mappings;
        

        static SqlServerDialect()
        {
            mappings = new List<RdbmsSqlTypeMapping>();

            mappings.Add(new RdbmsSqlTypeMapping(
                 "NVARCHAR",
                 typeof(String),
                 AkashicDbType.String,
                 255,
                 long.MaxValue,
                 "MAX"
            ));

            mappings.Add(new RdbmsSqlTypeMapping()
            {
                ClrType = typeof(char[]),
                DbType = AkashicDbType.StringFixedLength,
                SqlType = "NCHAR",
                DefaultLimit = 255,
                MaxLimit = long.MaxValue,
            });

            mappings.Add(new RdbmsSqlTypeMapping()
            {
                ClrType = typeof(decimal),
                DbType = AkashicDbType.Decimal,
                SqlType = "DECIMAL",
            });


            mappings.Add(new RdbmsSqlTypeMapping()
            {
                ClrType = typeof(int),
                DbType = AkashicDbType.Int32,
                SqlType = "INT",
            });

            mappings.Add(new RdbmsSqlTypeMapping()
            {
                ClrType = typeof(short),
                DbType = AkashicDbType.Int16,
                SqlType = "SMALLINT",
            });


            mappings.Add(new RdbmsSqlTypeMapping()
            {
                ClrType = typeof(byte),
                DbType = AkashicDbType.Byte,
                SqlType = "TINYINT",
            });

             mappings.Add(new RdbmsSqlTypeMapping()
            {
                ClrType = typeof(byte),
                DbType = AkashicDbType.Binary,
                SqlType = "BINARY",
                DefaultLimit = 256,
                MaxLimit = long.MinValue,
                MaxValue = "MAX"
            });

            mappings.Add(new RdbmsSqlTypeMapping()
            {
                ClrType = typeof(bool),
                DbType = AkashicDbType.Boolean,
                SqlType = "BIT",
            });

            mappings.Add(new RdbmsSqlTypeMapping()
            {
                ClrType = typeof(long),
                DbType = AkashicDbType.Int64,
                SqlType = "BIGINTEGER"
            });

            mappings.Add(new RdbmsSqlTypeMapping()
            {
                ClrType = typeof(Guid),
                DbType = AkashicDbType.Guid,
                SqlType = "UNIQUEIDENTIFIER",
            });

            mappings.Add(new RdbmsSqlTypeMapping()
            {
                ClrType = typeof(DateTime),
                DbType = AkashicDbType.DateTime,
                SqlType = "DATETIME",
            });

            mappings.Add(new RdbmsSqlTypeMapping()
            {
                ClrType = typeof(RdbmsTimestamp),
                DbType = AkashicDbType.TimeStamp,
                SqlType = "ROWVERSION",
            });

        }

        public override string Name => "SqlServer";

        public override string ParameterPrefix => "@";

        public override string LeftQuote => "[";

        public override string RightQuote => "]";

        public override string NewLine => "\n";

        public override string DateTimeFormat => "MM/dd/yyyy HH:mm:ss";

        public override string DateFormat => "MM/dd/yyyy";

        public override string TimeFormat => "HH:mm:ss";

       

        protected override IList<RdbmsSqlTypeMapping> GetMappings()
        {
            return mappings;
        }
        public override string FormatBoolean(bool value)
        {
            if (value == true)
                return "1";

            return "0";
        }
    }
}