using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Nexus.Data
{
    [Table("user_api_keys_roles", Schema = "nexus")]
    public class UserApiKeyRoleRecord
    {
        [Column("user_api_key_id")]
        public int UserApiKeyId { get; set; }

        [ForeignKey("UserApiKeyId")]
        public UserApiKeyRecord UserApiKey { get; set;}

        [Column("role_id")]
        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public RoleRecord Role { get; set; }
    }
}
