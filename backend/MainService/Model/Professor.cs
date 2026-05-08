using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class Professors
    {
        [Key]
        public int ProfessorId { get; set; }

        public int? DepartmentId { get; set; }
        public Departments? Department { get; set; }

        public int? PostId { get; set; }
        public EmployeePosts? Post { get; set; }

        [Required]
        public int UniversityEmployerId { get; set; }
        public UniversityEmployers? UniversityEmployer { get; set; }
    }
}