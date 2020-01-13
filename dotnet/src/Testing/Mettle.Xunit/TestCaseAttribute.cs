

using Xunit;

namespace Mettle 
{
    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public abstract class TestCaseAttribute : System.Attribute
    {
        
       
        public TestCaseAttribute()
        {
            
        }
        
        public string SkipReason { get; set; }

        public string Ticket { get; set; }

        public string Tag { get; set; } = "unit";

        public int Timeout { get; set; }

        public string DisplayName { get; set; }


    }
}