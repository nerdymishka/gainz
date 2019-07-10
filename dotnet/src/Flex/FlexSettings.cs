using System.Collections.Generic;

namespace NerdyMishka.Flex
{
    public class FlexSettings
    {
        public FlexSettings()
        {
            this.DefaultDateTimeOptions = new FlexDateTimeOptions() {
                IsUtc = false,
                Format = "yyyy-MM-ddTHH:mm:ss.FFFFFFFK",
                Name = "Default",
                Provider = null
            };

            this.DateTimeOptions = new List<FlexDateTimeOptions>();
        }

        public string NullValue { get; set; } = "null";

        public INamingConvention NamingConvention { get; set; }

        public bool OmitNulls { get; set; } = false;

        public IFlexCryptoProvider CryptoProvider { get; set; }

        public FlexDateTimeOptions DefaultDateTimeOptions { get; set; }

        public IList<FlexDateTimeOptions> DateTimeOptions { get; set; }
    }
}