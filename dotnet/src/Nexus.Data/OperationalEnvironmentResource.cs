using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Nexus.Data
{
    [Table("operational_environments_resources", Schema = "nexus")]
    public class OperationalEnvironmentResourceRecord
    {
        [Column("operational_environment_id")]
        public int OperationalEnvironmentId { get; set; }

        [ForeignKey("OperationalEnviornmentId")]
        public virtual OperationalEnvironmentRecord OperationEnvironment { get; set; }

        [Column("resource_id")]
        public long ResourceId { get; set; }

        [ForeignKey("ResourceId")]
        public ResourceRecord Resource {get; set;}
    }
}
