using Xunit.Abstractions;
using Xunit;
using Microsoft.Extensions.Logging;

namespace Mettle
{


    public class TestCase 
    {
        public ILogger Log { get; }
        
        public ITestOutputHelper Output { get; }

        public IAssert Assert { get; }
       

        public TestCase(
            ITestOutputHelper output)
        {
            var testLogger = new SerilogTestLogger(null, output);
            this.Output = testLogger.Helper;
            this.Log = testLogger.Log;
            this.Assert = AssertImpl.Current;
        }
    }
}