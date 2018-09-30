using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Nexus.Data
{
    /// <summary>
    /// A member of the group. 
    /// </summary>
    [Table("groups_users", Schema = "nexus")]
    public class GroupUserRecord
    {
        [ForeignKey("GroupId")]
        public virtual GroupRecord Group { get; set; }
        
        [Column("group_id")]
        public int GroupId { get; set; }


        [ForeignKey("UserId")]
        public virtual UserRecord User { get; set; }


        [Column("user_id")]
        public int UserId { get; set; }

        [Column("membership_type")]
        public byte MembershipType {get; set;}
    }
}
