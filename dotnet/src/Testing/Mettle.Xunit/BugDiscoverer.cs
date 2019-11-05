
using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mettle
{
    public class BugDiscoverer : ITraitDiscoverer
    {
        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
        {
            var tags = traitAttribute.GetCustomAttributes(typeof(Mettle.BugAttribute));
            foreach(BugAttribute tag in tags) {
              
                yield return new KeyValuePair<string, string>("bug", tag.Version);
                yield return new KeyValuePair<string, string>("bug.version", tag.Version);
                yield return new KeyValuePair<string, string>("bug.uri", tag.Uri);
                yield return new KeyValuePair<string, string>("bug.description", tag.Description);
            }
        }
    }
}


