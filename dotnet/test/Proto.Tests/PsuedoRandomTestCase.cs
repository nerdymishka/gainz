using Xunit;
using System;
using NerdyMishka;

namespace Tests 
{

    public abstract class PsuedoRandomTestCase
    {
        protected abstract IRandom CreateRng(int? seed = null);

        [Fact]
        public void ConstructorDoesNotThrow()
        {
            var random = new System.Random();
            for(var i = 0; i < 100; i++)
            {
                var seed = random.Next(-10000, 10000);
                try {
                    
                    var rng = CreateRng(seed);
                } catch(Exception ex) {
                    Assert.True(ex == null, $"Constructor threw exception for seed {seed} and constructor should not throw exception: " + ex.ToString());
                }
            }
        } 

        [Fact]
        public void SetSeed()
        {
            var rSeed10 = CreateRng(10);
            var r1 = CreateRng(1);
            var r2 = CreateRng(20);

            Assert.NotEqual(r1.NextInt32(), r2.NextInt32());
            Assert.NotEqual(r1.NextInt64(), r2.NextInt64());
            Assert.NotEqual(r1.NextFloat(), r2.NextFloat());
            Assert.NotEqual(r1.NextDouble(), r2.NextDouble());

            r1 = CreateRng(1);
            r2.SetSeed(1);
            
            Assert.Equal(r1.NextInt32(), r2.NextInt32());
            Assert.Equal(r1.NextInt64(), r2.NextInt64());
            Assert.Equal(r1.NextFloat(), r2.NextFloat());
            Assert.Equal(r1.NextDouble(), r2.NextDouble());
        }



        [Fact]
        public void DifferentSeeds_DifferentValues()
        {
            var r1 = CreateRng(0);
            var r2 = CreateRng(1);

            
            Assert.NotEqual(r1.NextInt32(), r2.NextInt32());
            Assert.NotEqual(r1.NextInt64(), r2.NextInt64());
            Assert.NotEqual(r1.NextFloat(), r2.NextFloat());
            Assert.NotEqual(r1.NextDouble(), r2.NextDouble());
        }

        [Fact]
        public void SameSeed_SameValues()
        {
            var r1 = CreateRng(1);
            var r2 = CreateRng(1);

            Assert.Equal(r1.NextInt32(), r2.NextInt32());
            Assert.Equal(r1.NextInt64(), r2.NextInt64());
            Assert.Equal(r1.NextFloat(), r2.NextFloat());
            Assert.Equal(r1.NextDouble(), r2.NextDouble());
            Assert.Equal(r1.NextBoolean(), r2.NextBoolean());
        }

        [Fact]
        public virtual void Int32_Min_Max()
        {
            var r1 = CreateRng(1);

            for(var i =0; i < 1000; i++)
            {
                var actual = r1.NextInt32(2000);
                Assert.True(actual >= 0 && actual <= 2000);
            }

            for(var i =0; i < 1000; i++)
            {
                var actual = r1.NextInt32(100, 2000);
                Assert.True(actual >= 100 && actual <= 2000);
            }
        }

        [Fact]
        public virtual void Int64_Min_Max()
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

            for(var i =0; i < 1000; i++)
            {
                var actual = r1.NextInt64(-max, -min);
                Assert.True(actual >= (-max) && actual <= (-min), $"{actual} is either lower than -{max} or greather than -{min}");
            }
        }
    }
}