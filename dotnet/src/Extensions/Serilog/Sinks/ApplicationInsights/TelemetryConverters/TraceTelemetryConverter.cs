using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NerdyMishka.Extensions.Logging;
using Serilog.Events;

namespace Serilog.Sinks.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters
{
    public class TraceTelemetryConverter : TelemetryConverterBase
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual TraceTelemetry GenerateTraceTelemetry(LogEvent logEvent, IFormatProvider formatProvider)
        {
            return new TraceTelemetry(logEvent.RenderMessage(formatProvider))
            {
                Timestamp = logEvent.Timestamp,
                LogLevel = ToSeverityLevel(logEvent.Level)
            };
        }

        public override IEnumerable<ITelemetry> Convert(LogEvent logEvent, IFormatProvider formatProvider)
        {
            if (logEvent == null)
                throw new ArgumentNullException(nameof(logEvent));

            if (logEvent.Exception == null)
            {
                var renderedMessage = logEvent.RenderMessage(formatProvider);

                var telemetry = this.GenerateTraceTelemetry(logEvent, formatProvider);

                // write logEvent's .Properties to the AI one
                ForwardPropertiesToTelemetryProperties(logEvent, telemetry, formatProvider);

                yield return telemetry;
            }
            else
            {
                yield return ToExceptionTelemetry(logEvent, formatProvider);
            }
        }
    }
}
