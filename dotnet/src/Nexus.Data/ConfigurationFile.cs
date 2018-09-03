using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Nexus.Data
{
    [Table("configuration_files", Schema = "nexus")]
    public class ConfigurationFile
    {
        [Key]
        [Column("id")]
        public int Id {get; set; }

        [Column("name")]
        [StringLength(500)]
        public string Name {get; set;}

        [Column("content")]
        public byte[] Content { get; set;}

        [Column("encoding")]
        [StringLength(100)]
        public string Encoding {get; set;}

        [Column("mime_type")]
        [StringLength(100)]
        public string MimeType {get; set;}

        [Column("is_encrypted")]
        public bool IsEncrypted {get; set;}

        [Column("resource_id")]
        public long? ResourceId { get; set; }

        [ForeignKey("resource_id")]
        public virtual Resource Resource { get; set; }

        [Column("owner_resource_id")]
        public long? ResourceIdOwner { get; set; }

        [ForeignKey("ResourceIdOwner")]
        public virtual Resource ResourceOwner { get; set; }
    }
}
