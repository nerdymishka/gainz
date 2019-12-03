using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.KeePass
{
    public enum KeePassFileHeaderFields : byte 
    {
        EndOfHeader = 0,
        Comment = 1,
        /// <summary>
        /// The UUID of the Cipher for the database
        /// </summary>
        DatabaseCipherId = 2,
        /// <summary>
        /// The database compression type (normal or gzipped)
        /// </summary>
        DatabaseCompression = 3,
        /// <summary>
        /// The seed used to generate the Cipher key.
        /// </summary>
        DatabaseCipherKeySeed = 4,
        /// <summary>
        /// The set used to generate bytes for the Cipher key.
        /// </summary>
        MasterKeyHashSeed = 5,
        /// <summary>
        /// The number of interations for the Encryption engine to execute. 
        /// </summary>
        MasterKeyHashRounds = 6,
        DatabaseCipherIV = 7,
        RandomBytesCryptoKey = 8,
        HeaderByteMark = 9,
        RandomBytesCryptoType = 10
    }
}
