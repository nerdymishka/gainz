using System;

namespace NerdyMishka.Extensions.Console.Options
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, 
        Inherited = true, 
        AllowMultiple = false)]
    public class OptionAttribute : AliasAttribute
    {

        public OptionAttribute(string name, string alias = null)
        {
            this.Name = name;
            this.Alias = alias;
        }

        public bool IsDefault { get; set; }
    }
}
