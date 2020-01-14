using System;
using Xunit;
using Xunit.Sdk;


namespace Mettle
{
    [XunitTestCaseDiscoverer("Mettle.Xunit.Sdk.TestCaseDiscoverer", "Mettle.Xunit")]
    public class UnitAttribute : TestCaseAttribute
    {
        public UnitAttribute()
        {
            this.Tags = this.Tags ?? "unit";
        }
    }
}