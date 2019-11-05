
# Extensions.Hosting.Console

Generic Host for a console program that takes care of the HostedService
plumbing and enables spinning up Logging, DepenencyInjection, Configuration,
etc.

The out-of-box scenario for the generic host provided by dotnet seems focused on providing
an alternative to runing a daemon/service.

This extension is focused on enabling a commandline application that may require
logging, configuration, and/or DI.  

## Overview

`IHostedService` is implemented to execute a class that implements `IConsoleProgram`.
There are helper extension methods that implement a delegate console program that
takes a funtion of type `Func<IHostContext, int>` so that implementing IConsoleProgram
isn't required.

```csharp
public interface IConsoleProgram
{
    Task<int> ExecuteAsync(IHostContext context, CancellationToken cancellationToken = default);
}

public interface IHostContext
{
    IDictionary<string, object> Properties { get; }

    IHostEnvironment HostEnvironment { get;  }

    IServiceProvider Services { get; }

    IList<string> Arguments { get; }

    System.Security.Claims.ClaimsPrincipal User  { get;set; } // not set by default. 
}
```

To wire up usage, you need to create a new `HostBuilder` object and then call
`UseConsoleProgram` to pass in arguments, the `IConsoleProgram` implementation.

`UseConsoleProgram` will wire up the basic services that you need:

```csharp
logging.AddConsole();
services.AddSingleton<IOptions<ConsoleHostOptions>>((s) => Options.Create<ConsoleHostOptions>(options));
services.AddSingleton<IConsoleArguments, ConsoleArguments>((s) => new ConsoleArguments(args));
services.AddSingleton<IHostLifetime, ConsoleLifetime>();
services.AddSingleton<IHost, ConsoleHost>();
services.AddSingleton<IConsoleHost, ConsoleHost>();
services.AddSingleton<IConsoleProgram, DelegateConsoleProgram>((s) => {
    return new DelegateConsoleProgram(execute);
});

```

### Sample Usage

```csharp
static int Main(string[] args)
{
    var builder = new HostBuilder().UseConsoleProgram(args, (ctx) => {
        var logger = ctx.Services.GetService<ILoggerFactory>();
        var log = logger.CreateLogger("Root");

        log.LogInformation("Hellooooow from logging");
        Console.WriteLine("Hello World!");


        return 0;
    });

    return builder
        .Build()
        .RunConsoleProgram();
}

```


## License - Apache 2.0