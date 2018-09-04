using Nexus.Api;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Nexus.Services
{
    public static class Response
    {
        public static PagedApiActionResponse<T> PagedEmpty<T>(
            int page = 1,
            int size = 20
        ) {
            return new PagedApiActionResponse<T>() {
                Results = Array.Empty<T>(),
                Page = page, 
                Size = size,
                Hits = 0,
                Ok = true 
            };
        }

        public static PagedApiActionResponse<T> PagedOk<T>(
            T[] results, 
            int page = 1, 
            int size = 20,
            int? hits = null)
        {
            return new PagedApiActionResponse<T>() {
                Results = results,
                Page = page,
                Size = 20,
                Hits = hits,
                Ok = true 
            };
        }

        public static PagedApiActionResponse<T> PagedFail<T>(
            string message, 
            ILogger logger,
            Exception ex,
            int page = 1,
            int size = 20,
            int? hits = null
            ) {

            if(logger.IsEnabled(LogLevel.Error))
                logger.LogError(ex, message);

            return PagedFail<T>(message);
        }

        public static PagedApiActionResponse<T> PagedFail<T>(
            string message, 
            ILogger logger,
            Exception ex,
            EventId eventId,
            int page = 1,
            int size = 20,
            int? hits = null
            ) {

            if(logger.IsEnabled(LogLevel.Error))
                logger.LogError(eventId, ex, message);

            return PagedFail<T>(message);
        }

        public static PagedApiActionResponse<T> PagedFail<T>(
            string message, 
            int page = 1, 
            int size = 20,
            int? hits = null)
        {
            return new PagedApiActionResponse<T>() {
                Results = default(T[]),
                Page = page,
                Size = 20,
                Hits = hits,
                Ok = false,
                ErrorMessages = new [] {message}
            };
        }

        public static PagedApiActionResponse<T> PagedFail<T>(
            string[] messages, 
            int page = 1, 
            int size = 20,
            int? hits = null)
        {
            return new PagedApiActionResponse<T>() {
                Results = default(T[]),
                Page = page,
                Size = 20,
                Hits = hits,
                Ok = false,
                ErrorMessages = messages
            };
        }

        public static PagedApiActionResponse<T> PagedOk<T>(
            IEnumerable<T> results, 
            int page = 1, 
            int size = 20,
            int? hits = null)
        {
            return new PagedApiActionResponse<T>() {
                Results = results.ToArray(),
                Page = page,
                Size = 20,
                Hits = hits,
                Ok = true 
            };
        }

        public static ApiActionResponse<T> Ok<T>(T result)
        {
            return new ApiActionResponse<T>() {
                Ok = true,
                Result = result,
            };
        }

        public static ApiActionResponse<T> Cancel<T>(string taskName)
        {
            return new ApiActionResponse<T>() {
                Ok = false,
                Result = default(T),
                ErrorMessages = new [] { $"{taskName} was cancelled"},
            };
        }

        public static ApiActionResponse<T> Fail<T>(string message)
        {
            return new ApiActionResponse<T>() {
                Ok = false,
                Result = default(T),
                ErrorMessages = new [] {message }
            };
        }

        public static ApiActionResponse<T> Fail<T>(string message, ILogger logger, Exception ex)
        {
            if(logger.IsEnabled(LogLevel.Error))
                logger.LogError(ex, message);
            
            return Fail<T>(message);
        }

        
        public static ApiActionResponse<T> Fail<T>(string message, ILogger logger, Exception ex, EventId eventId)
        {
            if(logger.IsEnabled(LogLevel.Error))
                logger.LogError(eventId, ex, message);
            
            return Fail<T>(message);
        }

        public static ApiActionResponse<T> Fail<T>(string[] messages)
        {
            return new ApiActionResponse<T>() {
                Ok = false,
                Result = default(T),
                ErrorMessages = messages 
            };
        }
    }
}