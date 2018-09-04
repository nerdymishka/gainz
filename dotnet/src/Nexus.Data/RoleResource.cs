using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Nexus.Data
{
    [Table("roles_resources", Schema = "nexus")]
    public class RoleResource
    {

        [Column("role_id")]
        public int RoleId { get; set; }

        [Column("resource_id")]
        public long ResourceId { get; set; }

        [Column("actions")]
        public byte Actions { get; set; } = 0x0;
    }
}
