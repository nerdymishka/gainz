using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka
{
    public class DotNetRandom : IRandom
    {
        private Random random;
        
        public DotNetRandom()
        {
            this.random = new Random();
        }

        public DotNetRandom(int seed)
        {
            this.random = new Random(seed);
        }

        public bool NextBoolean()
        {
            return this.random.Next(1) != 0;
        }

        void IRandomBytesGenerator.NextBytes(byte[] bytes)
        {
            this.NextBytes(bytes);
        }

        public byte[] NextBytes(int count)
        {
            var bytes = new byte[count];
            this.NextBytes(bytes);
            return bytes;
        }

        public void NextBytes(byte[] buffer, int offset = 0, int? length = default(int?))
        {
            if (offset < 0 || offset > buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            if (length != null && length.Value < 0 || length > buffer.Length || (length.Value - offset) > buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (offset == 0 && length == null)
            {
                this.random.NextBytes(buffer);
                return;
            }

            if (length == null)
                length = buffer.Length - offset;

            var buffer2 = new byte[(int)length - offset];
            this.random.NextBytes(buffer2);

            Array.Copy(buffer2, 0, buffer, offset, length.Value);
            Array.Clear(buffer2, 0, buffer2.Length);
        }

        public double NextDouble()
        {
            return this.random.NextDouble();
        }

        public float NextFloat()
        {
            return (float)this.random.NextDouble();
        }

        public int NextInt32(int max = int.MaxValue)
        {
            return this.random.Next(max);
        }

        public int NextInt32(int min, int max)
        {
            return this.random.Next(min, max);
        }

        public long NextInt64(long max = long.MaxValue)
        { 
            return (long)Math.Floor(this.NextDouble() * max);
        }

        public long NextInt64(long min, long max)
        {
            if(min > max)
                throw new ArgumentOutOfRangeException(nameof(min));

            var num = max - min;
         
            var v = (long)Math.Floor((this.NextDouble() * num));
            if(max < 0)
            {
                v = -v;

                if(v > max)
                    v += max;

                if(v < min)
                    v += num;

                return v;
            }
                
            if(v < min)
                v += min;
                
            if(v > max)
                v -= num;       

            return v;
        }

        public void SetSeed(int seed)
        {
            this.random = new Random(seed);
        }

        public void SetSeed(long seed)
        {
            seed = seed << 32;
            if (seed < 0)
                seed = seed - seed - seed;

            this.random = new Random((int)seed);
        }
    }
}
