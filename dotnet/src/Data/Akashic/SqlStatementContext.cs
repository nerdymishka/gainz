using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace NerdyMishka.Data
{
    public class SqlStatementContext
    {
        public SqlStatementContext(string query)
        {
            this.Query = query;
            this.Type = ParameterSetType.None;
        }


        public SqlStatementContext(string query, DbParameter[] parameters)
        {
            this.Query = query;
            this.Type = ParameterSetType.DbParameters;
            this.DbParameters = parameters;
        }

        public SqlStatementContext(string query, IEnumerable<DbParameter> parameters)
        {
            this.Query = query;
            this.Type = ParameterSetType.DbParameters;
            this.DbParameters = parameters;
        }

        public SqlStatementContext(string query, IList<DbParameter> parameters)
        {
            this.Query = query;
            this.Type = ParameterSetType.DbParameters;
            this.DbParameters = parameters;
        }

        public SqlStatementContext(string query, IList parameters)
        {
            this.Query = query;
            this.Type = ParameterSetType.Array;
            this.ParameterArray = parameters;
        }

        public SqlStatementContext(string query, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            this.Query = query;
            this.Type = ParameterSetType.KeyValue;
            this.Parameters = parameters;
        }

        public SqlStatementContext(string query, IDictionary hashtable)
        {
            this.Query = query;
            this.Type = ParameterSetType.Hashtable;
            this.Hastable = hashtable;
        }



        public enum ParameterSetType
        {
            None = 0,
            Array = 1,
            KeyValue = 2,
            Hashtable = 3,
            DbParameters = 4
        }

        public ParameterSetType Type { get; set; }

        public IEnumerable<DbParameter> DbParameters { get; set; }

        public IEnumerable<KeyValuePair<string, object>> Parameters { get; set; }

        public IDictionary Hastable { get; set; }
       
        public IList ParameterArray { get; set; }

        public IDataCommand Command { get; set; }

        public string Query { get; set; }

        public bool IsCompleteable { get; set; }
    }
}
