using Xunit.Abstractions;
using Xunit;
using Serilog;
using Serilog.Events;

namespace Mettle
{
    public class TestCase 
    {
        protected ILogger Log { get; }

        public TestCase(ITestOutputHelper output, LogEventLevel level = LogEventLevel.Verbose)
        {
            this.Log = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.TestOutput(output, level)
                .CreateLogger()
                .ForContext(this.GetType());
        }
    }
}