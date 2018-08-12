using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Data 
{
    public class SqliteDialect : SqlDialect
    {
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
            throw new NotImplementedException();
        }
    }
}
