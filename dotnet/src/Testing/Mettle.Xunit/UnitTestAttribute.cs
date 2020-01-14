using System;
using Xunit;
using Xunit.Sdk;


namespace Mettle
{
    [XunitTestCaseDiscoverer("Mettle.Xunit.Sdk.TestCaseDiscoverer", "Mettle.Xunit")]
    public class UnitTestAttribute : TestCaseAttribute
    {
        public UnitTestAttribute()
        {
            this.Tags = this.Tags ?? "unit";
        }
    }
}