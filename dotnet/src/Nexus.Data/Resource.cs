using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Nexus.Data
{
    [Table("resources", Schema = "nexus")]
    public class ResourceRecord
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("row_id")]
        public long? RowId { get; set; }

        [Column("kind_id")]
        public int KindId { get; set; }

        [ForeignKey("KindId")]
        public ResourceKindRecord Kind { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set;} = false;
    }
}
