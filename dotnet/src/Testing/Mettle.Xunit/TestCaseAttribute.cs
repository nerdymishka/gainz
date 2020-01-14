

using Xunit;

namespace Mettle 
{

    
    [System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public abstract class TestCaseAttribute : FactAttribute
    {
        
       
        public TestCaseAttribute()
        {
            
        }
        
        /// <summary>
        /// Gets or sets the SkipReason for this test.
        /// </summary>
        /// <value></value>
        public string SkipReason { get; set; }

        /// <summary>
        /// Gets or sets a link to the ticket this test was created for.
        /// </summary>
        /// <value></value>
        public string Ticket { get; set; }

        /// <summary>
        /// Gets or sets the ticket id, which can be used as a filter
        /// </summary>
        /// <value></value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the tags/categories
        /// </summary>
        /// <value></value>
        public string Tags { get; set; }
    }
}