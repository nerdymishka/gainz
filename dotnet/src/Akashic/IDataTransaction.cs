using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.Data
{
    public interface IDataTransaction : ISqlExecutor, IDisposable
    {
        void SetAutoCommit();

        IDataConnection Connection { get; }

        void Commit();

        void Rollback();
    }
}
