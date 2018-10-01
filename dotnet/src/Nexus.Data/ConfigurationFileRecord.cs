using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Nexus.Data
{
    [Table("configuration_files", Schema = "nexus")]
    public class ConfigurationFileRecord : IResource
    {
        [Key]
        [Column("id")]
        public int Id {get; set; }

        [Column("uri_path")]
        [StringLength(500)]
        public string UriPath {get; set;}

        [Column("content")]
        public byte[] Content { get; set; }
        
        [Column("description")]
        [StringLength(512)]
        public string Description { get; set; }

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

        [ForeignKey("ResourceId")]
        public virtual ResourceRecord Resource { get; set; }

        [Column("configuration_set_id")]
        public int? ConfigurationSetId { get; set; }

        [ForeignKey("ConfigurationSetId")]
        public ConfigurationSetRecord ConfigurationSet { get; set; }
    }
}
