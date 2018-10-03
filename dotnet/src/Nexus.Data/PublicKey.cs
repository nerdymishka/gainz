using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nexus.Data
{
    [Table("public_keys", Schema = "nexus")]
    public class PublicKeyRecord
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("uri_path")]
        [StringLength(512)]
        public string UriPath  {get; set ;}

        [Column("blob")]
        public byte[] Blob  { get; set; }

        [Column("user_id")]
        public int? UserId  { get; set; }

        [ForeignKey("UserId")]
        public UserRecord User { get; set; }
    }
}