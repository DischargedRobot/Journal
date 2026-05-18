using System.Diagnostics.CodeAnalysis;

namespace AuthService.Model
{
    public class UsersResponseDto
    {
        public required Guid Uuid { get; set; }
        public required string Login { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Patronymic { get; set; }
        public int TokenVersion { get; set; }

        public UsersResponseDto() { }

        [SetsRequiredMembers]
        public UsersResponseDto(Users user)
        {
            Uuid = user.Uuid;
            Login = user.Login;
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Patronymic = user.Patronymic;
            TokenVersion = (int)user.TokenVersion;
        }
    }
}
