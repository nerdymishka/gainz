using System;
using System.Collections.Generic;
using System.Linq;

namespace NerdyMishka.Data
{

    public abstract class SqlDialect 
    {

        public abstract string Name { get; }

        public abstract string ParameterPrefix { get;  }

        public virtual string LeftQuote => "\"";

        public virtual string RightQuote => "\"";

        public virtual string NewLine => "/n";

        public abstract string DateTimeFormat { get; }

        public abstract string DateFormat { get;  }

        public abstract string TimeFormat { get; }

        public virtual bool SupportsSchema => true;

    

        public abstract string FormatBoolean(Boolean value);

        public virtual string FormatDate(DateTime dateTime, SqlDateTimeFormat format)
        {
            string pattern = null;
            switch(format)
            {
                case SqlDateTimeFormat.Date:
                     pattern = this.DateFormat;
                     break;
                case SqlDateTimeFormat.Time:
                     pattern = this.TimeFormat;
                     break;
                 case SqlDateTimeFormat.DateTime:
                default:
                    pattern = this.DateTimeFormat;
                    break;
            }

            return this.LeftQuote + dateTime.ToString(pattern) + this.RightQuote;
        }

        public virtual string FormatDateAsInt(DateTime dateTime)
        {
            return dateTime.ToBinary().ToString();
        }

        public RdbmsSqlTypeMapping FindByClrType(Type clrType, long? limit = null)
        {
            if (clrType.IsGenericType && clrType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                clrType = clrType.GetGenericArguments()[0];
            }

            var set = GetMappings().Where(o => o.ClrType == clrType);
            if (limit.HasValue)
            {
                set = set.Where(o =>
                    o.MaxLimit.HasValue &&
                    limit <= o.MaxLimit && limit >= o.DefaultLimit);
            }

            var map = set.FirstOrDefault();
            return map;
        }

        public  string ConvertToSqlType(Type clrType, long? limit = null)
        {
            var map = FindByClrType(clrType, limit);
            if (map == null)
                return null;

            return map.SqlType;
        }

        public  string ConvertToSqlType(AkashicDbType dbType, long? limit = null)
        {
            var set = GetMappings().Where(o => o.DbType == dbType);

            if (limit.HasValue)
            {
                set = set.Where(o =>
                    o.MaxLimit.HasValue &&
                    limit <= o.MaxLimit && limit >= o.DefaultLimit);
            }

            var map = set.FirstOrDefault();
            if (map == null)
                return null;

            return map.SqlType;
        }

        public Type ConvertToType(string sqlType, long? limit = null)
        {
            var set = GetMappings().Where(o => o.SqlType == sqlType.ToUpper());
            var type = set.FirstOrDefault();
            if (type == null)
                return null;

            return type.ClrType;
        }

        protected abstract IList<RdbmsSqlTypeMapping> GetMappings();
    }
}