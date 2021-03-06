﻿// Copyright 2016 Serilog Contributors
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
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters;

namespace  NerdyMishka.Extensions.Logging.Sinks.ApplicationInsights
{
    /// <summary>
    /// Base class for Microsoft Azure Application Insights based Sinks.
    /// Inspired by their NLog Appender implementation.
    /// </summary>
    public class ApplicationInsightsSink : ILogEventSink, IDisposable
    {
        private long isDisposing = 0;
        private long isDisposed = 0;

        private ITelemetryClient telemetryClient;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is being disposed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is being disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposing
        {
            get => Interlocked.Read(ref isDisposing) == 1;
            protected set => Interlocked.Exchange(ref isDisposing, value ? 1 : 0);
        }

        /// <summary>
        /// Gets a value indicating whether this instance has been disposed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has been disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed
        {
            get => Interlocked.Read(ref isDisposed) == 1;
            protected set => Interlocked.Exchange(ref isDisposed, value ? 1 : 0);
        }

        private readonly IFormatProvider formatProvider;

        private readonly ITelemetryConverter telemetryConverter;

        /// <summary>
        /// Creates a sink that saves logs to the Application Insights account for the given <paramref name="telemetryClient" /> instance.
        /// </summary>
        /// <param name="telemetryClient">Required Application Insights <paramref name="telemetryClient" />.</param>
        /// <param name="telemetryConverter">The <see cref="LogEvent"/> to <see cref="ITelemetry"/> converter.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null for default provider.</param>
        /// <exception cref="ArgumentNullException"><paramref name="telemetryClient" /> cannot be null</exception>
        public ApplicationInsightsSink(
            NerdyMishka.Extensions.Logging.ITelemetryClient telemetryClient,
            ITelemetryConverter telemetryConverter,
            IFormatProvider formatProvider = null)
        {
            this.telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            this.telemetryConverter = telemetryConverter ?? throw new ArgumentNullException(nameof(telemetryConverter));

            this.formatProvider = formatProvider;
        }

        #region AI specifc Helper methods

        /// <summary>
        /// Hands over the <paramref name="telemetry" /> to the AI telemetry client.
        /// </summary>
        /// <param name="telemetry">The telemetry.</param>
        /// <exception cref="System.ArgumentNullException">telemetry</exception>
        protected virtual void TrackTelemetry(ITelemetry telemetry)
        {
            if (telemetry == null) throw new ArgumentNullException(nameof(telemetry));

            CheckForAndThrowIfDisposed();

            // the .Track() method is safe to use (even though documented otherwise)
            // see https://github.com/Microsoft/ApplicationInsights-dotnet/issues/244
            this.telemetryClient?.Track(telemetry);
        }

        #endregion AI specifc Helper methods

        #region Implementation of ILogEventSink

        /// <summary>
        /// Emit the provided log event to the sink.
        /// </summary>
        /// <param name="logEvent">The log event to write.</param>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        /// <exception cref="TargetInvocationException">A delegate callback throws an exception.</exception>
        public virtual void Emit(LogEvent logEvent)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

            CheckForAndThrowIfDisposed();

            try
            {
                IEnumerable<ITelemetry> telemetries = this.telemetryConverter.Convert(logEvent, this.formatProvider);

                // if 'null' is returned (& we therefore there's nothing to track), the logEvent is basically skipped
                if (telemetries != null)
                {
                    foreach (ITelemetry telemetry in telemetries)
                    {
                        if (telemetry != null)
                        {
                            TrackTelemetry(telemetry);
                        }
                    }
                }
            }
            catch (TargetInvocationException targetInvocationException)
            {
                // rethrow original exception (inside the TargetInvocationException) if any
                if (targetInvocationException.InnerException != null)
                {
                    ExceptionDispatchInfo.Capture(targetInvocationException.InnerException).Throw();
                }
                else
                {
                    throw;
                }
            }
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Checks whether this instance has been disposed and if so, throws an <see cref="ObjectDisposedException"/>.
        /// </summary>
        /// <exception cref="ObjectDisposedException"></exception>
        protected void CheckForAndThrowIfDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposeManagedResources"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (IsDisposing || IsDisposed)
                return;

            try
            {
                IsDisposing = true;

                // we only have managed resources to dispose of
                if (disposeManagedResources)
                {
                    // attempt to free managed resources
                    try
                    {
                        this.telemetryClient?.Flush();
                    }
                    finally
                    {
                        this.telemetryClient = null;
                    }
                }
            }
            finally
            {
                IsDisposed = true;
                IsDisposing = false;
            }
        }

        #endregion Implementation of IDisposable
    }
}
