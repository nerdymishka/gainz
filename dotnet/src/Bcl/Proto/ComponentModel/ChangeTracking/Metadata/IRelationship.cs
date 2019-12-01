
using System.Collections.Generic;

namespace NerdyMishka.ComponentModel.ChangeTracking.Metadata
{
    public interface IRelationship
    {
        IEntityType DependantType { get; }

        IReadOnlyList<IProperty> DependantKeyProperties { get; }

        IKey ParentKey { get; set; }

        IEntityType ParentType { get; set; }
        
        IKey IDependentKey { get; set; }

        INavigationProperty DependentProperty { get;  }

        INavigationProperty ParentalProperty { get; }

        bool IsRequired { get; }
    }
}