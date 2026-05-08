using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class Faculties
    {
        [Key]
        public int FacultyId { get; set; }

        [Required]
        public required string Name { get; set; }

        public required string ShortName { get; set; }

        public ICollection<Departments>? Departments { get; set; }
    }
}