using System;

namespace NerdyMishka
{
    /// <summary>
    /// A random data generator targeted towards generating cryptographically 
    /// secure <see cref="System.Byte[]"/>.
    /// </summary>
    public interface IRandomBytesGenerator
    {
        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers.
        /// </summary>
        /// <param name="count">The number of bytes that should be randomly generated.</param>
        /// <returns>The array of random bytes</returns>
        byte[] NextBytes(int count);

        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers.
        /// </summary>
        /// <param name="bytes">An array of bytes to contain random numbers.</param>
        void NextBytes(byte[] bytes);
    }
}