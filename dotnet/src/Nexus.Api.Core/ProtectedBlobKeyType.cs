namespace Nexus.Api.Core
{
    public enum ProtectedBlobKeyType : short
    {
        /// <summary>
        /// Default. Encrypt/Decrypt with a key that
        /// has one or more pieces.
        /// </summary>
        Composite = 0,

        /// <summary>
        /// Encrypt/Decrypt with a user supplied key.
        /// </summary>
        User = 1,

        /// <summary>
        /// Encrypt with a public key, service can't decrypt.
        /// </summary>
        Certificate = 2,
    }
}