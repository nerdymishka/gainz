using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace NerdyMishka.Security.Cryptography
{
    /// <summary>
    /// A stream cipher drop in replacement for RC4 developed by RL Rivest.  
    /// </summary>
    /// <remarks>
    ///     <para>
    ///     You can find the paper for Spritz from a mit server. 
    ///     https://people.csail.mit.edu/rivest/pubs/RS14.pdf
    ///     </para>
    /// </remarks>
    public class Spritz : SymmetricAlgorithm
    {
        private static readonly KeySizes[] s_legalBlockSizes;
        private static readonly KeySizes[] s_legalKeySizes;
        private RandomNumberGenerator rng;

        static Spritz()
        {
            s_legalBlockSizes = new[] { new KeySizes(8, 256, 8) };
            s_legalKeySizes = new[] { new KeySizes(8, 128, 8) };
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Spritz"/>
        /// </summary>
        protected Spritz()
        {
#if !NETCOREAPP
            LegalBlockSizesValue = s_legalBlockSizes;
            LegalKeySizesValue = s_legalKeySizes;
#endif
            this.BlockSize = 256;
            this.KeySize = 128;
            this.rng = RandomNumberGenerator.Create();
        }

        /// <summary>
        /// BlockSizes can be 8 to 256 at increments of 8
        /// </summary>
        public override KeySizes[] LegalBlockSizes
        {
            get
            {
                return s_legalBlockSizes;
            }
        }

        /// <summary>
        /// KeySizes can be 8 to 128 at increments of 8
        /// </summary>
        public override KeySizes[] LegalKeySizes
        {
            get
            {
                return s_legalKeySizes;
            }
        }

        /// <summary>
        /// Creates a symmetric decryptor object with the <paramref name="rgbKey"/> and initialization vector <paramref name="rgbIV"/>.
        /// </summary>
        /// <param name="rgbKey">The secret key to use for the symmetric algorithm.</param>
        /// <param name="rgbIV">The initializeation vector to use for the symmetric algorithm.</param>
        /// <returns>A symmetric decryptor object.</returns>
        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return new SpritzCryptoTransform(this.BlockSize, rgbKey, rgbIV, true);
        }

        /// <summary>
        /// Creates a symmetric encryptor object with the <paramref name="rgbKey"/> and initialization vector <paramref name="rgbIV"/>.
        /// </summary>
        /// <param name="rgbKey">The secret key to use for the symmetric algorithm.</param>
        /// <param name="rgbIV">The initializeation vector to use for the symmetric algorithm.</param>
        /// <returns>A symmetric encryptor object.</returns>
        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return new SpritzCryptoTransform(this.BlockSize, rgbKey, rgbIV, false);
        }

        /// <summary>
        /// Generates a value for the initalization vector property <see cref="System.Security.Cryptography.SymmetricAlgorithm.IV"/>
        /// </summary>
        public override void GenerateIV()
        {
            this.IV = GetRandomBytes(rng, this.BlockSize / 8);
        }

        /// <summary>
        /// Generates a value for the secret key property <see cref="System.Security.Cryptography.SymmetricAlgorithm.Key"/>
        /// </summary>
        public override void GenerateKey()
        {
            this.Key = GetRandomBytes(this.rng, this.KeySize / 8);
        }

#pragma warning disable CS0109
        /// <summary>
        /// Creates a new instance of <see cref="NerdyMishka.Security.Cryptography.Spritz" />
        /// </summary>
        /// <returns>A new instance of <see cref="Spritz"/></returns>
        public static new Spritz Create()
        {
            return new Spritz();
        }
#pragma warning restore  CS0109

        private static byte[] GetRandomBytes(RandomNumberGenerator rng, int byteCount)
        {
            byte[] bytes = new byte[byteCount];
            rng.GetBytes(bytes);
            return bytes;
        }

        private class SpritzCryptoTransform : ICryptoTransform
        {
            private int[] s;
            private int n, a, i, j, k, w, z;
            private int halfN;
            private int doubleN;
            private int oneMinusN;
            private readonly bool encrypt;
            private bool isDisposed;

            public SpritzCryptoTransform(int blockSize, byte[] key, byte[] iv, bool encrypt)
            {
                this.encrypt = encrypt;
                this.Setup(blockSize, key, iv);
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
                    return this.n;
                }
            }

            public int OutputBlockSize
            {
                get
                {
                    return this.n;
                }
            }

            private void Setup(int blockSize, byte[] key, byte[] iv)
            {
                this.InitializeState(blockSize);
                this.SetupKey(key);
                this.AbsorbStop();
                this.SetupIV(iv);
            }

            private void InitializeState(int blockSize)
            {
                this.a = this.i = this.j = this.k = this.z = 0;
                this.w = 1;
                this.n = blockSize;
                this.halfN = n / 2;
                this.oneMinusN = n - 1;
                this.doubleN = n * 2;
                this.s = new int[this.n];
                for (int v = 0; v < this.n; v++)
                {
                    this.s[v] = v;
                }
            }

            private void SetupKey(byte[] key)
            {
                this.Absorb(key, 0, key.Length);
            }

            private void SetupIV(byte[] iv)
            {
                this.Absorb(iv, 0, iv.Length);
            }

            // Normally called Crush
            private void NaeNae()
            {
                for (int v = 0; v < this.halfN; v++)
                {
                    int y = this.oneMinusN - v;
                    int x1 = this.s[v];
                    int x2 = this.s[y];
                    if (x1 > x2)
                    {
                        this.s[v] = x2;
                        this.s[y] = x1;
                    }
                    else {
                        this.s[v] = x1;
                        this.s[y] = x2;
                    }
                }
            }
            private void Shuffle()
            {
                // couldn't resist pop culture humor. 
                this.Whip();
                this.NaeNae();
                this.Whip();
                this.NaeNae();
                this.Whip();
                this.a = 0;
            }

            private void Whip()
            {
                for (int v = 0; v < this.doubleN; v++)
                {
                    this.Update();
                }
                this.w += 2;
            }

            private void Update()
            {
                this.i = (this.i + this.w) % this.n;
                int y = (this.j + this.s[this.i]) % this.n;
                this.j = (this.k + this.s[y]) % this.n;
                this.k = (this.i + this.k + this.s[this.j]) % this.n;

                // copy and swap. 
                int t = this.s[this.i & 0xff];
                this.s[this.i] = this.s[this.j];
                this.s[this.j] = t;
            }


            private void AbsorbNibble(int x)
            {
                if (this.a == this.halfN)
                {
                    this.Shuffle();
                }
                int y = (this.halfN + x) % this.n;
                int t = this.s[this.a];
                this.s[this.a] = this.s[y];
                this.s[y] = t;
                this.a++;
            }

            private int Output()
            {
                int y1 = (this.z + this.k) % this.n;
                int x1 = (this.i + this.s[y1]) % this.n;
                int y2 = (this.j + this.s[x1]) % this.n;
                this.z = this.s[y2];
                return this.z;
            }

            private void AbsorbStop()
            {
                if (this.a == this.halfN)
                {
                    this.Shuffle();
                }
                this.a++;
            }

            private void AbsorbByte(int b)
            {
                AbsorbNibble(b & 0x0F);        // low bits
                AbsorbNibble((b & 0xFF) >> 4); // high bits
            }

            private void Absorb(byte[] buffer, int offset, int length)
            {
                for (int end = offset + length; offset < end; offset++)
                {
                    AbsorbByte(buffer[offset] & 7);
                }
            }

            private int Drip()
            {
                if (this.a > 0)
                {
                    this.Shuffle();
                }
                this.Update();
                return this.Output();
            }



            private void Squeeze(byte[] outputBuffer, int offset, int length)
            {
                if (this.a > 0)
                {
                    this.Shuffle();
                }
                for (int end = offset + length; offset < end; offset++)
                {
                    outputBuffer[offset] = (byte)this.Drip();
                }
            }      

           
            public void Dispose()
            {
                if(!this.isDisposed)
                {
                    Array.Clear(this.s, 0, this.s.Length);
                    this.a = this.i = this.j = this.k = this.w = this.z = 0;

                    GC.SuppressFinalize(this);
                    this.isDisposed = true;
                }
            }

            public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
            {
                int i = 0;
                if (this.encrypt)
                {

                    for (; i < inputCount; i++)
                    {
                        outputBuffer[outputOffset + i] = (byte)(inputBuffer[inputOffset + i] + this.Drip());
                    }
                } else
                {
                    for (; i < inputCount; i++)
                    {
                        outputBuffer[outputOffset + i] = (byte)(inputBuffer[inputOffset + i] - this.Drip());
                    }
                }
              
                return i;
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
