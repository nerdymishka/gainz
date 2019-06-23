namespace NerdyMishka.EfCore.Identity
{
    public class UserToken
    {
        public int UserId  { get; set; }

        public string ProviderName { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }

    public class UserLogin
    {
        public int UserId  { get; set; }

        public string Key { get; set; }

        public string ProviderName { get; set; }

        public string DisplayName { get; set; }
    }
}