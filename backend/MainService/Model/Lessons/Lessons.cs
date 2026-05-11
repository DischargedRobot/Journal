using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MainService
{
    public class Lessons
    {
        [Key]
        public required int LessonId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();

        [Required]
        public required int Code { get; set; }

        [Required]
        public required DateTime StartDate { get; set; } = DateTime.UtcNow;

        [Required]
        public required string Name { get; set; }

        [Required]
        public required string ShortName { get; set; }

        public required int LessonTypeId { get; set; }
        [ForeignKey("LessonTypeId")]
        public LessonTypes? LessonType { get; set; }

        public required int DisciplineId { get; set; }
        [ForeignKey("DisciplineId")]
        public Disciplines? Discipline { get; set; }

        public ICollection<LessonPresences>? LessonPresences { get; set; } = [];
        public ICollection<LessonMarks>? LessonMarks { get; set; } = [];
    }
}