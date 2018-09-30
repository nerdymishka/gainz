using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Nexus.Data
{
    [Table("roles_resources", Schema = "nexus")]
    public class RoleResourceRecord
    {

        [Column("role_id")]
        public int RoleId { get; set; }

        
        [Column("RoleId")]
        public virtual RoleRecord Role {get; set; }

        [Column("resource_id")]
        public long ResourceId { get; set; }

        
        [Column("ResourceId")]
        public virtual ResourceRecord Resource {get; set; }

        [Column("actions")]
        public short Actions { get; set; } = 0;
    }
}
