using System;
using Xunit;
using Xunit.Sdk;


namespace Mettle
{
    [XunitTestCaseDiscoverer("UnitTestCaseDiscoverer", "NerdyMishka.Mettle.Xunit")]
    public class UnitAttribute : FactAttribute
    {


        public UnitAttribute()
        {
            
        }

        public string Ticket { get; set; }


    }
}