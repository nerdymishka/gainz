using System;
using Xunit.Sdk;


namespace Mettle
{
    [XunitTestCaseDiscoverer("UnitTestCaseDiscoverer", "NerdyMishka.Mettle.Xunit")]
    public class UnitAttribute : Xunit.FactAttribute
    {


        public UnitAttribute()
        {
            
        }

        public string Ticket { get; set; }


    }
}