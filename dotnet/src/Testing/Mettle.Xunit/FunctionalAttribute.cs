using System;
using Xunit.Sdk;


namespace Mettle
{   
    [XunitTestCaseDiscoverer("Mettle.Xunit.Sdk.TestCaseDiscoverer", "Mettle.Xunit")]
    public class FunctionalAttribute : TestCaseAttribute
    {
        public FunctionalAttribute()
        {
            this.Tags = this.Tags ?? "functional";
        }
    }
}