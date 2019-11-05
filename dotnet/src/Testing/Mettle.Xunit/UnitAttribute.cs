using System;
using Xunit.Sdk;


namespace Mettle
{
    public class UnitAttribute : TagAttribute
    {
        public UnitAttribute():base("tag", "unit")
        {
            
        }
    }
}