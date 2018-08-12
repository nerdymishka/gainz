using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NerdyMishka.Data
{
    /// <summary>
    /// DataConnection provides an abstraction for DbConnection with an
    /// provides enhanced extension points by implementing <see cref="ISqlExecutor" />
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This library does not reference any SQL implementations by design. The
    ///         calling library or application must reference the SQL implementation
    ///         before creating a new DataConnection.
    ///     </para>
    ///     <para>
    ///         Libraries that consume Akashic should create a wrapper method that
    ///         initializes a new DataConnection.
    ///     </para>
    /// </remarks>
    public class DataConnection : IDataConnection
    {
        private DbConnection connection;
        private bool autoClose = false;
        private SqlDialect dialect;
        private string providerName;

        private bool externallyControlled = false;

        /// <summary>
        /// Initializes a new instance of the DataConnection.
        /// </summary>
        /// <param name="providerName">
        ///     The name of the provider. <see cref="KnownProviders" /> provides
        ///     common defaults such as SqlServer or Sqlite.
        /// </param>
        /// <param name="connectionString">Optional. The connection string for the connection</param>
        public DataConnection(string providerName, string connectionString = null)
        {
            this.providerName = providerName;
            var info = AkashicProviderFactory.GetProviderInfo(providerName);
            var factory = AkashicProviderFactory.GetDbFactory(info);
            this.connection = factory.CreateConnection();
            this.connection.ConnectionString = connectionString;
            this.dialect = info.Dialect;
        }

        /// <summary>
        /// Initializes a new instance of the DataConnection.
        /// </summary>
        /// <param name="connection">An existing <see cref="System.Data.DBConnection" /></param>
        /// <param name="dialect">The dialect of SQL that must be used.</param>
        public DataConnection(DbConnection connection, SqlDialect dialect = null)
        {
            this.externallyControlled = true;
            this.dialect = dialect ?? new SqlServerDialect();
            this.connection = connection;
        }

        internal DbConnection InnerConnection => this.connection;

        /// <summary>
        /// Gets or sets the connection string for the connection.
        /// </summary>
        /// <value>string</value>
        public string ConnectionString
        {
            get { return this.connection.ConnectionString; }
            set { this.connection.ConnectionString = value; }
        }

        /// <summary>
        /// The name of the provider for this connection.
        /// </summary>
        public string ProviderName => this.providerName;

        /// <summary>
        /// Gets the state of the connection.
        /// </summary>
        /// <value></value>
        public virtual DataConnectionState State
        {
            get
            {
                switch (this.connection.State)
                {
                    case System.Data.ConnectionState.Closed:
                    case System.Data.ConnectionState.Broken:
                        return DataConnectionState.Closed;

                    case System.Data.ConnectionState.Connecting:
                        return DataConnectionState.Connecting;

                    default:
                        return DataConnectionState.Open;
                }
            }
        }

        /// <summary>
        /// Changes the target database for this connection.
        /// </summary>
        /// <param name="database">The name of the new target database.</param>
        public virtual void ChangeDatabase(string database)
        {
            try
            {
                this.InnerConnection.ChangeDatabase(database);
            } catch
            {

                this.InnerConnection.Close();
                var cs = this.ConnectionString;
                var info = AkashicProviderFactory.GetProviderInfo(providerName);
                var dbFactory = AkashicProviderFactory.GetDbFactory(info);
                var builder = dbFactory.CreateConnectionStringBuilder();

                var keys = new[] { "Database", "Initial Catalog", "DataSource" };
                builder.TryGetValue("Database", out object oldDb);

                if(oldDb != null)
                {
                    builder["Database"] = database;
                }
            }
        }

        /// <summary>
        /// The dialect of SQL for the connection. <see cref="SqlDialect" />
        /// is used by SqlBuilder to generate SQL strings that adhere to a
        /// a dialect's language implementation.
        /// </summary>
        public SqlDialect SqlDialect => this.dialect;

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Close() => this.connection.Close();

        /// <summary>
        /// Creates a new instance of <see cref="IDataCommand" />
        /// that has a default of <see cref="System.Data.CommandType.Text" />
        /// </summary>
        /// <returns>A new instance of <see cref="DataCommand" ></returns>
        public IDataCommand CreateCommand()
        {
            var cmd = this.connection.CreateCommand();
            cmd.CommandType = CommandType.Text;

             var dc = new DataCommand(cmd);
            dc.Connection = this;
            return dc;
        }

        /// <summary>
        /// Creates a new instance of <see cref="IDataCommand" />
        /// that has a default of <see cref="System.Data.CommandType.Text" />
        /// with the specified command behavior.
        /// </summary>
        /// <param name="commandBehavior">The behavior the command must use.</param>
        /// <returns>A new instance of <see cref="DataCommand" ></returns>
        public IDataCommand CreateCommand(CommandBehavior commandBehavior)  
        {
            var cmd = this.connection.CreateCommand();
            cmd.CommandType = CommandType.Text;

            var dc = new DataCommand(cmd, commandBehavior);
            dc.Connection = this;
            return dc;
        }

        /// <summary>
        /// Begins a transaction.
        /// </summary>
        /// <param name="level">
        ///     Optional. The level of isolation for the transaction. 
        ///     The default is Unspecified. 
        /// </param>
        /// <returns>A instance of <see cref="DataTransaction" /></returns>
        public virtual IDataTransaction BeginTransaction(IsolationLevel level = IsolationLevel.Unspecified)
        {
            return new DataTransaction(this, this.SqlDialect, level, false);
        }

        /// <summary>
        /// Prepares the next Sql statement for execution. <see cref="DataConnection.Open" /> is called if
        /// the connection is not already open.
        /// </summary>
        /// <param name="data">The sql statement context.</param>
        public void OnNext(SqlStatementContext data)
        {
            if(data == null)
                throw new ArgumentNullException(nameof(data));
            
            
            if (this.State != DataConnectionState.Open)
            {
                this.autoClose = data.IsCompleteable = true;
                this.Open();
            }

            if(this.State != DataConnectionState.Open)
            {
                throw new Exception("I am closed =(");
            }

            if (data.Command == null)
            {
              
                data.Command = this.CreateCommand();
                
            }

            if(data.Command.Connection == null)
                data.Command.Connection = this;

            switch (data.Type)
            {
                case SqlStatementContext.ParameterSetType.Array:
                    data.Command.Configure(data.Query, data.ParameterArray);
                    break;
                case SqlStatementContext.ParameterSetType.DbParameters:
                    data.Command.Configure(data.Query, data.DbParameters);
                    break;
                case SqlStatementContext.ParameterSetType.Hashtable:
                    data.Command.Configure(data.Query, data.Hastable);
                    break;
                case SqlStatementContext.ParameterSetType.KeyValue:
                    data.Command.Configure(data.Query, data.Parameters);
                    break;
                default:
                    data.Command.Configure(data.Query);
                    break;
            }
        }

        /// <summary>
        /// Closes the connection on error.
        /// </summary>
        /// <param name="ex"></param>
        public void OnError(Exception ex)
        {
            // TODO: log exception
            this.Close();
        }

        /// <summary>
        /// Closes the connection if OnNext opened the connection.
        /// </summary>
        public void OnCompleted()
        {
            if (this.autoClose)
            {
                this.autoClose = false;
                if (this.State != DataConnectionState.Closed)
                    this.Close();
            }
        }

        
        /// <summary>
        /// Disposes of resources. If the inner connection was created
        /// by this DataConnection instance, that connection will be disposed.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(true);
        }

        protected void Dispose(bool dispose)
        {
            if(dispose)
            {
                if(!this.externallyControlled)
                {
                    this.connection.Dispose();
                    this.connection = null;
                }
            }
        }

        ~DataConnection()
        {
            Dispose(false);
        }

        /// <summary>
        /// Opens the connection.
        /// </summary>
        public void Open() => this.connection.Open();

        /// <summary>
        /// Asynchronsly opens the connection.
        /// </summary>
        /// <returns>Returns a <see cref="System.Threading.Tasks.Task"> object </returns>
        public Task OpenAsync() => this.connection.OpenAsync();

        /// <summary>
        /// Asynchronsly opens the connection.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns a <see cref="System.Threading.Tasks.Task"> object </returns>
        public Task OpenAsync(CancellationToken cancellationToken)
            => this.connection.OpenAsync(cancellationToken);

        /// <summary>
        /// Creates a new instance of <see cref="SqlBuilder" />
        /// </summary>
        /// <returns>Returns a new instance of <see cref="SqlBuilder" /></returns>
        public SqlBuilder CreateSqlBuilder()
        {
            return new SqlBuilder(this.SqlDialect);
        }
    }
}
