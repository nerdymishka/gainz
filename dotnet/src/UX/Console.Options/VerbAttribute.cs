using System;

namespace NerdyMishka.Extensions.Console.Options
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property,
        Inherited = true, 
        AllowMultiple = false)]
    public class VerbAttribute : AliasAttribute
    {

        public VerbAttribute(string name, string alias = null)
        {
            this.Name = name;
            this.Alias= alias;
        }

        public bool IsDefault { get; set; }
    }
}
