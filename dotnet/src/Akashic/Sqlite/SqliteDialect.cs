
using System;
using System.Collections.Generic;

namespace NerdyMishka.Data.Sqlite
{
     public class SqliteDialect : SqlDialect
    {
        private static List<RdbmsSqlTypeMapping> mappings;

        static SqliteDialect()
        {
            mappings = new List<RdbmsSqlTypeMapping>();

            mappings.Add(new RdbmsSqlTypeMapping(
                 "TEXT",
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
                SqlType = "TEXT",
                DefaultLimit = 255,
                MaxLimit = long.MaxValue,
            });

            mappings.Add(new RdbmsSqlTypeMapping()
            {
                ClrType = typeof(decimal),
                DbType = AkashicDbType.Decimal,
                SqlType = "FLOAT",
            });


            mappings.Add(new RdbmsSqlTypeMapping()
            {
                ClrType = typeof(int),
                DbType = AkashicDbType.Int32,
                SqlType = "INTEGER",
            });

            mappings.Add(new RdbmsSqlTypeMapping()
            {
                ClrType = typeof(short),
                DbType = AkashicDbType.Int16,
                SqlType = "INTEGER",
            });


            mappings.Add(new RdbmsSqlTypeMapping()
            {
                ClrType = typeof(byte),
                DbType = AkashicDbType.Byte,
                SqlType = "BLOB",
            });

            mappings.Add(new RdbmsSqlTypeMapping()
            {
                ClrType = typeof(byte[]),
                DbType = AkashicDbType.Binary,
                SqlType = "BLOB",
            });

            mappings.Add(new RdbmsSqlTypeMapping()
            {
                ClrType = typeof(bool),
                DbType = AkashicDbType.Boolean,
                SqlType = "INTEGER",
            });

            mappings.Add(new RdbmsSqlTypeMapping()
            {
                ClrType = typeof(long),
                DbType = AkashicDbType.Int64,
                SqlType = "INTEGER"
            });

            mappings.Add(new RdbmsSqlTypeMapping()
            {
                ClrType = typeof(Guid),
                DbType = AkashicDbType.Guid,
                SqlType = "TEXT",
            });

            mappings.Add(new RdbmsSqlTypeMapping()
            {
                ClrType = typeof(DateTime),
                DbType = AkashicDbType.DateTime,
                SqlType = "INTEGER",
            });

            mappings.Add(new RdbmsSqlTypeMapping()
            {
                ClrType = typeof(RdbmsTimestamp),
                DbType = AkashicDbType.TimeStamp,
                SqlType = "INTEGER",
            });
        }
        public override string Name => "Sqlite";

        public override string ParameterPrefix => "@";

        public override string LeftQuote => "\"";

        public override string RightQuote => "\"";

        public override string NewLine => "\n";

        public override string DateTimeFormat => "yyyy-MM-dd HH:mm:ss";

        public override string DateFormat => "yyyy-MM-dd";

        public override string TimeFormat => "HH:mm:ss";

        public override bool SupportsSchema => false;

        public override string FormatBoolean(bool value)
        {
            return value == true ? "1" : "0";
        }

        protected override IList<RdbmsSqlTypeMapping> GetMappings()
        {
           return mappings;
        }
    }
}