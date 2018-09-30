using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Nexus.Data
{
    [Table("operational_environments", Schema = "nexus")]
    public class OperationalEnvironmentRecord : IResource
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        [StringLength(256)]
        public string Name { get; set; }

        [Column("uri_path")]
        [StringLength(256)]
        public string UriPath { get; set; }

        [Column("description")]
        [StringLength(512)]
        public string Description { get; set;}

        [Column("alias")]
        [StringLength(32)]
        public string Alias { get; set; }

        [Column("resource_id")]
        public long? ResourceId { get; set; }

        [ForeignKey("ResourceId")]
        public virtual ResourceRecord Resource { get; set; }
    }
}
