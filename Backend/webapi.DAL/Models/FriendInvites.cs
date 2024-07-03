using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class FriendInvites : BaseEntity
    {
        //Foreign key to UserInfo userId
        public long SenderUserId { get; set; }

        //Navigation property
        public UserInfo SenderUserInfo { get; set; }

        public long TargetUserId { get; set; }

        public UserInfo TargetUserInfo { get; set; }
    }
}
