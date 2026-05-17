using System.ComponentModel.DataAnnotations;

namespace AuthService.Models
{
    public class Users : BaseEntity
    {
        [Key]
        public int UserId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();
        public required string Login { get; set; }
        public required string PasswordHash { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Patronymic { get; set; }
        public uint TokenVersion { get; set; } = 0;
        public ICollection<Sessions>? Sessions { get; set; }
        public ICollection<Roles>? Roles { get; set; }
    }
}