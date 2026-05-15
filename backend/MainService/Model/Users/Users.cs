using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public enum UserRole
    {
        Unknown = 0,
        Student = 1,
        Professor = 2
    }

    public class Users : BaseEntity
    {
        [Key]
        public int UserId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();

        [Required]
        public required string UserUuid { get; set; }

        public StudentPersons? StudentPerson { get; set; }
        public UniversityEmployers? UniversityEmployer { get; set; }

        public UserRole Role { get; set; } = UserRole.Unknown;

    }
}
