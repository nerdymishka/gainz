using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Nexus.Data
{
    [Table("users", Schema = "nexus")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("display_name")]
        [StringLength(255)]
        public string DisplayName { get; set; }

        [Column("name")]
        [StringLength(255)]
        public string Username { get; set; }

        [Column("is_banned")]
        public bool IsBanned { get; set; }

        [Column("resource_id")]
        public long? ResourceId { get; set; }

        [ForeignKey("resource_id")]
        public virtual Resource Resource { get; set; }
    }
}
