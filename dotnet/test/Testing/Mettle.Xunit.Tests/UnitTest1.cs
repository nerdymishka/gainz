using System;
using Mettle;
using Xunit;

namespace Mettle.Xunit.Tests
{
    public class UnitTest1
    {
        
        [UnitTest]
        public void Test1()
        {
            Console.Write("Test");
        }

        [Fact]
        public void Test2()
        {
            Console.Write("Test");
        }
    }
}
