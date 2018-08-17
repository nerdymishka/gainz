using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace NerdyMishka.Data 
{
    public class QueryBuilder
    {
        private SqlBuilder sql;

        private IDataConnection connection;

        private Dictionary<string, object> parameters;
        private List<object> args;

        public static Func<DataConnection> ConnectionFactory { get; set;}


        public QueryBuilder()
        {

        }

        public QueryBuilder(DataConnection connection) {
            this.UseConnection(connection);
        }

        public QueryBuilder UseConnection(IDataConnection connection)
        {
            this.sql = connection.CreateSqlBuilder();
            this.connection = connection;
            return this;
        }

        protected Dictionary<string, object> GetParameters()
        {
            if(parameters !=null)
                return parameters;

            if(this.args != null)
                throw new InvalidOperationException("Only AddParameters or AddArguments can be used in a given context");

            return parameters = new Dictionary<string, object>();
        }

        protected List<object> GetArguments()
        {
            if(this.args != null)
                return this.args;

            if(this.parameters != null)
                throw new InvalidOperationException("Only AddParameters or AddArguments can be used in a given context");

            return this.args = new List<object>();
        }

        protected void EnsureConnection()
        {
            if(this.connection == null)
                this.connection = ConnectionFactory();
            
            if(this.sql == null)
                this.sql = connection.CreateSqlBuilder();
        }

        public QueryBuilder Append(string value)
        {
            this.EnsureConnection();
            this.sql.Append(value);

            return this;
        }

        public QueryBuilder Append(SqlBuilder value)
        {
            this.EnsureConnection();
            this.sql.Append(value);

            return this;
        }

         public void Add(string parameterName, object parameterValue)
        {
            var p = this.GetParameters();
            p[parameterName] = parameterValue;
        }

        public QueryBuilder AddParameter(string parameterName, object parameterValue)
        {
            var p = this.GetParameters();
            p[parameterName] = parameterValue;

            return this;
        }

        public QueryBuilder AddArgument(object value)
        {
            var a = this.GetArguments();
            a.Add(value);

            return this;
        }

        private void Clear()
        {
             this.sql = null;
                this.parameters = null;
                this.args = null;
        }

        public int Excecute() 
        {
            this.EnsureConnection();
            int result;

            if(this.parameters != null){
                result = this.connection.Execute(this.sql, this.parameters);
            } else if(this.args != null) {
                result = this.connection.Execute(this.sql, this.args);
            } else {
                result = this.connection.Execute(this.sql);
            }
                
            this.Clear();
            return result;
        }

        public IDataReader FetchReader()
        {
            this.EnsureConnection();
            IDataReader dr;

            if(this.parameters != null) {
                dr = this.connection.FetchReader(this.sql);
            } else if(this.args != null) {
                dr = this.connection.FetchReader(this.sql.ToString(true), this.args);
            } else {
                dr = this.connection.FetchReader(this.sql);
            }

            this.Clear();

            return dr;
        }

         public async Task<IDataReader> FetchReaderAsync(CancellationToken token = default(CancellationToken))
        {
            this.EnsureConnection();
            IDataReader dr;

            if(this.parameters != null) {
                dr = await this.connection.FetchReaderAsync(this.sql.ToString(true), this.parameters, token);
            } else if(this.args != null) {
                dr = await this.connection.FetchReaderAsync(this.sql.ToString(true), this.args, token);
            } else {
                dr = await this.connection.FetchReaderAsync(this.sql, token);
            }

            this.Clear();

            return dr;
        }

        public async Task<int>  ExcecuteAsync(CancellationToken token = default(CancellationToken)) 
        {
            this.EnsureConnection();
            int result;

            if(this.parameters != null){
                result = await this.connection.ExecuteAsync(this.sql, this.parameters, token);
            } else if(this.args != null) {
                result = await this.connection.ExecuteAsync(this.sql, this.args, token);
            } else {
                result = await this.connection.ExecuteAsync(this.sql, token);
            }
                
            this.Clear();
            return result;
        }
    }
}