
namespace NerdyMishka.Security.Cryptography
{

    public class EncryptedMessage
    {
        public bool Encrypted { get; set; } = false;

        public byte[] MetaData { get; set; }

        public byte[] Message { get; set; }
    }
}