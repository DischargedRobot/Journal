using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainService
{
    public class LessonMarks
    {
        [Key]
        public int LessonId { get; set; }
        public Lessons? Lesson { get; set; }

        public required int MarkId { get; set; }
        [ForeignKey("MarkId")]
        public Marks? Mark { get; set; }

        public required int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Students? Student { get; set; }
    }
}