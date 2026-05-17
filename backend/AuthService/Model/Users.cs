using System.ComponentModel.DataAnnotations;

namespace AuthService.Models
{
    public class Users : BaseEntity
    {
        [Key]
        public int UserId { get; set; }

        public Guid Uuid { get; set; } = Guid.NewGuid();

        public required string Login { get; set; }
        public required string Password { get; set; }

        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Patronymic { get; set; }

        public ICollection<RefreshTokens>? RefreshTokens { get; set; }
        public ICollection<UserRoles>? UserRoles { get; set; }
    }
}