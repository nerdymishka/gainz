using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Nexus.Data
{
    [Table("user_api_keys", Schema = "nexus")]
    public class UserApiKeyRecord
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual UserRecord User {get; set; }

        [Column("api_key")]
        [StringLength(1024)]
        public string ApiKey { get; set; }

        [Column("expires_at")]
        public DateTime? ExpiresAt { get; set; }
    }
}
