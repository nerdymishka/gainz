using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NerdyMishka.Data
{
    public enum SqlDateTimeFormat
    {
        DateTime =0,
        Date = 1,
        Time = 2
    }

    public class SqlBuilder
    {
        private StringBuilder builder = new StringBuilder();
        private SqlDialect dialect;
        private const string indent = "    ";


        public SqlBuilder()
            :this(new SqlServerDialect())
        {

        }

        public SqlBuilder(SqlDialect dialect)
        {
            if (dialect == null)
                throw new ArgumentNullException(nameof(dialect));

            this.dialect = dialect;
        }

        internal SqlBuilder(SqlDialect info, char[] data)
        {
            this.dialect = info;
            this.builder = new StringBuilder();
            this.builder.Append(data);
        }

        public SqlBuilder Clone(bool deep = false)
        {
            if(deep)
            {
                var array = new Char[this.builder.Length];
                this.builder.CopyTo(0, array, 0, this.builder.Length);

                return new SqlBuilder(this.dialect, array);
            }

            return new SqlBuilder(this.dialect);
        }


        public SqlBuilder AppendLine(string value)
        {
            this.builder.Append(value)
                .Append(this.dialect.NewLine);

            return this;
        }

        public SqlBuilder Indent(int tabs)
        {
            var tab = 0;
            while (tab < tabs)
            {
                this.builder.Append(indent);
                tab++;
            }

            return this;
        }

        public SqlBuilder Append(SqlBuilder builder)
        {
            if (builder == null)
                return this;

            return this.Append(builder.builder);
        }

        public SqlBuilder Append(StringBuilder builder)
        {
            if (builder == null)
                return this;

            char[] array = new char[builder.Length];
            builder.CopyTo(0, array, 0, builder.Length);
            this.builder.Append(array);

            return this;
        }

        public SqlBuilder Append(DBNull value)
        {
            return this.AppendNull();
        }

        public SqlBuilder Append(bool value)
        {
            this.builder.Append(this.dialect.FormatBoolean(value));
            return this;
        }

        public SqlBuilder Append(bool? value)
        {
            if (!value.HasValue)
                return this.AppendNull();
            
            return this.Append(value.Value);
        }

        public SqlBuilder Append(char value)
        {
            this.builder.Append(value);
            return this;
        }

        public SqlBuilder Append(char? value)
        {
            if (!value.HasValue)
                return this;

            return this.Append(value);
        }
        
        public SqlBuilder AppendNull()
        {
            this.Append("NULL");
            return this;
        }

        public SqlBuilder Append(DateTime? value, SqlDateTimeFormat format = SqlDateTimeFormat.DateTime)
        {
            if (!value.HasValue)
                return this.AppendNull();

            return this.Append(value.Value, format);
        }

        public SqlBuilder AppendAsInt(DateTime? value)
        {
            if (!value.HasValue)
                return this.AppendNull();

            return this.AppendAsInt(value);
        }

        public SqlBuilder Append(DateTime value, SqlDateTimeFormat format = SqlDateTimeFormat.DateTime)
        {
            return this.Append(this.dialect.FormatDate(value, format));
        }

        public SqlBuilder AppendAsInt(DateTime value)
        {
            return this.Append(this.dialect.FormatDateAsInt(value));
        }

        public SqlBuilder Append(char[] value)
        {
            if (value == null)
                return this.AppendNull();

            this.builder.Append('\'')
                .Append(value)
                .Append('\'');

            return this;
        }

        public SqlBuilder Append(byte[] value)
        {
            if (value == null)
                return this.AppendNull();

            string literal = "0x" + String.Join("", value.Select(n => n.ToString("X2")));
            this.builder.Append(literal);

            return this;
        }


        public SqlBuilder Append(double? value)
        {
            if (!value.HasValue)
                return this.AppendNull();


            return this.Append(value.Value);
        }


        public SqlBuilder Append(double value)
        {
            this.builder.Append(value);
            return this;
        }

        public SqlBuilder Append(decimal? value)
        {
            if (!value.HasValue)
                return this.AppendNull();


            return this.Append(value.Value);
        }


        public SqlBuilder Append(decimal value)
        {
            this.builder.Append(value);
            return this;
        }

        public SqlBuilder Append(short value)
        {
            this.builder.Append(value);
            return this;
        }

        public SqlBuilder Append(short? value)
        {
            if (!value.HasValue)
                return this.AppendNull();

            return this.Append(value.Value);
        }

        public SqlBuilder Append(int value)
        {
            this.builder.Append(value);
            return this;
        }

        public SqlBuilder Append(int? value)
        {
            if (!value.HasValue)
                return this.AppendNull();

            return this.Append(value.Value);
        }


        public SqlBuilder Append(long value)
        {
            this.builder.Append(value);
            return this;
        }

        public SqlBuilder Append(long? value)
        {
            if (!value.HasValue)
                return this.AppendNull();

            return this.Append(value.Value);
        }

        public SqlBuilder Append(string value)
        {
            if (value == null || value == string.Empty)
                return this;

            this.builder.Append(value);
            return this;
        }


        public SqlBuilder Quote(char[] value)
        {
            if (value == null)
                return this.AppendNull();

            this.builder.
                Append('\'')
                .Append(value)
                .Append('\'');

            return this;
        }

        public SqlBuilder Quote(char value)
        {
            this.builder
                .Append('\'')
                .Append(value)
                .Append('\'');

            return this;
        }

        public SqlBuilder Quote(string value)
        {
            this.builder.
                Append('\'')
                .Append(value)
                .Append('\'');

            return this;
        }

        public SqlBuilder AppendParameter(string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter))
                return this;

            this.builder.Append(this.dialect.ParameterPrefix)
                .Append(parameter);

            return this;
        }

        

        public SqlBuilder AppendIdentifiers(params string[] identifiers)
        {
            if (identifiers == null)
                return this;

            var last = identifiers.Length - 1;
            for (var i = 0; i < identifiers.Length; i++)
            {
                var identifier = identifiers[i];
                if (string.IsNullOrWhiteSpace(identifier))
                    continue;

                this.builder.Append(this.dialect.LeftQuote);
                this.builder.Append(identifier);
                this.builder.Append(this.dialect.RightQuote);

                if (i < last)
                    this.builder.Append(".");
            }

            return this;
        }

        public SqlBuilder AppendIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                return this;

            this.builder.Append(this.dialect.LeftQuote);
            this.builder.Append(identifier);
            this.builder.Append(this.dialect.RightQuote);

            return this;
        }

        public void Clear()
        {
            this.builder.Clear();
        }

        public override string ToString()
        {
            return this.ToString(false);
        }

        public string ToString(bool clear)
        {
            var data = this.builder.ToString();
            if (clear)
                this.Clear();
            return data;
        }
    }
}
