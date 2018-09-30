using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Nexus.Data
{
    [Table("groups", Schema = "nexus")]
    public class GroupRecord : IResource
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

        public virtual Collection<GroupUserRecord> GroupUsers { get; set; }

        public virtual Collection<RoleGroupRecord> GroupRoles { get; set; }
    }
}
