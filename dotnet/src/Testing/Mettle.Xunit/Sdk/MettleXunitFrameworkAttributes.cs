using System;
using Xunit.Sdk;


namespace Mettle.Xunit.Sdk
{
    [TestFrameworkDiscoverer("Mettle.Xunit.Sdk.MettleTestFrameworkTypeDiscoverer", "Mettle.Xunit")]
    [System.AttributeUsage(System.AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    public sealed class MettleXunitFrameworkAttribute : System.Attribute, ITestFrameworkAttribute
    {
    
        public Type Type { get; set; }

        public string Assembly { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public MettleXunitFrameworkAttribute()
        {
            
        }
        
    }
}