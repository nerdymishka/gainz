using System;
using Xunit.Sdk;


namespace Mettle
{
    public class IntegrationAttribute : TagAttribute
    {
        public IntegrationAttribute():base("tag", "integration")
        {
            
        }
    }
}