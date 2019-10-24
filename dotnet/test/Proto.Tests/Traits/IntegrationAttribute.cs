using System;
using Xunit.Sdk;


namespace NerdyMishka
{
    public class IntegrationAttribute : TagAttribute
    {
        public IntegrationAttribute():base("tag", "integration")
        {
            
        }
    }
}