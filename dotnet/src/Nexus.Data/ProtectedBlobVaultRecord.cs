using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nexus.Data
{
    [Table("protected_blob_vaults", Schema = "nexus")]
    public class ProtectedBlobVaultRecord
    {
        [Key]
        [Column("id")]
        public int Id  { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("entropy")]
        public byte[] Entropy {get; set; }

        [Column("key_type")]
        public short KeyType { get; set;}

        [Column("user_id")]
        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual UserRecord User { get; set; }

        [Column("operational_environment_id")]
        public int? OperationalEnvironmentId { get; set; }

        [ForeignKey("OperationalEnvironmentId")]
        public virtual OperationalEnvironmentRecord OperationalEnvironment { get; set; }

        [Column("public_key_id")]
        public int? PublicKeyId { get; set;}

        [ForeignKey("PublicKeyId")]
        public virtual PublicKeyRecord PublicKey { get; set;}

        public virtual Collection<ProtectedBlobRecord> ProtectedBlobs  { get; set;}
     }
}