using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Nexus.Data
{
    [Table("roles_users", Schema = "nexus")]
    public class RoleUserRecord
    {

        [Column("role_id")]
        public int RoleId { get; set; }

        
        [Column("RoleId")]
        public virtual RoleRecord Role {get; set; }


        [Column("user_id")]
        public int UserId { get; set; }

        [Column("UserID")]
        public virtual UserRecord User {get; set; }
    }
}
