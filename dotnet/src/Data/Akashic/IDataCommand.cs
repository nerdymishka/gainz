using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NerdyMishka.Data
{
    public interface IDataCommand : IDisposable
    {

        IDbDataParameter AddParameter(object value);

        IDbDataParameter AddParameter(string name, object value);

        IDbDataParameter AddParameter(string name, DbType type, int? size);

        IDbDataParameter AddParameter(string name, DbType type, int precision, int scale);

        IDbDataParameter CreateParameter();

        IDataCommand SetMap(Func<IDataReader, Type, object> map);

        IDataConnection Connection { get; set; }

        ICollection<IDbDataParameter> Parameters { get; }

        string Text { get; set; }

        CommandBehavior Behavior {get; }

        CommandType Type { get; set; }

        int Timeout { get; set; }
    
        int Execute();

        Task<int> ExecuteAsync();

        Task<int> ExecuteAsync(CancellationToken cancellationToken);

        object Fetch();

        T Fetch<T>() where T : class;

        T Fetch<T>(CommandBehavior behavior) where T : class;

        Task<object> FetchAsync();

        Task<object> FetchAsync(CancellationToken cancellationToken);

        IDataReader FetchReader();

        IDataReader FetchReader(CommandBehavior behavior);

        Task<IDataReader> FetchReaderAsync();

        Task<IDataReader> FetchReaderAsync(CommandBehavior behavior);

        Task<IDataReader> FetchReaderAsync(CancellationToken cancellationToken);

        Task<IDataReader> FetchReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken);

        ICollection<T> FetchCollection<T>() where T : class;

        ICollection<T> FetchCollection<T>(CommandBehavior behavior) where T : class;

        Task<ICollection<T>> FetchCollectionAsync<T>() where T : class;

        Task<ICollection<T>> FetchCollectionAsync<T>(CommandBehavior behavior) where T : class;

        Task<ICollection<T>> FetchCollectionAsync<T>(CommandBehavior behavior, CancellationToken cancellationToken) where T : class;

        Task<ICollection<T>> FetchCollectionAsync<T>(CancellationToken cancellationToken) where T : class;

        void Prepare();
    }
}
