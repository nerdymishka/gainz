using NerdyMishka.ComponentModel.DataAnnotations;

namespace NerdyMishka.EfCore.Identity
{
    public class UserToken
    {

        public int UserId  { get; set; }

        public string ProviderName { get; set; }

        public string Name { get; set; }

        [Encrypt]
        public string Value { get; set; }
    }
}