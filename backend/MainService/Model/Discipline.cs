using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class Disciplines
    {
        [Key]
        public int DisciplineId { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required bool IsArchived { get; set; } = false;

    }
}