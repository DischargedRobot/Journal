using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainService
{
    public class Groups
    {
        [Key]
        public int GroupId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();

        [Required]
        public required DateOnly AdmissionDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        [Required]
        public required string Code { get; set; }

        public ICollection<Brigades>? Brigades { get; set; } = [];

        public required int TrainingDirectionId { get; set; }
        [ForeignKey("TrainingDirectionId")]
        public TrainingDirections? TrainingDirection { get; set; }

        public required int FacultyId { get; set; }
        [ForeignKey("FacultyId")]
        public Faculties? Faculty { get; set; }

        public ICollection<Students>? Students { get; set; } = [];

        public ICollection<Disciplines>? Disciplines { get; set; } = [];

        public ICollection<Professors>? Curators { get; set; }
    }
}
