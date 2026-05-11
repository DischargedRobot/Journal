using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();

        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Patronymic { get; set; }

        [Required]
        public required string UserUuid { get; set; }

        public Students? Student { get; set; }
        public UniversityEmployers? UniversityEmployer { get; set; }
    }
}
