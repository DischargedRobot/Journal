using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Swashbuckle.AspNetCore.Annotations;

namespace MainService
{
    public class Disciplines
    {
        [SwaggerSchema("Уникальный идентификатор дисциплины")]
        [Key]
        public int DisciplineId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();

        [Required] // == MinLength(1) || != null
        [SwaggerSchema("Название дисциплины")]
        public required string Name { get; set; }

        [Required] // == MinLength(1) || != null
        public required string ShortName { get; set; }

        public required bool IsArchived { get; set; } = false;

        public int? DisciplineRegisterId { get; set; }
        [ForeignKey("DisciplineRegisterId")]
        public DisciplinesRegisters? DisciplineRegister { get; set; }

        public required int SemesterId { get; set; }
        [ForeignKey("SemesterId")]
        public Semesters? Semester { get; set; }

        public required int AcademicYearId { get; set; }
        [ForeignKey("AcademicYearId")]
        public AcademicYears? AcademicYear { get; set; }

        public ICollection<Brigades>? Brigades { get; set; } = [];

        [Required, MinLength(1)]
        public ICollection<Groups> Groups { get; set; } = [];

        public ICollection<Professors>? Professors { get; set; } = [];

        public ICollection<NotesAboutStudent>? NotesAboutStudent { get; set; } = [];

        public ICollection<Lessons>? Lessons { get; set; } = [];

        public ICollection<SelectedMarkTypes>? SelectedMarkTypes { get; set; } = [];

        public ICollection<Attestations>? Attestations { get; set; } = [];
    }
}