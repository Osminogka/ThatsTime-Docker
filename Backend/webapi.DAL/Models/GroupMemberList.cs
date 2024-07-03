using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class GroupMemberList : BaseEntity
    {
        //Navigation property to GroupsCreatorsList
        public GroupsCreatorsList RelatedGroup { get; set; }

        //Foreign key to UserInfo.Id
        public long MemberId { get; set; }

        //Navigation property to UserInfo
        public UserInfo RelatedUser { get; set; }

        public long RoleId { get; set; }

        public MemberRole Role { get; set; }
    }
}
