using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class Lessons
    {
        [Key]
        public int LessonId { get; set; }

        [Required]
        public int Code { get; set; }

        [Required]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        [Required]
        public required string Name { get; set; }
        [Required]
        public required string ShortName { get; set; }

        public ICollection<LessonPresences>? LessonPresences { get; set; }
        public ICollection<LessonMarks>? LessonMarks { get; set; }
    }
}