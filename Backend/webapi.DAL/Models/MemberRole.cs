using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapi.Models
{
    public class MemberRole : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; } = string.Empty;

        public ICollection<GroupMemberList> Members;
    }
}
