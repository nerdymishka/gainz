using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Nexus.Data
{   
    [Table("configuration_sets", Schema = "nexus")]
    public class ConfigurationSetRecord
    {   
        public ConfigurationSetRecord()
        {
            this.Files = new Collection<ConfigurationFileRecord>();
        }

        [Column("id")]
        [Key]
        public int Id { get; set; }

        [Column("name")]
        [StringLength(256)]
        public string Name { get; set; }

        [Column("operational_environment_id")]
        public int? OperationalEnvironmentId { get; set; }

        [ForeignKey("OperationalEnvironmentId")]
        public virtual OperationalEnvironmentRecord OperationalEnvironment { get; set; }


        public virtual Collection<ConfigurationFileRecord> Files { get; set; }
    }
}