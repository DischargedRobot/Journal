using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public required string UserUuid { get; set; }

        public Students? Student { get; set; }
        public UniversityEmployers? UniversityEmployer { get; set; }
    }
}
