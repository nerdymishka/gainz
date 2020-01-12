using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NerdyMishka.Extensions.Logging;
using Serilog.Events;

namespace Serilog.Sinks.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters
{
    public class EventTelemetryConverter : TelemetryConverterBase
    {   

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual EventTelemetry GenerateEventTelemetry(LogEvent logEvent)
        {
            return new EventTelemetry(logEvent.MessageTemplate.Text) {
                Timestamp = logEvent.Timestamp
            };
        }

        public override IEnumerable<ITelemetry> Convert(LogEvent logEvent, IFormatProvider formatProvider)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

            if (logEvent.Exception == null)
            {
                var telemetry = this.GenerateEventTelemetry(logEvent);

                // write logEvent's .Properties to the AI one
                ForwardPropertiesToTelemetryProperties(logEvent, telemetry, formatProvider);

                yield return telemetry;
            }
            else
            {
                yield return ToExceptionTelemetry(logEvent, formatProvider);
            }
        }

        public override void ForwardPropertiesToTelemetryProperties(LogEvent logEvent, ISupportProperties telemetryProperties, IFormatProvider formatProvider)
        {
            ForwardPropertiesToTelemetryProperties(logEvent, telemetryProperties, formatProvider,
                includeLogLevel: false,
                includeRenderedMessage: true,
                includeMessageTemplate: false);
        }
    }
}
