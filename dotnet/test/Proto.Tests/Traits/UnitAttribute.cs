using System;
using Xunit.Sdk;


namespace NerdyMishka
{
    public class UnitAttribute : TagAttribute
    {
        public UnitAttribute():base("tag", "unit")
        {
            
        }
    }
}