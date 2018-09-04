using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Nexus.Data
{
    [Table("roles_users", Schema = "nexus")]
    public class RoleUser
    {

        [Column("role_id")]
        public int RoleId { get; set; }


        [Column("user_id")]
        public int UserId { get; set; }
    }
}
