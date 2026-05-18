using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Model
{
    public class Sessions : BaseEntity
    {
        [Key]
        public int SessionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string? UserAgent { get; set; }
        public string? BrowserName { get; set; }
        public string? BrowserVersion { get; set; }
        public string? OsName { get; set; }
        public Guid RefreshTokenUuid { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public Users? User { get; set; }
    }
}
