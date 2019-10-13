using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NerdyMishka.Data
{
    public class DataCommand : IDataCommand
    {
        private IDbCommand command;

        private DbCommand command2;

        private DataParameterCollection parameters;
        private Func<IDataReader, Type, object> mapData;
        private IDataConnection connection;
        private CommandBehavior commandBehavior = CommandBehavior.Default;

        public DataCommand(string providerName)
        {
            var factory = AkashicProviderFactory.GetDbFactory(providerName);
            this.command = factory.CreateCommand();
            this.mapData = AkashicSettings.MapData;

            if(this.command is DbCommand)
                this.command2 = (DbCommand)this.command;
        }

        public DataCommand(IDbCommand command, IDataConnection connection)
        {
            this.mapData = AkashicSettings.MapData;
            this.command = command;
            this.connection = connection;

            if(this.command is DbCommand)
                this.command2 = (DbCommand)this.command;
        }

        public DataCommand(IDbCommand command, CommandBehavior commandBehavior)
        {
            this.mapData = AkashicSettings.MapData;
            this.command = command;
            if (this.command.Connection != null)
                this.connection = new DataConnection(command.Connection);

            this.commandBehavior = commandBehavior;

            if(this.command is DbCommand)
                this.command2 = (DbCommand)this.command;
        }

        public DataCommand(IDbCommand command)
        {
            this.mapData = AkashicSettings.MapData;
            this.command = command;
            if (this.command.Connection != null)
                this.connection = new DataConnection(command.Connection);

            if(this.command is DbCommand)
                this.command2 = (DbCommand)this.command;
        }

        public IDataConnection Connection
        {
            get
            {
                return this.connection;
            }
            set
            {
                this.connection = value;
                var inner = (DataConnection)this.connection;
                this.command.Connection = inner.InnerConnection;
            }
        }

        public IDbDataParameter AddParameter(object value)
        {
            return this.Parameters.Add(value);
        }

        public IDbDataParameter AddParameter(string name, object value)
        {
            return this.Parameters.Add(name, value);
        }

        public IDbDataParameter AddParameter(string name, DbType type, int? size)
        {
            return this.Parameters.Add(name, type, size);
        }

        public IDbDataParameter AddParameter(string name, DbType type, int precision, int scale)
        {
            return this.Parameters.Add(name, type, precision, scale);
        }

        public IDataCommand SetMap(Func<IDataReader, Type, object> map)
        {
            this.mapData = map;
            return this;
        }

       

        public IDbDataParameter CreateParameter() => this.command.CreateParameter();

        public DataParameterCollection Parameters
        {
            get
            {
                if (this.parameters == null)
                    this.parameters = new DataParameterCollection(this.command.Parameters, this);
                return this.parameters;
            }
        }

        public virtual string Text
        {
            get { return this.command.CommandText; }
            set { this.command.CommandText = value; }
        }

        public int Timeout
        {
            get { return this.command.CommandTimeout; }
            set { this.command.CommandTimeout = value; }
        }

        public virtual CommandBehavior Behavior => this.commandBehavior;

        public virtual CommandType Type
        {
            get { return this.command.CommandType; }
            set { this.command.CommandType = value; }
        }

        public int Execute() => this.command.ExecuteNonQuery();

        public object Fetch() => this.command.ExecuteScalar();

        public T Fetch<T>() where T : class
        {
            return Fetch<T>(CommandBehavior.CloseConnection);
        }

        public T Fetch<T>(CommandBehavior behavior) where T :class
        {
            var close = false;
            if (behavior == CommandBehavior.CloseConnection)
            {
                close = this.connection.State == DataConnectionState.Closed;
                if (close)
                    this.connection.Open();
            }
            
            
            var t = typeof(T);
            T result = default(T);
            using (var dr = this.FetchReader())
            {
                if(dr.Read())
                {
                    result = (T)this.mapData(dr, t);
                }
            }

            if (close)
                this.connection.Close();

            return result;
        }

        public Task<object> FetchAsync() 
            => this.command2?.ExecuteScalarAsync();

        public Task<object> FetchAsync(CancellationToken cancellationToken)
            => this.command2?.ExecuteScalarAsync(cancellationToken);

        public IDataReader FetchReader() => this.FetchReader(this.commandBehavior);

        public IDataReader FetchReader(CommandBehavior behavior) => new DataReader(this.command.ExecuteReader(behavior));

        public virtual Task<IDataReader> FetchReaderAsync() => this.FetchReaderAsync(CommandBehavior.Default);

        public async virtual Task<IDataReader> FetchReaderAsync(CommandBehavior behavior)
        {
           if(this.command2 != null)
            {
                var dr = await this.command2?.ExecuteReaderAsync(behavior);
                return new DataReader(dr);
            }

            var dr2 = this.command.ExecuteReader(behavior);
            return new DataReader(dr2);
        }

        public virtual Task<IDataReader> FetchReaderAsync(CancellationToken cancellationToken)
            => this.FetchReaderAsync(CommandBehavior.Default, cancellationToken);


        public async virtual Task<IDataReader> FetchReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
        {
            if(this.command2 != null)
            {
                var dr = await this.command2?.ExecuteReaderAsync(behavior, cancellationToken);
                return new DataReader(dr);
            }

            if(cancellationToken.IsCancellationRequested)
                return null;

            var dr2 = this.command.ExecuteReader(behavior);
            return new DataReader(dr2);
        }

    

        public ICollection<T> FetchCollection<T>() where T : class
        {
            return FetchCollection<T>(CommandBehavior.Default);
        }

        public ICollection<T> FetchCollection<T>(CommandBehavior behavior) where T : class
        {
            var close = false;
            if(behavior == CommandBehavior.CloseConnection)
            {
                close = this.connection.State == DataConnectionState.Closed;
                if (close)
                    this.connection.Open();
            }

            var list = new List<T>();
            var t = typeof(T);
            using (var dr = this.FetchReader())
            {
                while (dr.Read())
                {
                    var instance = this.mapData(dr, t);
                    list.Add((T)instance);
                }
            }

            if (close)
                this.connection.Close();

            return list;
        }

        public virtual Task<ICollection<T>> FetchCollectionAsync<T>() where T : class
        {
            return FetchCollectionAsync<T>(CommandBehavior.Default);
        }

        public async virtual Task<ICollection<T>> FetchCollectionAsync<T>(CommandBehavior behavior) where T : class
        {
            var list = new List<T>();
            var t = typeof(T);
            using (var dr = await this.FetchReaderAsync())
            {
                while (dr.Read())
                {
                    var instance = this.mapData(dr, t);
                    list.Add((T)instance);
                }
            }

            return list;
        }

        public virtual Task<ICollection<T>> FetchCollectionAsync<T>(CancellationToken cancellationToken) where T : class
        {
            return FetchCollectionAsync<T>(CommandBehavior.Default, cancellationToken);
        }

        public async virtual Task<ICollection<T>> FetchCollectionAsync<T>(CommandBehavior behavior, CancellationToken cancellationToken) where T : class
        {
            var list = new List<T>();
            var t = typeof(T);
            using (var dr = await this.FetchReaderAsync(behavior, cancellationToken))
            {
                while (dr.Read())
                {
                    var instance = this.mapData(dr, t);
                    list.Add((T)instance);
                }
            }

            return list;
        }

        public void Prepare() => this.command.Prepare();


        ICollection<IDbDataParameter> IDataCommand.Parameters
        {
            get { return this.Parameters; }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.command.Dispose();
                    this.command = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DataCommand() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        public Task<int> ExecuteAsync() 
        {
            if(this.command2 != null)
                return this.command2.ExecuteNonQueryAsync();

            return Task.Run(() => this.command.ExecuteNonQuery());
        }

        public Task<int> ExecuteAsync(CancellationToken token)
        {
           if(this.command2 != null)
                return this.command2.ExecuteNonQueryAsync(token);

            return Task.Run(() => this.command.ExecuteNonQuery(), token);
        }
        #endregion
    }
}
