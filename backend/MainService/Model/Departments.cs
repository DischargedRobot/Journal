using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class Departments
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required string ShortName { get; set; }

        [Required]
        public required string Code { get; set; }
    }
}