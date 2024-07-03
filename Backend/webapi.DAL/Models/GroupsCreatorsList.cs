using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class GroupsCreatorsList : BaseEntity
    {
        [Required]
        public string GroupName { get; set; } = string.Empty;

        //Foreighn key to UserInfo.Id
        public long CreatorId { get; set; }

        //Navigation property to UserInfo
        public UserInfo Creator { get; set; }

        public bool IsGroupClosed { get; set; }

        //Navigation property to GroupInvites
        public ICollection<GroupInvites> GroupInvites { get; set; }

        //Navigation property to GroupMemberList
        public ICollection<GroupMemberList> GroupMembers { get; set; }

        public ICollection<Record> RecordsForThisGroup { get; set; }
    }
}
