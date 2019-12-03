

using System.Collections.Generic;

namespace NerdyMishka.Data
{
    public interface IConstraintReference : INamedDbObject
    {
        IEnumerable<IColumnReference> Columns { get; }    
    }
}