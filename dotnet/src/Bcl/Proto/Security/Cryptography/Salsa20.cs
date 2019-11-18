using System;
using System.Security.Cryptography;

namespace NerdyMishka.Security.Cryptography
{
    /// <summary>
    /// An implementation of Salsa20, a stream cipher proposed by Daniel J. Bernstein available for
    /// use in the public domain. 
    /// </summary>
    public class Salsa20 : SymmetricAlgorithm
    {
        private static readonly KeySizes[] s_legalBlockSizes;
        private static readonly KeySizes[] s_legalKeySizes;
        private RandomNumberGenerator rng;

        static Salsa20()
        {
            s_legalBlockSizes = new[] { new KeySizes(64, 64, 0) };
            s_legalKeySizes = new[] { new KeySizes(128, 256, 128) };
        }


        /// <summary>
        /// Initializes a new instance of <see cref="Salsa20"/>
        /// </summary>
        protected Salsa20()
        {
#if !NETCOREAPP
            LegalBlockSizesValue = s_legalBlockSizes;
            LegalKeySizesValue = s_legalKeySizes;
#endif
            this.BlockSize = 64;
            this.KeySize = 256;
            this.Rounds = Salsa20Rounds.Twenty;
            this.rng = new RandomNumberGenerator();
            
        }

        /// <summary>
        /// Gets or sets the number of rounds that should be used.
        /// </summary>
        public Salsa20Rounds Rounds { get; set; }

        /// <summary>
        /// Gets or sets whether or skip a XOR operation during the transform block 
        /// </summary>
        public bool SkipXor { get; set; }

        /// <summary>
        /// Gets the block sizes, in bits, that are supported by the symmetric algorithm.
        /// </summary>
        public override KeySizes[] LegalBlockSizes
        {
            get
            {
                return s_legalBlockSizes;
            }
        }

        /// <summary>
        /// Gets the key sizes, in bits, that are supported by the symmetric algorithm.
        /// </summary>
        public override KeySizes[] LegalKeySizes
        {
            get
            {
                return s_legalKeySizes;
            }
        }

#pragma warning disable CS0109
        /// <summary>
        /// Creates a new instance of <see cref="NerdyMishka.Security.Cryptography.Salsa20" />
        /// </summary>
        /// <returns>A new instance of <see cref="Salsa20"/></returns>
        public static new Salsa20 Create()
        {
            return new Salsa20();
        }
#pragma warning restore CS0109

        /// <summary>
        /// Creates a symmetric decryptor object with the <paramref name="rgbKey"/> and initialization vector <paramref name="rgbIV"/>.
        /// </summary>
        /// <param name="rgbKey">The secret key to use for the symmetric algorithm.</param>
        /// <param name="rgbIV">The initializeation vector to use for the symmetric algorithm.</param>
        /// <returns>A symmetric decryptor object.</returns>
        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return new Salsa20CryptoTransform(rgbKey, rgbIV, (int)this.Rounds, this.SkipXor);
        }

