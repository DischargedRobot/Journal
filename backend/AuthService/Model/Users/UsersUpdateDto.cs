namespace AuthService.Model
{
    public class UsersUpdateDto
    {
        public string? Login { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Patronymic { get; set; }
        public Guid[]? RolesUuid { get; set; }
    }
}
