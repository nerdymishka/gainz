using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Nexus.Data
{
    [Table("roles_groups", Schema = "nexus")]
    public class RoleGroupRecord
    {
        [Column("role_id")]
        public int RoleId { get; set; }

        [Column("RoleId")]
        public virtual RoleRecord Role {get; set; }

        [Column("group_id")]
        public int GroupId { get; set; }

        [ForeignKey("GroupId")]
        public virtual GroupRecord Group { get; set; }
    }
}
