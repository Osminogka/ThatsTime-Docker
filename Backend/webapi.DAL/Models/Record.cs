using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace webapi.Models
{
    public class Record : BaseEntity
    {
        public Record()
        {

        }

        public Record(RecordFromFrontEnd recordFromFrontEnd, long selectedObjectId, long creatorId)
        {
            DateTime = new DateTime(recordFromFrontEnd.selectedYear, recordFromFrontEnd.selectedMonth, recordFromFrontEnd.selectedDay,recordFromFrontEnd.hour, recordFromFrontEnd.minute, 0);
            IsRecordForGroup = recordFromFrontEnd.showGroupList;
            if(IsRecordForGroup)
            {
                RelatedGroupId = selectedObjectId;
                RelatedUserId = null;
            }
            else
            {
                RelatedUserId = selectedObjectId;
                RelatedGroupId = null;
            }
            CreatorId = creatorId;
            IsRecordForYourSelf = recordFromFrontEnd.yourSelf;
            if(IsRecordForYourSelf)
                RelatedUserId = creatorId;
            Importance = recordFromFrontEnd.importance;
            RecordName = recordFromFrontEnd.recordName;
            RecordContent = recordFromFrontEnd.recordContent;
        }

        public DateTime DateTime { get; set; }

        public bool IsRecordForGroup { get; set; }

        //Foreign key to userInfo userId
        public long? RelatedUserId { get; set; }
        public UserInfo RelatedUser { get; set; }

        //Foreign key to GroupCreatorList groupId
        public long? RelatedGroupId { get; set; }
        public GroupsCreatorsList RelatedGroup { get; set; }

        [Required]
        public long CreatorId { get; set; }

        public UserInfo CreatorUser { get; set; }

        [Required]
        public bool IsRecordForYourSelf { get; set; }

        [Required]
        public int Importance { get; set; }

        [Required]
        [StringLength(1, MinimumLength = 50)]
        public string RecordName { get; set; } = string.Empty;

        [Required]
        [StringLength(1, MinimumLength = 500)]
        public string RecordContent { get; set; } = string.Empty;
    }
}
