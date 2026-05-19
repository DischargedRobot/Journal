using System.IdentityModel.Tokens.Jwt;
using System.Text;

using Microsoft.IdentityModel.Tokens;
namespace AuthService.Lib.Utils
{
    public class TokenService
    {

        private string _secutiryKey;
        public TokenService(string secutiryKey) => _secutiryKey = secutiryKey;

        public string GenerateAccessToken(Guid tokenUuid, Guid userUUID, IEnumerable<string> roles)
        {
            SymmetricSecurityKey key = AuthOptions.GetSymmetricSecurityKey(_secutiryKey);
            JwtHeader header = new(new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
            JwtPayload payload = new()
            {
                { "iss", AuthOptions.ISSUER },
                { "aud", AuthOptions.AUDIENCE },
                { "sub", userUUID.ToString() },
                { "roles", roles },
                { "jti", tokenUuid.ToString() },
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

        public bool ValidateToken(string token, out Guid tokenUuid, out Guid userUUID, out IEnumerable<string> roles)
        {
            userUUID = Guid.Empty;
            roles = [];
            tokenUuid = Guid.Empty;

            JwtSecurityTokenHandler tokenHandler = new();
            try
            {
                tokenHandler.ValidateToken(token, 
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = AuthOptions.ISSUER,
                    ValidateAudience = true,
                    ValidAudiences = AuthOptions.AUDIENCE, // проверяем все элементы массива
                    ValidateLifetime = true,
                    IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(_secutiryKey),
                    ValidateIssuerSigningKey = true,
                }, 
                out SecurityToken validatedToken);

                JwtSecurityToken jwtToken = (JwtSecurityToken)validatedToken;
                tokenUuid = Guid.Parse(jwtToken.Claims.First(c => c.Type == "jti").Value);
                userUUID = Guid.Parse(jwtToken.Claims.First(c => c.Type == "sub").Value);
                // т.к. JwtSecurityToken при создании токена сериализует массив ролей в виде нескольких клеймов с одинаковым типом "roles", 
                // то для получения всех ролей нужно выбрать все клеймы с типом "roles" и взять их значения
                roles = jwtToken.Claims.Where(c => c.Type == "roles").Select(c => c.Value);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public static class AuthOptions
    {
        public static readonly string[] AUDIENCE = ["auth-service", "main-service"]; // потребитель токена
        public static readonly string ISSUER = "auth-service"; // издатель токена
        public static SymmetricSecurityKey GetSymmetricSecurityKey(string securityKey)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
        }
    }
}
