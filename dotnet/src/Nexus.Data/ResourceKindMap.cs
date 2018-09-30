using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nexus.Data
{
    [Table("resource_kind_maps", Schema = "nexus")]
    public class ResourceKindMapRecord
    {
        [Column("resource_kind_owner_id")]
        public int ResourceKindOwnerId { get; set; }

        [ForeignKey("RecordKindOwnerId")]
        public virtual ResourceRecord Owner { get; set; }

        [Column("resource_kind_id")]
        public int ResourceKindId { get; set; }

        [ForeignKey("ResourceKindId")]
        public virtual ResourceKindRecord ResourceKind { get; set; }
    }
}