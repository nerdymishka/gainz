using System;
using Xunit.Sdk;


namespace Mettle
{   
    [XunitTestCaseDiscoverer("Mettle.Xunit.Sdk.TestCaseDiscoverer", "Mettle.Xunit")]
    public class IntegrationAttribute : TestCaseAttribute
    {
        public IntegrationAttribute()
        {
            this.Tags = this.Tags ?? "integration";
        }
    }
}