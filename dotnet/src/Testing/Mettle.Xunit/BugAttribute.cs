using System;
using Xunit.Sdk;


namespace Mettle
{
    [TraitDiscoverer("NerdyMishka.BugDiscoverer", "NerdyMishka.Mettle.Xunit")]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class BugAttribute : System.Attribute, ITraitAttribute
    {
        public string Version { get; set; }

        public string Uri  { get; set; }

        public string Description { get; set; }

      

        public BugAttribute(string version, string uri = null, string description = null)
        {
             this.Version = version;
             this.Uri = uri;
             this.Description = description;
        }
    }
}