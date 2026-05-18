namespace AuthService.Model
{
    public class SessionsUpdateDto
    {
        public DateTime? ExpiresAt { get; set; }
        public string? UserAgent { get; set; }
        public string? BrowserName { get; set; }
        public string? BrowserVersion { get; set; }
        public string? OsName { get; set; }
    }
}
