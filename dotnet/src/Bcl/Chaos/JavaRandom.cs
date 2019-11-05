/**
 * Copyright 2016 Nerdy Mishka
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;

namespace NerdyMishka
{
    /// <summary>
    /// A psuedo repeatable random number generator based on the Java implementation
    /// which in turn is based the works of Donald E. Knuth.  
    /// </summary>
    /// <remarks>
    ///     <para>
    /// https://android.googlesource.com/platform/libcore/+/184d077ac4e1e008b8ef8a4cf0af60c4d87cfccb/luni/src/main/java/java/util/Random.java    
    /// </para>
    /// </remarks>
    public class JavaRandom : IRandom
    {
        private const long SerialVersionId = 3905348978240129619;
        private const long Multiplyer = 0x5deece66d;
        private bool hasNextNextGaussian;
        private long seed;
        private double nextNextGuassian;
        private static long seedBase = 0;

        /// <summary>
        /// Initializes a new instance of <see cref="JavaRandom"/>
        /// </summary>
        public JavaRandom()
        {
            this.SetSeed(DateTime.Now.Millisecond + seedBase);
            System.Threading.Interlocked.Increment(ref seedBase);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JavaRandom"/> with the given <paramref name="seed"/>.
        /// </summary>
        /// <param name="seed">A number used to calculate a starting value for the pseudo-random number sequence. </param>
        public JavaRandom(int seed)
        {
            this.SetSeed(seed);
            System.Threading.Interlocked.Increment(ref seedBase);
        }

        private int Next(int bits)
        {
            this.seed = (this.seed * Multiplyer + 0xbL) & ((1L << 48) - 1);
            return (int)( ((uint)this.seed) >> (48 - bits));
        }

        /// <summary>
        /// Generates a random <see cref="Boolean"/> value.
        /// </summary>
        /// <returns>A random <see cref="Boolean"/> value.</returns>
        public bool NextBoolean()
        {
            return this.Next(1) != 0;
        }

        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers.
        /// </summary>
        /// <param name="bytes">An array of bytes to contain random numbers.</param>
        void IRandomBytesGenerator.NextBytes(byte[] bytes)
        {
            this.NextBytes(bytes);
        }

        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers.
        /// </summary>
        /// <param name="count">The number of bytes that should be randomly generated.</param>
        /// <returns>The array of random bytes</returns>
        public byte[] NextBytes(int count)
        {
            var bytes = new byte[count];
            this.NextBytes(bytes);
            return bytes;
        }

        /// <summary>
        /// Generates random bytes for the buffer.
        /// </summary>
        /// <param name="buffer">The buffer that will be filled.</param>
        /// <param name="offset">The index of the buffer to start generating bytes. Defaults to zero.</param>
        /// <param name="length">The number of bytes that should be generated. Defaults to the length of the buffer.</param>
        public void NextBytes(byte[] buffer, int offset = 0, int? length = default(int?))
        {
            int random = 0,
                count = 0,
                loop = 0;

            while(count < buffer.Length)
            {
                if(loop == 0)
                {
                    random = this.NextInt32();
                    loop = 3;
                } else
                {
                    loop--;
                }

                buffer[count++] = (byte)random;
                random >>= 8;
            }
        }

        /// <summary>
        /// Generates a random <see cref="Double"/> value.
        /// </summary>
        /// <returns>A random <see cref="Double"/> value.</returns>
        public double NextDouble()
        {
            return (((long)this.Next(26) << 27) + this.Next(27)) / (double)(1L << 53);
        }


        /// <summary>
        /// Generates a random <see cref="float"/> value.
        /// </summary>
        /// <returns>A random <see cref="float"/> value. </returns>
        public float NextFloat()
        {
            return (this.Next(24) / 16777216f);
        }

        private double NextGaussian()
        {
            if(this.hasNextNextGaussian)
            {
                this.hasNextNextGaussian = false;
                return this.nextNextGuassian;
            }

            double v1, v2, s;
            do
            {
                v1 = 2 * this.NextDouble() - 1;
                v2 = 2 * this.NextDouble() - 1;
                s = v1 * v1 + v2 * v2;
            } while (s >= 1 || s == 0);

            double multiplier = Math.Sqrt(-2 * Math.Log(s) / s);
            this.nextNextGuassian = v2 * multiplier;
            this.hasNextNextGaussian = true;

            return v1 * multiplier;
        }

        /// <summary>
        /// Generates a random <see cref="System.Int32"/> between zero and a <paramref name="max"/> value. 
        /// </summary>
        /// <param name="max">The maximum value for the randomly generated value. Defaults to <see cref="System.Int32.MaxValue"/></param>
        /// <returns>A random <see cref="Int32"/> value.</returns>
        public int NextInt32(int max = int.MaxValue)
        {
            return this.NextInt32(0, max);
        }

        /// <summary>
        /// Generates a random <see cref="Int32"/> between the <paramref name="min"/> and <paramref name="max"/> value.
        /// </summary>
        /// <param name="min">The maximum value for the randomly generated value.</param>
        /// <param name="max">The minimum value for the randomly generated value.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <list type="">
        ///         <item>Thrown when <paramref name="max"/> is less than zero.</item>
        ///         <item>Thrown when <paramref name="min"/> is less than zero.</item>
        ///         <item>Thrown when <paramref name="min"/> is greater than <paramref name="max"/>.</item> 
        ///     </list>
        /// </exception>
        /// <returns>A random <see cref="Int32"/> value. </returns>
        public int NextInt32(int min, int max)
        {
            if (max <= 0)
                throw new ArgumentOutOfRangeException(nameof(max), max, $"{nameof(max)} must be 0 or greater");


            if (min < 0)
                throw new ArgumentOutOfRangeException(nameof(min), min, $"{nameof(min)} must be 0 or greater");

            if (min > max)
                throw new ArgumentOutOfRangeException(nameof(min), min, $"{nameof(min)} must be less than {nameof(max)} {max}");


            if (min == 0 && max == int.MaxValue)
                return this.Next(32);

            if ((max & -max) == max)
            {
                return (int)((max * (long)this.Next(31)) >> 31);
            }
            int bits, val;
            do
            {
                bits = this.Next(31);
                val = bits % max;
            } while (val > (max - 1) || val < min);

            return val;
        }

        /// <summary>
        /// Generates a random <see cref="System.Int64"/> between zero and a <paramref name="max"/> value. 
        /// </summary>
        /// <param name="max">The maximum value for the randomly generated value. Defaults to <see cref="System.Int64.MaxValue"/></param>
        /// <returns>A random <see cref="Int64"/> value.</returns>
        public long NextInt64(long max = long.MaxValue)
        {
            return this.NextInt64(0, max);
        }

        /// <summary>
        /// Generates a random <see cref="Int64"/> between the <paramref name="min"/> and <paramref name="max"/> value.
        /// </summary>
        /// <param name="min">The maximum value for the randomly generated value.</param>
        /// <param name="max">The minimum value for the randomly generated value.</param>
        /// <returns>A random <see cref="Int64"/> value. </returns>
        public long NextInt64(long min, long max)
        {
            if (max <= 0)
                throw new ArgumentOutOfRangeException(nameof(max), max, $"{nameof(max)} must be 0 or greater");


            if (min < 0)
                throw new ArgumentOutOfRangeException(nameof(min), min, $"{nameof(min)} must be 0 or greater");

            if (min > max)
                throw new ArgumentOutOfRangeException(nameof(min), min, $"{nameof(min)} must be less than {nameof(max)} {max}");


            if (min == 0 && max == int.MaxValue)
                return ((long)this.Next(32) << 32) + this.Next(32);

            if ((max & -max) == max)
            {
                return 0L;
            }
            long bits, val;
            do
            {
                bits = ((long)this.Next(32) << 32) + this.Next(32);
                val = bits % max;
            } while (val > max || val < min);

            return val;
        }

        /// <summary>
        /// Sets the <paramref name="seed"/> value for the <see cref="IRandom"/> instance.
        /// </summary>
        /// <param name="seed">A number used to calculate a starting value for the pseudo-random number sequence. </param>
        public void SetSeed(int seed)
        {
            this.seed = (seed ^ Multiplyer) & ((1L << 48) - 1);
            this.hasNextNextGaussian = false;
        }

        /// <summary>
        /// Sets the <paramref name="seed"/> value for the <see cref="IRandom"/> instance.
        /// </summary>
        /// <param name="seed">A number used to calculate a starting value for the pseudo-random number sequence. </param>
        public void SetSeed(long seed)
        {
            this.seed = (seed ^ Multiplyer) & ((1L << 48) - 1);
            this.hasNextNextGaussian = false;
        }
    }
}
