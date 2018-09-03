using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Nexus.Data
{
    [Table("user_api_keys_roles", Schema = "nexus")]
    public class UserApiKeyRole
    {
        [Key]
        [Column("user_api_key_id")]
        public string UserApiKeyId { get; set; }

        [Key]
        [Column("role_id")]
        public int RoleId { get; set; }
    }
}
