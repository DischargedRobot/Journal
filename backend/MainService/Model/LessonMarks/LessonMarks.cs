using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace MainService
{
    [Index(nameof(LessonId), nameof(MarkId), nameof(StudentId), IsUnique = true)]
    public class LessonMarks : BaseEntity
    {
        [Key]
        public int LessonMarkId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();

        public required int LessonId { get; set; }
        [ForeignKey("LessonId")]
        public Lessons? Lesson { get; set; }

        public required int MarkId { get; set; }
        [ForeignKey("MarkId")]
        public Marks? Mark { get; set; }

        public required int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Students? Student { get; set; }
    }
}