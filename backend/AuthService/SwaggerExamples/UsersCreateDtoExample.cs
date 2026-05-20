using Swashbuckle.AspNetCore.Filters;
using AuthService.Model;

namespace AuthService.SwaggerExamples
{
    public class UsersCreateDtoExample : IExamplesProvider<UsersCreateDto>
    {
        public UsersCreateDto GetExamples()
        {
            return new UsersCreateDto
            {
                Login = "alice",
                Password = "P@ssw0rd!",
                Email = "alice@example.com",
                FirstName = "Alice",
                LastName = "Ivanova",
                RolesUuid = null
            };
        }
    }
}
