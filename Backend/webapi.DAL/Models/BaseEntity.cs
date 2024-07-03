using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class BaseEntity
    {
        [Key]
        public long Id { get; set; }
    }
}
