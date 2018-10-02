using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nexus.Data
{
    [Table("resource_id", Schema = "nexus")]
    public class ResourceKind
    {
        [Column("id")]
        public int Id {get; set;}

        [Column("name")]
        [StringLength(256)]
        public string Name {get; set;}

        [Column("uri_path")]
        [StringLength(1024)]
        public string UriPath {get; set;}
    }
}