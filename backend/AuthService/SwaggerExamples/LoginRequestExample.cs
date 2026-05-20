using Swashbuckle.AspNetCore.Filters;
using AuthService.Controller.Dto;

namespace AuthService.SwaggerExamples
{
    public class LoginRequestExample : IExamplesProvider<LoginRequest>
    {
        public LoginRequest GetExamples()
        {
            return new LoginRequest
            {
                Login = "alice",
                Password = "P@ssw0rd!"
            };
        }
    }
}
