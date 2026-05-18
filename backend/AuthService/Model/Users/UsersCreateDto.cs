namespace AuthService.Model
{
    public class UsersCreateDto
    {
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Email { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Patronymic { get; set; }
        public string? RoleName { get; set; }
    }
}
