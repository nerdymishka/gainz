
using System;

namespace Mettle
{
    public interface IServiceProviderFactory 
    {
        IServiceProvider CreateProvider();
    }
}