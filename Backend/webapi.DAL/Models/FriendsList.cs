using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class FriendsList : BaseEntity
    {

        //Foreign key to UserInfo userId
        public long FirstUserId { get; set; }

        public UserInfo FirstUserInfo { get; set; }

        //Foreign key to UserInfo userId
        public long SecondUserId { get; set; }

        public UserInfo SecondUserInfo { get; set; }
    }
}
