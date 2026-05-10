using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class DisciplinesRegisters
    {
        [Key]
        public int DisciplineId { get; set; }

        [Required]
        public required string DisciplineName { get; set; }

        public ICollection<Disciplines>? Disciplines { get; set; }
    }
}