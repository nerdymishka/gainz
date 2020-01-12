// Copyright 2016 Serilog Contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.


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
    public static class SerilogITelemetryClientExtensions
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
            ITelemetryClient client,
            ITelemetryConverter telemetryConverter = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IFormatProvider formatProvider = null)
        {
            if(telemetryConverter == null)
                telemetryConverter = TelemetryConverter.Traces;
        
            return loggerConfiguration.Sink(new ApplicationInsightsSink(client, telemetryConverter, formatProvider), restrictedToMinimumLevel);
        }

      
    }
}
