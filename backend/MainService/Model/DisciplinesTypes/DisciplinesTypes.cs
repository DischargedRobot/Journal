using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class DisciplinesTypes : BaseEntity
    {
        [Key]
        public int DisciplineTypeId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();

        [Required]
        public required string Name { get; set; }

        public string? ShortName { get; set; }

        public ICollection<Disciplines>? Disciplines { get; set; } = [];
    }
}