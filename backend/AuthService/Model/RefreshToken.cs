using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models
{
    public class RefreshTokens : BaseEntity
    {
        [Key]
        public int RefreshId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        public required string TokenHash { get; set; }
        public string? UserAgent { get; set; }
        public string? BrowserName { get; set; }
        public string? BrowserVersion { get; set; }
        public string? OsName { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public Users? User { get; set; }
    }
}
