using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Nexus.Data
{
    [Table("operational_environments_resources", Schema = "nexus")]
    public class OperationalEnvironmentResource
    {
        [Key]
        [Column("operational_environment_id")]
        public int Id { get; set; }

        [Key]
        [Column("resource_id")]
        public long ResourceId { get; set; }
    }
}
