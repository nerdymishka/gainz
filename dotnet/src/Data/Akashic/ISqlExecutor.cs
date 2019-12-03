using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.Data
{
    public interface ISqlExecutor : IObserver<SqlStatementContext>, IDisposable
    {
      

        IDataCommand CreateCommand();

        IDataCommand CreateCommand(CommandBehavior commandBehavior);

        SqlDialect SqlDialect { get; }

        SqlBuilder CreateSqlBuilder();
    }
}
