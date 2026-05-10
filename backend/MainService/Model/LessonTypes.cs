using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class LessonTypes
    {
        [Key]
        public int LessonTypeId { get; set; }

        [Required]
        public required string Name { get; set; }

        public ICollection<Lessons>? Lessons { get; set; } = [];

        public ICollection<SelectedMarkTypes>? SelectedMarkTypes { get; set; } = [];
    }
}