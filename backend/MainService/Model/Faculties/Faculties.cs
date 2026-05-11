using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class Faculties
    {
        [Key]
        public int FacultyId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();

        public required string Name { get; set; }

        public required string ShortName { get; set; }

        public ICollection<Departments>? Departments { get; set; } = [];

        public ICollection<Groups>? Groups { get; set; } = [];
    }
}