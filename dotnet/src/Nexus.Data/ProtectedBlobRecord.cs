using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nexus.Data
{
    [Table("protected_blob", Schema = "nexus")]
    public class ProtectedBlobRecord
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }
        
        [Column("uri_path")]
        public string UriPath { get; set; }

        [Column("blob")]
        public byte[] Blob { get; set; }

        [Column("expires_at")]
        public DateTime? ExpiresAt { get; set; }

        [Column("protected_blob_vault_id")]
        public int ProtectedBlobVaultId  { get; set; }

        [ForeignKey("ProtectedBlobVaultId")]
        public virtual ProtectedBlobVaultRecord Vault  { get; set; }

        [Column("blob_type")]
        public short BlobType { get; set; }

        [Column("tags")]
        public string Tags { get; set; }
    }
}