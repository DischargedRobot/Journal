using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class LessonTypes
    {
        [Key]
        public int LessonTypeId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();

        [Required]
        public required string Name { get; set; }

        public ICollection<Lessons>? Lessons { get; set; } = [];

        public ICollection<SelectedMarkTypes>? SelectedMarkTypes { get; set; } = [];
    }
}