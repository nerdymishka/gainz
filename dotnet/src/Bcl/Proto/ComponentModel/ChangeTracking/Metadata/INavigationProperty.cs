
using System.Collections.Generic;

namespace NerdyMishka.ComponentModel.ChangeTracking.Metadata
{
    public interface INavigationProperty : IProperty
    {
        IRelationship Relationship { get; }
    }
}