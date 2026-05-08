using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class LessonMarks
    {
        [Key]
        public int LessonMarkId { get; set; }

        public int LessonId { get; set; }
        public Lessons? Lesson { get; set; }

        public int MarkId { get; set; }
        public Marks? Mark { get; set; }

        public int StudentId { get; set; }
        public Students? Student { get; set; }
    }
}