
using System;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mettle.Xunit.Sdk
{

    
    public class MettleTestFrameworkTypeDiscoverer : ITestFrameworkTypeDiscoverer
    {
        /// <inheritdoc/>
        public Type GetTestFrameworkType(IAttributeInfo attribute)
        {
            
            var type = attribute.GetNamedArgument<Type>("Type");
            if(type != null)
                return type;
            
            
            return typeof(MettleTestFramework);
        }
    }

}