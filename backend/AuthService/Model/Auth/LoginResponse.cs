namespace AuthService.Model.Auth.Dto
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = null!;

        public static LoginResponse Example => new()
        {
            AccessToken = "example_access_token"
        };
    }
}