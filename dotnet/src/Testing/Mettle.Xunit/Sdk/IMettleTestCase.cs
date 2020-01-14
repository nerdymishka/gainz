

using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mettle.Xunit.Sdk
{
    public interface IMettleTestCase : IXunitTestCase
    {
        Task<RunSummary> RunAsync(
            IMessageSink diagnosticMessageSink, 
            IMessageBus messageBus, 
            object[] constructorArguments, 
            ExceptionAggregator aggregator, 
            CancellationTokenSource cancellationTokenSource,
            IServiceProvider serviceProvider);
    }
}