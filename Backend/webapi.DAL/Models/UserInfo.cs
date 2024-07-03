using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class UserInfo : BaseEntity
    {
        [Required]
        public string UserName { get; set; } = string.Empty;


        //Navigation property to GroupInvites
        public ICollection<GroupInvites> GroupInvites;

        //Navigation property to GroupsCreatorsList
        public ICollection<GroupsCreatorsList> CreatorOfGroups;

        //Navigation property to GroupMemberList
        public ICollection<GroupMemberList> GroupMembers;

        //Navigation property to user that sent invite
        public ICollection<FriendInvites> SenderUsers;

        //Navigation property to user that received invite
        public ICollection<FriendInvites> TargetUsers;

        public ICollection<FriendsList> FirstFromFriendList;

        public ICollection<FriendsList> SecondFromFriendList;

        public ICollection<Record> RecordsForThisUser;

        public ICollection<Record> RecordsCreators;
    }
}
