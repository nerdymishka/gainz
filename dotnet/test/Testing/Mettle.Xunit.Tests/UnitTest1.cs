using System;
using Mettle;
using Xunit;

[assembly:ServiceProviderFactory(FactoryType = typeof(Mettle.Xunit.Tests.ServiceBuilder))]

namespace Mettle.Xunit.Tests
{

    public class ServiceBuilder : IServiceProviderFactory
    {
        public IServiceProvider CreateProvider()
        {
            var provider = new SimpleServiceProvider();

            return provider;
        }
    }

    public class ServiceBuilder2: IServiceProviderFactory
    {
        public IServiceProvider CreateProvider()
        {
            var provider = new SimpleServiceProvider();
            provider.AddTransient(typeof(UnitTestData), (s) => { return new UnitTestData(); });

            return provider;
        }
    }

    public class UnitTestData
    {
        public string Name { get; set; } = "test";
    }

    public class UnitTest1
    {
        
        
        [UnitTest]
        public void Test1(IAssert assert)
        {
            assert.Ok("a" == "a");
            Console.Write("Test");
        }

        [UnitTest]
        [ServiceProviderFactory(FactoryType = typeof(ServiceBuilder2))]
        public void Test3(UnitTestData data)
        {
            var assert = AssertImpl.Current;
            assert.NotNull(data);
        }



        [Fact]
        public void Test2()
        {
            Console.Write("Test");
        }
    }
}
