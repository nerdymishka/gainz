

using System.Collections.Generic;

namespace NerdyMishka.Data
{
    public interface IColumnReference : INamedDbObject
    {
        ITableReference Table { get; }

        IEnumerable<IConstraintReference> Constraints { get; }

    
    }
}