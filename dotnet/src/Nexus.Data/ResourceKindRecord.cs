using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Nexus.Data
{
    [Table("resource_kinds", Schema = "nexus")]
    public class ResourceKindRecord
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("name")]
        [StringLength(256)]
        public string Name { get; set; }

        [Column("uri_path")]
        [StringLength(256)]
        public string UriPath { get; set; }

        [Column("table_name")]
        [StringLength(512)]
        public string TableName { get; set; }

        [Column("clr_type_name")]
        [StringLength(512)]
        public string ClrTypeName { get; set; }
    }

}