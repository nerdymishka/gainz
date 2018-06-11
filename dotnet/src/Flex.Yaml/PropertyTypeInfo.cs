using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NerdyMishka.Flex.Yaml
{
    public class PropertyTypeInfo
    {
        private string symbol;

        public string Name => this.Info.Name;

        public PropertyInfo Info { get; set; }

        public DefaultPropertyAttribute Default { get; set; }

        public SwitchAttribute Switch { get; set; }

        public EncryptAttribute Encrypt { get; set; }

        public IgnoreAttribute Ignore { get; set; }

        public SymbolAttribute Symbol { get; set; }

        public DateTimeFormatAttribute[] DateTimeFormats { get; set; }

        public bool IsIgnored => this.Ignore != null;

        public bool IsEncrypted => this.Encrypt != null;

        public bool IsSwitch => this.Switch != null;

        public bool IsDefault => this.Default != null;

        public bool HasSymbol => this.Symbol != null;
    }
}
