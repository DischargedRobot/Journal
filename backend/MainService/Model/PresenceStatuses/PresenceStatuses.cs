using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class PresenceStatuses : BaseEntity
    {
        [Key]
        public int PresenceStatusId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();

        [Required]
        public required string Name { get; set; }

        public ICollection<LessonPresences>? LessonPresences { get; set; }
    }
}