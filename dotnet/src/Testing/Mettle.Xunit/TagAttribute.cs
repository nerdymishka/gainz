using System;
using Xunit.Sdk;


namespace Mettle
{

    [TraitDiscoverer("Mettle.TagDiscoverer", "NerdyMishka.Mettle.Xunit")]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class TagAttribute : System.Attribute, ITraitAttribute
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public TagAttribute(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}