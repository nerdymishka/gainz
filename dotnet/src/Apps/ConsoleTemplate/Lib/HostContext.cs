

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;

namespace NerdyMishka.Extensions.Hosting.Console
{

    public interface IHostContext
    {
        IDictionary<string, object> Properties { get; }

        IHostEnvironment HostEnvironment { get;  }

        IServiceProvider Services { get; }

        IList<string> Arguments { get; }

        System.Security.Claims.ClaimsPrincipal User  { get;set; }
    }

    public class HostContext : IHostContext
    {
       

        public HostContext(
            IHostEnvironment environment,
            IServiceProvider services, 
            IConsoleArguments arguments) {

            this.HostEnvironment = environment;
            this.Services = services;
            
            this.Properties = new ConcurrentDictionary<string, object>();
            this.Arguments = arguments?.Arguments;
        }

        public IList<string> Arguments { get; }

        public IDictionary<string, object> Properties { get; }

        public IHostEnvironment HostEnvironment { get; private set; }

        public IServiceProvider Services { get; private set; }

        public System.Security.Claims.ClaimsPrincipal User  { get;set; }
    }
}