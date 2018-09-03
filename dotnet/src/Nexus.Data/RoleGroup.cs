using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Nexus.Data
{
    [Table("roles_groups", Schema = "nexus")]
    public class RoleGroup
    {
        [Key]
        [Column("role_id")]
        public int RoleId { get; set; }

        [Key]
        [Column("group_id")]
        public long? ResourceId { get; set; }
    }
}
