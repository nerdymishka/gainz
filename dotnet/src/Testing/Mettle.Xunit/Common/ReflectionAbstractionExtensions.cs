
using System;
using System.Threading;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mettle.Xunit.Sdk
{
    public static class ReflectionAbstractionExtensions
    {
        
        
        /// <summary>
        /// Creates an instance of the test class for the given test case. Sends the <see cref="ITestClassConstructionStarting"/>
        /// and <see cref="ITestClassConstructionFinished"/> messages as appropriate.
        /// </summary>
        /// <param name="test">The test</param>
        /// <param name="testClassType">The type of the test class</param>
        /// <param name="constructorArguments">The constructor arguments for the test class</param>
        /// <param name="messageBus">The message bus used to send the test messages</param>
        /// <param name="timer">The timer used to measure the time taken for construction</param>
        /// <param name="cancellationTokenSource">The cancellation token source</param>
        /// <returns></returns>
        public static object CreateTestClassWithServices(this ITest test,
                                            Type testClassType,
                                            object[] constructorArguments,
                                            IMessageBus messageBus,
                                            ExecutionTimer timer,
                                            CancellationTokenSource cancellationTokenSource)
        {
            object testClass = null;

            if (!messageBus.QueueMessage(new TestClassConstructionStarting(test)))
                cancellationTokenSource.Cancel();
            else
            {
                try
                {
                    if (!cancellationTokenSource.IsCancellationRequested)
                        timer.Aggregate(() => {
                            

                            testClass = Activator.CreateInstance(testClassType, constructorArguments);
                        });
                }
                finally
                {
                    if (!messageBus.QueueMessage(new TestClassConstructionFinished(test)))
                        cancellationTokenSource.Cancel();
                }
            }

            return testClass;
        }
    }
}