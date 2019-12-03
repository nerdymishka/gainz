

using NerdyMishka;
using Xunit;

namespace Tests 
{
    //[Unit]
    [Trait("tag", "unit")]
    public class ArrayExtensionTests
    {

#pragma warning disable xUnit2013


        [Fact]
        public void Clear()
        {
            var listA = new [] { "alpha", "beta", "gamma", "delta" };
            var listB = new string[4];
            listA.CopyTo(listB, 0);

            Assert.Equal(4, listB.Length);
            Assert.Contains("alpha", listB);
            
            listB.Clear();
            Assert.DoesNotContain("alpha", listB);
            Assert.DoesNotContain("beta", listB);
            Assert.DoesNotContain("gamma", listB);
            Assert.DoesNotContain("delta", listB);

            listA.CopyTo(listB, 0);
            listB.Clear(2, 2);
            Assert.Contains("alpha", listB);
            Assert.Contains("beta", listB);
            Assert.DoesNotContain("gamma", listB);
            Assert.DoesNotContain("delta", listB);
        }

        [Fact]
        public void Grow()
        {
            var list = new string[0];

            Assert.Equal(0, list.Length);
            list = list.Grow(10);

            Assert.Equal(10, list.Length);
        }


        [Fact]
        public void Shrink()
        {
            var list = new string[10];

            Assert.Equal(10, list.Length);
            list = list.Shrink(2);

            Assert.Equal(8, list.Length);
        }
    }

#pragma warning restore xUnit2013
}