

using System;
using NerdyMishka;
using Xunit;

namespace Tests
{
    //[Unit]
    [Trait("tag", "unit")]
    public class JavaRandomTests : PsuedoRandomTestCase
    {
        protected override IRandom CreateRng(int? seed = null)
        {
            if(seed == null)
                return new JavaRandom();

            return new JavaRandom(seed.Value);
        }

        [Fact]
        public override void Int64_Min_Max()
        {

            var r1 = CreateRng();
            long min = Int32.MaxValue;
            long max = ((long)Int32.MaxValue) + 2000;
            for(var i =0; i < 1000; i++)
            {
                var actual = r1.NextInt64(max);
                Assert.True(actual >= 0 && actual <= max);
            }

            for(var i =0; i < 1000; i++)
            {
                var actual = r1.NextInt64(min, max);
                Assert.True(actual >= min && actual <= max, $"{actual} is either lower than {min} or greather than {max}");
            }

            // java version does not accept negative min/max values
       
        }
    }
}