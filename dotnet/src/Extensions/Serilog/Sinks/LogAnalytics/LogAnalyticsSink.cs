using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Serilog.Core;
using Serilog.Events;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;

namespace  NerdyMishka.Extensions.Logging
{
    public class LogAnalyticsSinkOptions
    {
        public int BufferSize { get; set; } = 20;
    }

    public class LogAnalyticsSink : ILogEventSink, IDisposable
    {
        private long isDisposing = 0;
        private long isDisposed = 0;


        private LogAnalyticsSinkOptions options;

        private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        private ConcurrentQueue<LogEvent> events = new ConcurrentQueue<LogEvent>();

        private LogAnalyticsWriter writer;

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



        /// <summary>
        /// Creates a sink that saves logs to the Application Insights account for the given <paramref name="telemetryClient" /> instance.
        /// </summary>
        /// <param name="telemetryClient">Required Application Insights <paramref name="telemetryClient" />.</param>
        /// <param name="telemetryConverter">The <see cref="LogEvent"/> to <see cref="ITelemetry"/> converter.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null for default provider.</param>
        /// <exception cref="ArgumentNullException"><paramref name="telemetryClient" /> cannot be null</exception>
        public LogAnalyticsSink(
            LogAnalyticsWriter logAnalyticsWriter,
            IFormatProvider formatProvider = null)
        {
            this.writer = logAnalyticsWriter ?? throw new ArgumentNullException(nameof(LogAnalyticsWriter));
            this.formatProvider = formatProvider;
        }



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
                Task.Run(async() => {
                    await this.semaphore.WaitAsync();
                    this.events.Enqueue(logEvent);

                    if(this.events.Count < this.options.BufferSize)
                    {
                        this.semaphore.Release();
                        return;
                    }

                    
                    var messages = new List<object>();
                    var options = new JsonSerializerOptions() {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };

                    foreach(var e in this.events)
                    {
                        var data = JsonSerializer.Serialize(logEvent.Properties);
                        string exception = null;
                        if(e.Exception != null)
                        {
                            var list = new List<object>();
                            var ex = e.Exception;
                            while(ex != null)
                            {
                                var exceptionData = new Dictionary<string, object>();
                                var trace = new StackTrace(ex);
                                var frames = trace.GetFrames();
                                var stack = new List<object>();
                                
                                foreach(var frame in trace.GetFrames())
                                {
                                    var line = new Dictionary<string, object>();
                                    if(frame.HasMethod())
                                    {
                                        var method = frame.GetMethod();
                                        var parameters = method.GetParameters();
                                    
                                        line.Add("class", method.DeclaringType.FullName);
                                        line.Add("assembly", method.DeclaringType.Assembly.FullName);
                                        line.Add("method", frame.GetMethod().Name);
                                        var args = new List<string>();
                                        if(parameters != null && parameters.Length >0)
                                        {
                                            
                                            foreach(var p in parameters) {
                                                args.Add(p.ParameterType.FullName + " " + p.Name);
                                            }   
                                        }

                                        line.Add("parameters", args);
                                    }

                                    if(frame.HasSource())
                                    {
                                        line.Add("file", frame.GetFileName());
                                        line.Add("line", frame.GetFileLineNumber());
                                        line.Add("column", frame.GetFileColumnNumber());
                                    }
                                        
                                    stack.Add(line);
                                }
                                exceptionData.Add("stackFrames", stack);
                                exceptionData.Add("stackTrack", ex?.StackTrace?.Split('\n'));
                                list.Add(exceptionData);

                                ex = ex.InnerException;
                            }

                            exception = JsonSerializer.Serialize(list, options);
                        }

                        var message = new Dictionary<string, string>()
                        {
                            {"message", logEvent.RenderMessage(this.formatProvider) },
                            {"logLevel", logEvent.Level.ToString() },
                            {"data", data },
                            {"timestamp",  logEvent.Timestamp.ToUniversalTime().ToString("o") } ,
                            {"exceptions", exception }
                        };

                        messages.Add(message);
                    }

                    var json = JsonSerializer.Serialize(messages, options);
                    await this.writer.WriteAsync(json);
                    this.semaphore.Release();
                });
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
                        this.writer?.Dispose();
                    }
                    finally
                    {
                        this.writer = null;
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
