using System;
using Xunit.Sdk;


namespace Mettle
{   
    [XunitTestCaseDiscoverer("Mettle.Xunit.Sdk.TestCaseDiscoverer", "Mettle.Xunit")]
    public class IntegrationTestAttribute : TestCaseAttribute
    {
        public IntegrationTestAttribute()
        {
            this.Tags = this.Tags ?? "integration";
        }
    }
}