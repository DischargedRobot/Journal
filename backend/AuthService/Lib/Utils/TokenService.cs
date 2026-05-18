using System.IdentityModel.Tokens.Jwt;
using System.Text;

using Microsoft.IdentityModel.Tokens;
namespace AuthService.Lib.Utils
{
    public class TokenService
    {

        private string _secutiryKey;
        public TokenService(string secutiryKey) => _secutiryKey = secutiryKey;

        public string GenerateAccessToken(Guid userUUID, IEnumerable<string> roles)
        {
            SymmetricSecurityKey key = AuthOptions.GetSymmetricSecurityKey(_secutiryKey);
            JwtHeader header = new(new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
            JwtPayload payload = new()
            {
                { "iss", AuthOptions.ISSUER },
                { "aud", AuthOptions.AUDIENCE },
                { "sub", userUUID.ToString() },
                { "roles", roles },
                { "jti", Guid.NewGuid().ToString() },
                { "exp", DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds() }
            };
            JwtSecurityToken token = new(header, payload);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string GenerateRefreshToken(Guid userUUID)
        {
            SymmetricSecurityKey key = AuthOptions.GetSymmetricSecurityKey(_secutiryKey);
            JwtHeader header = new(new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
            JwtPayload payload = new()
            {
                { "iss", AuthOptions.ISSUER },
                { "aud", AuthOptions.AUDIENCE },
                { "sub", userUUID.ToString() },
                { "jti", Guid.NewGuid().ToString() },
                { "exp", DateTimeOffset.UtcNow.AddDays(7).ToUnixTimeSeconds() }
            };
            JwtSecurityToken token = new(header, payload);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public static class AuthOptions
    {
        public static readonly string[] AUDIENCE = ["AuthServer", "MainService"]; // потребитель токена
        public static readonly string ISSUER = "AuthServer"; // издатель токена
        public static SymmetricSecurityKey GetSymmetricSecurityKey(string securityKey)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
        }
    }
}
