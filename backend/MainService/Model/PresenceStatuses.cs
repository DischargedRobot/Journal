using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class PresenceStatuses
    {
        [Key]
        public int PresenceStatusId { get; set; }

        [Required]
        public required string Name { get; set; }

        public ICollection<LessonPresences>? LessonPresences { get; set; }
    }
}