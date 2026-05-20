using System.IdentityModel.Tokens.Jwt;
using System.Text;

using AuthService.Redis;
using System.Diagnostics.CodeAnalysis;

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

        public class TokenValidationResult
        {
            [MemberNotNullWhen(true, nameof(Payload))]
            public bool IsValid { get; init; }
            public TokenPayload? Payload { get; init; }

            public void Deconstruct(out bool isValid, out TokenPayload? payload)
            {
                isValid = IsValid;
                payload = Payload;
            }
        }

        public class TokenPayload
        {
            public Guid TokenUuid { get; set; }
            public Guid UserUuid { get; set; }
            public IEnumerable<string> Roles { get; set; } = [];
        }

        public async Task<TokenValidationResult> ValidateTokenAsync(
            string token,
            ITokenStore? blacklist = null
        )
        {
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
                Guid tokenUuid = Guid.Parse(jwtToken.Claims.First(c => c.Type == "jti").Value);
                Guid userUUID = Guid.Parse(jwtToken.Claims.First(c => c.Type == "sub").Value);
                string[] roles = jwtToken.Claims.Where(c => c.Type == "roles").Select(c => c.Value).ToArray();

                if (blacklist != null && await blacklist.GetAsync(tokenUuid) != null)
                {
                    return new TokenValidationResult
                    {
                        IsValid = false,
                        Payload = new TokenPayload
                        {
                            TokenUuid = tokenUuid,
                            UserUuid = userUUID,
                            Roles = roles
                        }
                    };
                }

                return new TokenValidationResult
                {
                    IsValid = true,
                    Payload = new TokenPayload
                    {
                        TokenUuid = tokenUuid,
                        UserUuid = userUUID,
                        Roles = roles
                    }
                };
            }
            catch
            {
                return new TokenValidationResult { IsValid = false, Payload = null };
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
