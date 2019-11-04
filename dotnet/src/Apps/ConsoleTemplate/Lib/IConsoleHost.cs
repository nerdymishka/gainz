using System;
using Microsoft.Extensions.Hosting;

namespace NerdyMishka.Extensions.Hosting.Console 
{

    public interface IConsoleHost : IHost
    {
        int ExitCode  { get;  }

        AggregateException Exception { get;  }
    }
}