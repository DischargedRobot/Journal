using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
namespace MainService
{
    [Index(nameof(MarkTypeId), nameof(Value), IsUnique = true)]
    public class Marks : BaseEntity
    {
        [Key]
        public int MarkId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();

        [Required]
        public required string Value { get; set; }

        public required int MarkTypeId { get; set; }
        [ForeignKey("MarkTypeId")]
        public MarkTypes? MarkType { get; set; }

        public ICollection<LessonMarks>? LessonMarks { get; set; }
    }
}