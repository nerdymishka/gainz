using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NerdyMishka.Data
{
    public interface IDataReader : IDataRecord, IEnumerable<IDataRecord>, IDisposable
    {
      

        int Depth { get; }

    
        bool HasRows { get; }

        bool IsClosed { get; }

        bool Read();

        Task<bool> ReadAsync();

        Task<bool> ReadAsync(CancellationToken cancellationToken);

        bool NextResult();

        Task<bool> NextResultAsync();

        Task<bool> NextResultAsync(CancellationToken cancellationToken);
    }
}
