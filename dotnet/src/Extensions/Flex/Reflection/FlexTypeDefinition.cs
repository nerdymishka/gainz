using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Flex.Reflection
{
    public class FlexTypeDefinition
    {
        public Type Type { get; set; }

        public bool IsList { get; set; }

        public bool IsDictionary { get; set; }

        public bool IsNullable { get; set; }

        public Type ValueType { get; set; }

        public Type KeyType { get; set; }

        public bool IsArray { get; set; }

        public Type ListType { get; set; }

        public Dictionary<string, FlexPropertyDefinition> Properties { get; set; } = new Dictionary<string, FlexPropertyDefinition>();

        public bool IsDataType { get; set; }
    }
}
