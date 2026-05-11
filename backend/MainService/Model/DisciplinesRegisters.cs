using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class DisciplinesRegisters
    {
        [Key]
        public int DisciplineId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();

        [Required]
        public required string DisciplineName { get; set; }

        public ICollection<Disciplines>? Disciplines { get; set; }
    }
}