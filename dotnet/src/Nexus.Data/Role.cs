using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Nexus.Data
{
    [Table("roles", Schema = "nexus")]
    public class Role
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("uri_fragment")]
        [StringLength(256)]
        [Required]
        public string UriFragment { get; set; }

        [Column("display_name")]
        [StringLength(256)]
        public string DisplayName { get; set; }

        [Column("description")]
        [StringLength(512)]
        public string Description { get; set;}

        [Column("resource_id")]
        public long? ResourceId { get; set; }

        [ForeignKey("resource_id")]
        public virtual Resource Resource { get; set; }
    }
}
