

using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mettle.Xunit.Sdk
{
    public class MettleTestFramework : TestFramework
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MettleTestFramework"/> class.
        /// </summary>
        /// <param name="messageSink">The message sink used to send diagnostic messages</param>
        public MettleTestFramework(IMessageSink messageSink) : base(messageSink) { }

        /// <inheritdoc/>
        protected override ITestFrameworkDiscoverer CreateDiscoverer(IAssemblyInfo assemblyInfo)
        {
            return new XunitTestFrameworkDiscoverer(assemblyInfo, SourceInformationProvider, DiagnosticMessageSink);
        }

        /// <inheritdoc/>
        protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
        {
            return new XunitTestFrameworkExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink);
        }
    }
}