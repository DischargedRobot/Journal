using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainService
{
    public class LessonPresences
    {
        [Key]
        public int LessonPresenceId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();

        public bool IsPresent { get; set; } = false;

        public required int LessonId { get; set; }
        [ForeignKey("LessonId")]
        public Lessons? Lesson { get; set; }

        public required int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Students? Student { get; set; }

        public required int PresenceStatusId { get; set; }
        [ForeignKey("PresenceStatusId")]
        public PresenceStatuses? PresenceStatus { get; set; }
    }
}