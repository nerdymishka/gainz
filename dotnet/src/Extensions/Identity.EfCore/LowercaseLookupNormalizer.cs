using Microsoft.AspNetCore.Identity;

namespace NerdyMishka.Identity
{
    public class LowercaseLookupNormalizer : ILookupNormalizer
    {
        public string Normalize(string key)
        {
            return key?.ToLowerInvariant();
        }

        public string NormalizeEmail(string email)
        {
            return email?.ToLowerInvariant();
        }

        public string NormalizeName(string name)
        {
            return name?.ToLowerInvariant();
        }
    }
}