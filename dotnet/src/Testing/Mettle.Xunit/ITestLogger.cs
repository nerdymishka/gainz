

using Xunit.Abstractions;
using Microsoft.Extensions.Logging;

namespace Mettle
{
    public interface ITestLogger
    {
        ITestOutputHelper Helper { get; }

        ILogger  Log { get; }
    }
}