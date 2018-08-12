using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NerdyMishka.Data
{
    public interface IDataConnection : ISqlExecutor, IDisposable
    {
        string ConnectionString { get; set; }

        string ProviderName { get;  }

        IDataTransaction BeginTransaction(IsolationLevel level = IsolationLevel.Unspecified);

        DataConnectionState State { get; }

        void Open();
        Task OpenAsync();
        Task OpenAsync(CancellationToken cancellationToken);
        void Close();
    }
}
