using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace MainService
{
    [PrimaryKey(nameof(LessonMarkId), nameof(MarkId), nameof(StudentId))]
    public class LessonMarks
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