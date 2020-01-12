

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace NerdyMishka.Extensions.Logging
{

    public interface ICachedMetric
    {
        ICachedMetric TrackValue(double value);

        ICachedMetric TrackValue(double value, string dimension);
    }

    public class ExceptionTelemetry : ITelemetry, ISupportProperties
    {
        public ExceptionTelemetry(Exception exception)
        {
            this.Exception = exception;
        }

        public virtual Exception Exception { get; set; }

        public virtual string Sequence { get; set; }

        public virtual DateTimeOffset Timestamp { get; set; }

        public virtual IDictionary<string, string> Properties { get; set; }

        public virtual LogLevel LogLevel { get; set; }
    }


    public class EventTelemetry : ITelemetry, ISupportProperties, ISupportMetrics
    {
        public EventTelemetry(string name = null)
        {
            this.Name = name;
        }

        public virtual string Name { get; set; }

        public virtual string Sequence { get; set; }

        public virtual DateTimeOffset Timestamp { get; set; }

        public virtual IDictionary<string, string> Properties { get; set; }

        public virtual IDictionary<string, double> Metrics { get; set; }
    }

    public class TraceTelemetry : ITelemetry, ISupportProperties
    {

        public TraceTelemetry(string message = null)
        {
            this.Message = message;
        }

        public virtual string Message { get; set; }

        public virtual string Sequence { get; set; }

        public virtual DateTimeOffset Timestamp { get; set; }

        public virtual LogLevel LogLevel { get; set; }

        public virtual IDictionary<string, string> Properties { get; set; }
    }

    public interface ISupportMetrics 
    {
        IDictionary<string, double> Metrics { get; set; }
    }

    public interface ISupportProperties
    {
        IDictionary<string, string> Properties { get; set; }
    }

    public interface ITelemetry
    {
        string Sequence { get; set; }
        DateTimeOffset Timestamp { get; set; }
    }


    public interface ITelemetryClient
    {

        void Flush();

        ICachedMetric GetMetric(string metricId);

        ICachedMetric GetMetric(string metricId, string dimension1);

        ICachedMetric GetMetric(string metricId, string dimension1, string dimension2);

        ICachedMetric GetMetric(string metricId, string dimension1, string dimension2, string dimension3);

        ICachedMetric GetMetric(string metricId, string dimension1, string dimension2, string dimension3, string dimension4);

        ITelemetryClient TrackAvailability (
            string name, 
            DateTimeOffset timeStamp, 
            TimeSpan duration, 
            string runLocation, 
            bool success, 
            string message = null, 
            IDictionary<string,string> properties = null, 
            IDictionary<string,double> metrics = null);


        ITelemetryClient TrackDependency(
            string dependencyTypeName, 
            string dependencyName, 
            string data, 
            DateTimeOffset startTime, 
            TimeSpan duration, 
            bool success);

        ITelemetryClient TrackDependency(
           string dependencyTypeName, 
           string target, 
           string dependencyName, 
           string data, 
           DateTimeOffset startTime, 
           TimeSpan duration, 
           string resultCode, 
           bool success);

        ITelemetryClient TrackEvent (
            string eventName, 
            IDictionary<string,string> properties = null, 
            IDictionary<string,double> metrics = null,
            DateTimeOffset? timeStamp = null);    

        
        ITelemetryClient Track(ITelemetry telemetry);

        ITelemetryClient TrackException(
            Exception exception, 
            IDictionary<string,string> properties = null, 
            IDictionary<string,double> metrics = null);

        ITelemetryClient TrackMetric(string metricId, double value);

        ITelemetryClient TrackTrace(string message, 
            LogLevel logLevel = LogLevel.Information, 
            System.Collections.Generic.IDictionary<string,string> properties = null);

        ITelemetryClient TrackTrace(string message, 
            int logLevel = -1, 
            System.Collections.Generic.IDictionary<string,string> properties = null);

        ITelemetryClient TrackPageRequest(string name);

        ITelemetryClient TrackRequest(
            string name, 
            System.DateTimeOffset startTime, 
            TimeSpan duration, 
            string responseCode = "200", 
            bool success = true);
    
    }
}