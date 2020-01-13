
using System;
using Microsoft.Extensions.Logging;
using Serilog;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mettle
{
    


    public class SerilogTestLogger : ITestLogger
    {
        private string name;
        public SerilogTestLogger(
            string name = null, 
            ITestOutputHelper helper = null)
        {
            this.name = name ?? "";
            this.helper = helper ?? new TestOutputHelper();
        }

        private ITestOutputHelper helper = new TestOutputHelper();
        private Microsoft.Extensions.Logging.ILogger logger;
        public ITestOutputHelper Helper => helper;

        public Serilog.Events.LogEventLevel Level { get; set; } = Serilog.Events.LogEventLevel.Debug;

        public Microsoft.Extensions.Logging.ILogger Log 
        {
            get{
                if(this.logger == null)
                {
                    this.logger = Microsoft.Extensions.Logging.LoggerFactory.Create((lb) => {
                        lb.AddSerilog(new LoggerConfiguration()
                            .MinimumLevel.Verbose()
                            .WriteTo.TestOutput(this.helper, this.Level)
                            .CreateLogger());
                            
                    }).CreateLogger(this.name);
                }

                return this.logger;
            }
        }
    }
}