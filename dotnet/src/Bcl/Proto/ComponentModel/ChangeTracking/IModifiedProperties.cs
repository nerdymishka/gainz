
using System.Collections.Generic;

namespace NerdyMishka.ComponentModel.ChangeTracking
{
    public interface IModifiedProperties
    {
        IDictionary<string, object> ModifiedProperties { get; }
    }
}