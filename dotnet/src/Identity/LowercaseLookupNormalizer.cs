using Microsoft.AspNetCore.Identity;

namespace NerdyMishka.Identity
{
    public class LowercaseLookupNormalizer : ILookupNormalizer
    {
        public string Normalize(string key)
        {
            return key?.ToLowerInvariant();
        }
    }
}