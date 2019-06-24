namespace NerdyMishka.EfCore.Identity
{
    public class UserLogin
    {

        public int UserId  { get; set; }

        public string Key { get; set; }

        public string ProviderName { get; set; }

        public string DisplayName { get; set; }
    }
}