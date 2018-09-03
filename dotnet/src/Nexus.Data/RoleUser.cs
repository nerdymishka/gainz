using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Nexus.Data
{
    [Table("roles_users", Schema = "nexus")]
    public class RoleUser
    {
        [Key]
        [Column("role_id")]
        public int RoleId { get; set; }

        [Key]
        [Column("resource_id")]
        public int UserId { get; set; }
    }
}
