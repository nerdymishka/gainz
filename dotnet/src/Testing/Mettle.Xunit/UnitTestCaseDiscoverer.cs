
using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;
using System.Linq;

namespace Mettle
{
    public sealed class UnitTestDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly IMessageSink _messageSink;

        public UnitTestDiscoverer(IMessageSink messageSink)
        {
            _messageSink = messageSink;
        }

        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {    
            var parameters = testMethod.Method.GetParameters();
            var parameterCount = parameters.Count();
            var parameterValues = new object[parameterCount];

           
            if(parameterCount > 0)
            {
                var provider = SimpleServiceProvider.Current;
                int i =0;
                
                foreach(var parameter in parameters)
                {
                    var type = parameter.ParameterType.ToRuntimeType();
                    var parameterValue = provider.GetService(parameter.ParameterType.ToRuntimeType());
                    parameterValues[i] = parameterValue;
                    i++;
                }
            }
        

            yield return new UnitTestCase(
                _messageSink,
                TestMethodDisplay.ClassAndMethod, TestMethodDisplayOptions.All,
                testMethod, 
                parameterValues);
            
        }
    }
}
