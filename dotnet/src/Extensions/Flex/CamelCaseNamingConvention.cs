using System.Collections.Generic;
using System.Text;


namespace NerdyMishka.Flex
{
    public class CamelCaseNamingConvention : INamingConvention
    {

        public CamelCaseNamingConvention(bool cache = false)
        {
            if(cache)
                this.Cache = new Dictionary<string, string>();
        }

        public virtual string Name => "Default";
        public string Provider { get; set; }

        private Dictionary<string, string> Cache { get; set; } 

        public virtual string Transform(string value)
        {
            if(string.IsNullOrWhiteSpace(value))
                return value;

            if(this.Cache != null && this.Cache.TryGetValue(value, out string result))
                return result;

            var sb = new StringBuilder();
            var transform = false;
            for(int i = 0; i < value.Length; i++) {
                var c = value[i];
                if(i == 0 && char.IsUpper(c))
                {
                    sb.Append(char.ToLowerInvariant(c));
                    continue;
                }

                if(c == '-' || c == '_')
                {
                    transform = true;
                    continue;
                }

                if(transform)
                {
                    sb.Append(char.ToUpperInvariant(c));
                    continue;
                }

                sb.Append(c);
            }
            
            var camelCasedName = sb.ToString();
            if(this.Cache != null)
                this.Cache.Add(value, camelCasedName);

            return camelCasedName;
        }
    }
}