using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Nexus.Data
{
    [Table("resources", Schema = "nexus")]
    public class Resource
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("key")]
        public long? Key { get; set; }

        [Column("type")]
        public string Type { get; set;}

        [Column("uri")]
        [StringLength(2048)]
        public string Uri { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set;} = false;
    }
}
