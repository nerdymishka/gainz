using System;

namespace Nexus.Api
{
    public class ProtectedBlob
    {
        public int? Id { get; set; }

        public string UriPath { get; set; }

        public string Base64Blob  {get; set; }

        public DateTime? ExpiresAt  { get; set; }

        public int? VaultId { get; set; }

        public string VaultName { get; set; }

        public string BlobType { get; set; }

        public string Tags { get; set; }

        public bool Private { get; set; } = false;
    }
}