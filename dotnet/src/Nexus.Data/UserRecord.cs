using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Nexus.Data
{
    [Table("users", Schema = "nexus")]
    public class UserRecord : IResource
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("display_name")]
        [StringLength(255)]
        public string DisplayName { get; set; }

        [Column("icon_uri")]
        [StringLength(1014)]
        public string IconUri { get; set;}

        [Column("password")]
        public string Password { get; set; }

        [Column("name")]
        [StringLength(255)]
        public string Name { get; set; }

        [Column("is_banned")]
        public bool IsBanned { get; set; }

        [Column("is_admin")]
        public bool IsAdmin { get; set; }

        [Column("role_cache")]
        [StringLength(2048)]
        public string RoleCache { get; set; }

        [Column("resource_id")]
        public long? ResourceId { get; set; }

        [ForeignKey("ResourceId")]
        public virtual ResourceRecord Resource { get; set; }


        public virtual Collection<UserApiKeyRecord> ApiKeys { get; set; }

        public virtual Collection<PublicKeyRecord> PublicKeys { get; set; }
    }
}