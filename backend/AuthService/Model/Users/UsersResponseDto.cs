using System.Diagnostics.CodeAnalysis;

namespace AuthService.Model
{
    public class UsersResponseDto : IExampleProvider<UsersResponseDto>
    {
        public required Guid Uuid { get; set; }
        public required string Login { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Patronymic { get; set; }
        public int TokenVersion { get; set; }

        public Guid[]? RolesUuid { get; set; }

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
            TokenVersion = user.TokenVersion;
            RolesUuid = user.Roles?.Select(r => r.Uuid).ToArray();
        }

        public static UsersResponseDto Example => new()
        {
            Uuid = Guid.NewGuid(),
            Login = "johndoe",
            Email = "johndoe@example.com",
            FirstName = "John",
            LastName = "Doe",
            Patronymic = "Middle",
            TokenVersion = 0,
            RolesUuid = Array.Empty<Guid>()
        };
    }
}
