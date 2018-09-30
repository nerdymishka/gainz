using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Nexus.Data
{
    [Table("roles", Schema = "nexus")]
    public class RoleRecord : IResource
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("uri_path")]
        [StringLength(256)]
        [Required]
        public string UriPath { get; set; }

        [Column("name")]
        [StringLength(256)]
        public string Name { get; set; }

        [Column("description")]
        [StringLength(512)]
        public string Description { get; set;}

        [Column("resource_id")]
        public long? ResourceId { get; set; }

        [ForeignKey("ResourceId")]
        public virtual ResourceRecord Resource { get; set; }
    }
}
