using System.IO;

namespace NerdyMishka
{
    /// <summary>
    /// Noncryptographic hash created by Austin Appleby
    /// </summary>
    /// <remarks>
    ///     <para>
    ///          https://github.com/aappleby/smhasher
    ///     </para>
    /// </remarks>
    public static class MurMurHash3
    {
        private const int DefaultSeed = 79;

        /// <summary>
        /// Computes a 32 bit <see cref="MurMurHash3"/> based on the <paramref name="seed"/> and <paramref name="bytes"/>
        /// </summary>
        /// <param name="bytes">The bytes that will be hashed.</param>
        /// <param name="seed">The default seed.</param>
        /// <returns>The numeric hash value.</returns>
        public static int ComputeHash(byte[] bytes, int seed = -1)
        {
            using (var ms = new MemoryStream(bytes))
            {
                return ComputeHash(ms, seed);
            }
        }

        /// <summary>
        /// Computes a 32 bit <see cref="MurMurHash3"/> based on the bytes in the <paramref name="stream"/> and the
        /// <paramref name="seed"/>.
        /// </summary>
        /// <param name="stream">The stream of bytes that will be hashed.</param>
        /// <param name="seed">The default seed.</param>
        /// <returns>The numeric hash value.</returns>
        public static int ComputeHash(Stream stream, int seed = -1)
        {
            const uint c1 = 0xcc9e2d51;
            const uint c2 = 0x1b873593;

            uint k1 = 0,
                 length = 0,
                 h1 = seed == -1 ? DefaultSeed : (uint)seed;

            using (BinaryReader reader = new BinaryReader(stream))
            {
                byte[] chunk = reader.ReadBytes(4);
                while (chunk.Length > 0)
                {
                    length += (uint)chunk.Length;
                    switch (chunk.Length)
                    {
                        case 4:
                            // convert 4 bytes to uint
                            k1 = (uint)
                               (chunk[0]
                              | chunk[1] << 8
                              | chunk[2] << 16
                              | chunk[3] << 24);


                            k1 *= c1;
                            k1 = BitShift.RotateLeft32(k1, 15);
                            k1 *= c2;
                            h1 ^= k1;

                            h1 = BitShift.RotateLeft32(h1, 13);
                            h1 = h1 * 5 + 0xe6546b64;
                            chunk = reader.ReadBytes(4);
                            continue;

                        case 3:
                            // convert 3 bytes to uint
                            k1 = (uint)
                               (chunk[0]
                              | chunk[1] << 8
                              | chunk[2] << 16);
                            break;

                        case 2:
                            // convert 2 bytes to uint
                            k1 = (uint)
                               (chunk[0]
                              | chunk[1] << 8);
                            break;

                        case 1:
                            // convert single byte to uint
                            k1 = (chunk[0]);
                            break;
                    }


                    k1 *= c1;
                    k1 = BitShift.RotateLeft32(k1, 15);
                    k1 *= c2;
                    h1 ^= k1;
                    chunk = reader.ReadBytes(4);
                }
            }

            // perform avalanche. 
            h1 ^= length;
            h1 = FinalizationMix(h1);

            unchecked
            {
                return (int)h1;
            }
        }


        private static uint FinalizationMix(uint h)
        {
            h ^= h >> 16;
            h *= 0x85ebca6b;
            h ^= h >> 13;
            h *= 0xc2b2ae35;
            h ^= h >> 16;
            return h;
        }
    }
}