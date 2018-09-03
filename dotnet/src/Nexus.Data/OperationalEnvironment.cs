using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Nexus.Data
{
    [Table("operational_environments", Schema = "nexus")]
    public class OperationalEnvironment
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        [StringLength(256)]
        public string Name { get; set; }

        [Column("display_name")]
        [StringLength(256)]
        public string DisplayName { get; set; }

        [Column("description")]
        [StringLength(512)]
        public string Description { get; set;}

        [Column("alias")]
        [StringLength(20)]
        public string Alias { get; set; }

        [Column("resource_id")]
        public long? ResourceId { get; set; }

        [ForeignKey("resource_id")]
        public virtual Resource Resource { get; set; }
    }
}
