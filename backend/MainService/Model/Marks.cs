using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
namespace MainService
{
    [Index(nameof(MarkTypeId), nameof(Value), IsUnique = true)]
    public class Marks
    {
        [Key]
        public int MarkId { get; set; }

        [Required]
        public required string Value { get; set; }

        public required int MarkTypeId { get; set; }
        [ForeignKey("MarkTypeId")]
        public MarkTypes? MarkType { get; set; }

        public ICollection<LessonMarks>? LessonMarks { get; set; }
    }
}