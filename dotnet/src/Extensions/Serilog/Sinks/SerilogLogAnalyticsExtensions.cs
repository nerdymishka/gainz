

using System;
using NerdyMishka.Extensions.Logging;
using NerdyMishka.Extensions.Logging.Sinks.ApplicationInsights;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.ApplicationInsights;
using Serilog.Sinks.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters;

namespace Serilog
{

    /// <summary>
    /// Adds the WriteTo.ApplicationInsights() extension method to <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class SerilogLogAnalyticsExtensions
    {
        /// <summary>
        /// Adds a Serilog sink that writes <see cref="LogEvent">log events</see> to Microsoft Application Insights 
        /// using a custom <see cref="ITelemetry"/> converter / constructor.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="telemetryConfiguration">Required Application Insights configuration settings.</param>
        /// <param name="telemetryConverter">Required telemetry converter.</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <returns></returns>
        public static LoggerConfiguration ApplicationInsights(
            this LoggerSinkConfiguration loggerConfiguration,
            LogAnalyticsWriter writer,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IFormatProvider formatProvider = null)
        {
            return loggerConfiguration.Sink(new LogAnalyticsSink(writer, formatProvider), restrictedToMinimumLevel);
        }
    }
}
