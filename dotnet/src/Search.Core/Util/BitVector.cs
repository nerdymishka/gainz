
namespace NerdyMishka.Search
{
    /// <summary>
    /// Optimized implementation of a vector of bits.  
    /// </summary>
    /// <remarks>
    /// This is more-or-less like java.util.BitSet. Additional features:
    ///         
    /// - a size propertie which effiently computes the number of bits set to one.
    /// - inlineable get method
    /// - ability to override methods to read/write to disk. 
    /// </summary>
    public class BitVector
    {
        private static readonly byte[] s_byteCounts = {	  // table of bits/byte
            0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4,
            1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
            1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
            2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
            1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
            2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
            2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
            3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
            1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
            2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
            2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
            3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
            2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
            3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
            3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
            4, 5, 5, 6, 5, 6, 6, 7, 5, 6, 6, 7, 6, 7, 7, 8
        };

        private byte[] bits;
        
        private int size = -1;

        /// <summary>
        /// Initializes a new instance of <see cref="BitVector" />.
        /// </summary>
        internal protected BitVector()
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="BitVector" />.
        /// </summary>
        /// <param name="length">The number of bits the vector will hold.</param>
        public BitVector(int length) 
        {
            this.Initialize(length);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="BitVector" />.
        /// </summary>
        /// <param name="length">The number of bits the vector will hold.</param>
        public BitVector(int length, int numberOfBytes, byte[] bits) 
        {
            this.Length = length;
            this.bits = bits;
            this.size = numberOfBytes;
        }

        /// <summary>
        /// The number of bits in the vector
        /// </summary>
        /// <value></value>
        public int Length { get; private set; }

        /// <summary>
        /// The size of bits per byte.
        /// </summary>
        /// <value></value>
        public int Size
        { 
            get 
            {
                if (this.size == -1) {
                    int c = 0,
                        end = this.bits.Length;
                    
                    for (int i = 0; i < end; i++) {
                        c += s_byteCounts[this.bits[i] & 0xFF];	  // sum bits per byte 
                    }
                    this.size = c;
                }
                return this.size;
            }
        }

        /// <summary>
        /// Initializes the <see cref="BitVector" />
        /// </summary>
        /// <param name="length">The length of the bit vector.</param>
        internal protected void Initialize(int length)
        {
            this.Length = length;
            this.bits = new byte[(this.Length >> 3) + 1];
        }



        /// <summary>
        /// Sets the bit to one.
        /// </summary>
        /// <param name="bit">The position of the bit.</param>
        public void Set(int bit) 
        {
            this.bits[bit >> 3] |= (byte)(1 << (bit & 7));
            this.size = -1;
        }

        /// <summary>
        /// Clears the bit to zero.
        /// </summary>
        /// <param name="bit">The bit.</param>
        public void Clear(int bit) 
        {
            this.bits[bit >> 3] &= (byte)(~(1 << (bit & 7)));
            this.size = -1;
        }

        /// <summary>
        /// Returns <c>true</c> if <paramref name="bit" /> is one and
        /// <c>false</c> if zero.
        /// </summary>
        /// <param name="bit">The bit.</param>
        /// <returns>True if the byte is one.</returns>
        public bool Get(int bit) 
        {
            return (this.bits[bit >> 3] & (1 << (bit & 7))) != 0;
        }
    }
}