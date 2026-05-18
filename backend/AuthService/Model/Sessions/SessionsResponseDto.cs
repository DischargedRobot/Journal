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
    }
}
