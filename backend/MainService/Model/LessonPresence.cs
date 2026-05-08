using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class LessonPresences
    {
        [Key]
        public int LessonPresenceId { get; set; }

        public bool IsPresent { get; set; }

        public int LessonId { get; set; }
        public Lessons? Lesson { get; set; }

        public int StudentId { get; set; }
        public Students? Student { get; set; }

        public int? PresenceStatusId { get; set; }
        public PresenceStatuses? PresenceStatus { get; set; }
    }
}