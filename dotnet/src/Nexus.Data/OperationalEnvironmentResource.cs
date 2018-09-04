using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Nexus.Data
{
    [Table("operational_environments_resources", Schema = "nexus")]
    public class OperationalEnvironmentResource
    {
        [Column("operational_environment_id")]
        public int OperationalEnvironmentId { get; set; }

        [Column("resource_id")]
        public long ResourceId { get; set; }
    }
}
