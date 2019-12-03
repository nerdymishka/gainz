using System;

namespace NerdyMishka.Extensions.Console.Options
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, 
        Inherited = true, 
        AllowMultiple = false)]
    public class FlagAttribute : AliasAttribute
    {

        public FlagAttribute(string name, string alias = null)
        {
            this.Name = name;
            this.Alias = alias;
        }

        public bool DefaultValue { get; set; }
    }
}
