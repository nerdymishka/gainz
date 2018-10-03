using System.Collections.ObjectModel;

namespace Nexus.Api
{
    public class ProtectedBlobVault
    {
        public ProtectedBlobVault()
        {
            this.Blobs = new Collection<ProtectedBlob>();
        }

        public int? Id { get; set; }

        public string Name { get; set; }

        public byte[] Entropy { get; set; }

        public string KeyType { get; set; }

        public int? UserId { get; set; }

        public string Username { get; set; }

        public int? OperationalEnvironmentId  { get; set; }

        public string OperationalEnvironmentName { get; set; }

        public int? PublicKeyId { get; set; }

        public string PublicKeyUriPath { get; set; }

        public Collection<ProtectedBlob> Blobs { get; set; }
    }
}