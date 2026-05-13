using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class DisciplinesRegisters : BaseEntity
    {
        [Key]
        public int DisciplineRegisterId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();

        [Required]
        public required string Name { get; set; }

        [Required] // == MinLength(1) || != null
        public required string ShortName { get; set; }

        public ICollection<Disciplines>? Disciplines { get; set; }
    }
}