        /// <summary>
        /// Creates a symmetric encryptor object with the <paramref name="rgbKey"/> and initialization vector <paramref name="rgbIV"/>.
        /// </summary>
        /// <param name="rgbKey">The secret key to use for the symmetric algorithm.</param>
        /// <param name="rgbIV">The initializeation vector to use for the symmetric algorithm.</param>
        /// <returns>A symmetric encryptor object.</returns>
        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return new Salsa20CryptoTransform(rgbKey, rgbIV, (int)this.Rounds, this.SkipXor);
        }

        public override void GenerateIV()
        {
            this.IV = GetRandomBytes(rng, this.BlockSize / 8);
        }

        public override void GenerateKey()
        {
            this.Key = GetRandomBytes(this.rng, this.KeySize / 8);
        }

        private static byte[] GetRandomBytes(RandomNumberGenerator rng, int byteCount)
        {
            byte[] bytes = new byte[byteCount];
            rng.NextBytes(bytes);
            return bytes;
        }

        private class Salsa20CryptoTransform : ICryptoTransform
        {
            // https://dotnetfiddle.net/Bh4ijW
            private static readonly uint[] Sigma = new uint[] { 0x61707865, 0x3320646E, 0x79622D32, 0x6B206574 };
            private static readonly uint[] Tau = new uint[] { 0x61707865, 0x3120646E, 0x79622D36, 0x6B206574 };
            private uint[] state;
            private uint[] reusableBuffer = new uint[16];
            private int rounds = 10;
            private bool isDisposed = false;
            private bool skipXor = false;
            private int bytesRemaining = 0;
            private byte[] internalBuffer = new byte[64];

            public Salsa20CryptoTransform(byte[] key, byte[] iv, int rounds, bool skipXor)
            {
                this.state = CreateState(key, iv);
                this.rounds = rounds;
                this.skipXor = skipXor;
            }

            public bool CanReuseTransform
            {
                get
                {
                    return false;
                }
            }

            public bool CanTransformMultipleBlocks
            {
                get
                {
                    return true;
                }
            }

            public int InputBlockSize
            {
                get
                {
                    return 64;
                }
            }

            public int OutputBlockSize
            {
                get
                {
                    return 64;
                }
            }



            private static void AddRotateXor(uint[] state, uint[] buffer, byte[] output, int rounds)
            {
                Array.Copy(state, buffer, 16);
                var v = buffer;

                for (var i = 0; i < rounds; i++)
                {
                    v[4] ^= BitShift.RotateLeft32(v[0] + v[12], 7);
                    v[8] ^= BitShift.RotateLeft32(v[4] + v[0], 9);
                    v[12] ^= BitShift.RotateLeft32(v[8] + v[4], 13);
                    v[0] ^= BitShift.RotateLeft32(v[12] + v[8], 18);
                    v[9] ^= BitShift.RotateLeft32(v[5] + v[1], 7);
                    v[13] ^= BitShift.RotateLeft32(v[9] + v[5], 9);
                    v[1] ^= BitShift.RotateLeft32(v[13] + v[9], 13);
                    v[5] ^= BitShift.RotateLeft32(v[1] + v[13], 18);
                    v[14] ^= BitShift.RotateLeft32(v[10] + v[6], 7);
                    v[2] ^= BitShift.RotateLeft32(v[14] + v[10], 9);
                    v[6] ^= BitShift.RotateLeft32(v[2] + v[14], 13);
                    v[10] ^= BitShift.RotateLeft32(v[6] + v[2], 18);
                    v[3] ^= BitShift.RotateLeft32(v[15] + v[11], 7);
                    v[7] ^= BitShift.RotateLeft32(v[3] + v[15], 9);
                    v[11] ^= BitShift.RotateLeft32(v[7] + v[3], 13);
                    v[15] ^= BitShift.RotateLeft32(v[11] + v[7], 18);
                    v[1] ^= BitShift.RotateLeft32(v[0] + v[3], 7);
                    v[2] ^= BitShift.RotateLeft32(v[1] + v[0], 9);
                    v[3] ^= BitShift.RotateLeft32(v[2] + v[1], 13);
                    v[0] ^= BitShift.RotateLeft32(v[3] + v[2], 18);
                    v[6] ^= BitShift.RotateLeft32(v[5] + v[4], 7);
                    v[7] ^= BitShift.RotateLeft32(v[6] + v[5], 9);
                    v[4] ^= BitShift.RotateLeft32(v[7] + v[6], 13);
                    v[5] ^= BitShift.RotateLeft32(v[4] + v[7], 18);
                    v[11] ^= BitShift.RotateLeft32(v[10] + v[9], 7);
                    v[8] ^= BitShift.RotateLeft32(v[11] + v[10], 9);
                    v[9] ^= BitShift.RotateLeft32(v[8] + v[11], 13);
                    v[10] ^= BitShift.RotateLeft32(v[9] + v[8], 18);
                    v[12] ^= BitShift.RotateLeft32(v[15] + v[14], 7);
                    v[13] ^= BitShift.RotateLeft32(v[12] + v[15], 9);
                    v[14] ^= BitShift.RotateLeft32(v[13] + v[12], 13);
                    v[15] ^= BitShift.RotateLeft32(v[14] + v[13], 18);
                }

                for (int i = 0; i < 16; ++i)
                {
                    v[i] += state[i];
                    output[i << 2] = (byte)v[i];
                    output[(i << 2) + 1] = (byte)(v[i] >> 8);
                    output[(i << 2) + 2] = (byte)(v[i] >> 16);
                    output[(i << 2) + 3] = (byte)(v[i] >> 24);
                }

                state[8]++;
                if (state[8] == 0)
                    state[9]++;
            }

            private static uint[] CreateState(byte[] key, byte[] iv)
            {
                int offset = key.Length - 16;
                uint[] expand = key.Length == 32 ? Sigma : Tau;
                uint[] state = new uint[16];

                // key
                state[1] = key.ToUInt32(0);
                state[2] = key.ToUInt32(4);
                state[3] = key.ToUInt32(8);
                state[4] = key.ToUInt32(12);

                // key offset
                state[11] = key.ToUInt32(offset + 0);
                state[12] = key.ToUInt32(offset + 4);
                state[13] = key.ToUInt32(offset + 8);
                state[14] = key.ToUInt32(offset + 12);

                // sigma / tau 
                state[0] = expand[0];
                state[5] = expand[1];
                state[10] = expand[2];
                state[15] = expand[3];

                state[6] = iv.ToUInt32(0);
                state[7] = iv.ToUInt32(4);
                state[8] = 0;
                state[9] = 0;

                return state;
            }

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected void Dispose(bool isDisposing)
            {
                if (isDisposing)
                {
                    Array.Clear(this.reusableBuffer, 0, this.reusableBuffer.Length);
                    Array.Clear(this.state, 0, this.state.Length);
                    this.reusableBuffer = null;
                    this.state = null;
                    this.isDisposed = true;
                }
            }

            ~Salsa20CryptoTransform()
            {
                this.Dispose(false);
            }

            public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
            {
                this.CheckDisposed();

                int bytesTransformed = 0;
                int internalOffset = 0;
                if (this.bytesRemaining > 0)
                    internalOffset = 64 - this.bytesRemaining;


                while (inputCount > 0)
                {
                    if (this.bytesRemaining == 0)
                    {
                        AddRotateXor(this.state, this.reusableBuffer, this.internalBuffer, this.rounds);
                        bytesRemaining = 64;
                        internalOffset = 0;
                    }

                    var length = Math.Min(bytesRemaining, inputCount);


                    if (this.skipXor)
                    {
                        Array.Copy(this.internalBuffer, internalOffset, outputBuffer, outputOffset, length);
                    }
                    else
                    {
                        for (int i = 0; i < length; i++)
                            outputBuffer[outputOffset + i] = (byte)(inputBuffer[inputOffset + i] ^ this.internalBuffer[i]);
                    }

                    this.bytesRemaining -= length;
                    bytesTransformed += length;
                    inputCount -= length;
                    outputOffset += length;
                    inputOffset += length;
                }

                return bytesTransformed;
            }

            public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
            {
                this.CheckDisposed();

                byte[] output = new byte[inputCount];
                TransformBlock(inputBuffer, inputOffset, inputCount, output, 0);
                return output;
            }

            private void CheckDisposed()
            {
                if (this.isDisposed)
                    throw new ObjectDisposedException("ICryptoTransform is already disposed");
            }
        }
    }
}
