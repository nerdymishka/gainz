
using System;
using Microsoft.Extensions.Hosting;

namespace NerdyMishka.Extensions.Hosting.Console 
{
    public interface IConsoleHostedService : IHostedService
    {
        int ExitCode  { get;  }

        AggregateException Exception { get;  }
    }
}

