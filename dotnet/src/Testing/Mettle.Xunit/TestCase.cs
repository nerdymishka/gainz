using Xunit.Abstractions;
using Xunit;
using Serilog;
using Serilog.Events;

namespace Mettle
{

    public class TestCaseOptions
    {
        public LogEventLevel Level { get; set; } = LogEventLevel.Verbose;

        public IAssert Assert { get; set; } = AssertImpl.Current;
    }

    public class TestCase 
    {
        protected ILogger Log { get; }

        protected IAssert Assert { get; }

        public TestCase(
            ITestOutputHelper output, TestCaseOptions options = null)
        {
            options = options ?? new TestCaseOptions();
            this.Assert = options.Assert;
            this.Log = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.TestOutput(output, options.Level)
                .CreateLogger()
                .ForContext(this.GetType());
        }
    }
}