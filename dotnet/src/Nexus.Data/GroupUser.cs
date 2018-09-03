using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Nexus.Data
{
    /// <summary>
    /// A member of the group. 
    /// </summary>
    [Table("groups_users", Schema = "nexus")]
    public class GroupUser
    {
        [Key]
        [Column("group_id")]
        public int GroupId { get; set; }

        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("membership_type")]
        public byte MembershipType {get; set;}
    }
}
