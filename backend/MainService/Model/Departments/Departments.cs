using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainService
{
    public class Departments : BaseEntity
    {
        [Key]
        public int DepartmentId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();

        [Required]
        public required string Name { get; set; }

        [Required] // == MinLength(1) || != null
        public required string ShortName { get; set; }

        [Required]
        public required string Code { get; set; }

        [Required]
        public required int FacultyId { get; set; }
        [ForeignKey("FacultyId")]
        public Faculties? Faculty { get; set; }

        public ICollection<Professors>? Professors { get; set; } = [];
    }
}