
using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace NerdyMishka
{
    public class TagDiscoverer : ITraitDiscoverer
    {
        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
        {
            var tags = traitAttribute.GetCustomAttributes(typeof(NerdyMishka.TagAttribute));
            foreach(TagAttribute tag in tags) {
              
                yield return new KeyValuePair<string, string>(tag.Name, tag.Value);
            }
        }
    }
}


