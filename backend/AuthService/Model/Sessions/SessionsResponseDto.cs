namespace AuthService.Model
{
    public class SessionsResponseDto
    {
        public int SessionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public Guid RefreshTokenUuid { get; set; }
        public int UserId { get; set; }
        public Guid UserUuid { get; set; }
        public string? UserAgent { get; set; }
        public string? BrowserName { get; set; }
        public string? BrowserVersion { get; set; }
        public string? OsName { get; set; }

        public static SessionsResponseDto Example => new()
        {
            SessionId = 1,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            RefreshTokenUuid = Guid.NewGuid(),
            UserId = 1,
            UserUuid = Guid.NewGuid(),
            UserAgent = "Mozilla/5.0",
            BrowserName = "Firefox",
            BrowserVersion = "112.0",
            OsName = "Windows"
        };
    }
}
