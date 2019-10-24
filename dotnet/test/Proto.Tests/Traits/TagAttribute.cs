using System;
using Xunit.Sdk;


namespace NerdyMishka
{

    [TraitDiscoverer("NerdyMishka.TagDiscoverer", "NerdyMishka.Proto.Tests")]
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