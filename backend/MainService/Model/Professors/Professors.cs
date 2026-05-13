using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainService
{
    public class Professors : BaseEntity
    {
        [Key]
        public int ProfessorId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();

        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Departments? Department { get; set; }

        public int? PostId { get; set; }
        [ForeignKey("PostId")]
        public EmployeePosts? Post { get; set; }

        public required int AcademicYearId { get; set; }
        [ForeignKey("AcademicYearId")]
        public AcademicYears? AcademicYear { get; set; }

        public required int UniversityEmployerId { get; set; }
        [ForeignKey("UniversityEmployerId")]
        public UniversityEmployers? UniversityEmployer { get; set; }

        public ICollection<Groups>? GroupCurator { get; set; }

        public ICollection<Disciplines>? Disciplines { get; set; } = [];
    }
}