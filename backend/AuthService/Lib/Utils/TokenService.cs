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
        private byte[] _opaqueKey;
        public TokenService(string secutiryKey, byte[] opaqueKey)
        {
            _secutiryKey = secutiryKey;
            _opaqueKey = opaqueKey;
        }
        // TODO: Реализовать шифрование/дешифрование токена с помощью _opaqueKey
        public string GenerateOpaqueToken(Guid tokenUuid) => tokenUuid.ToString();
        public bool DecryptOpaqueToken(string token, out Guid uuid)
        {
            // TODO: Реализовать шифрование/дешифрование токена с помощью _opaqueKey
            uuid = Guid.Empty;
            if (Guid.TryParse(token, out Guid parsedUuid))
            {
                uuid = parsedUuid;
                return true;
            }
            else
            {
                return false;
            }
        }

        public class TokenOpaqueValidationResult
        {
            [MemberNotNullWhen(true, nameof(Token))]
            public bool IsValid { get; init; }
            /// <summary>
            /// Соедржит JWT токен, который можно распарсить
            /// </summary>
            public string? Token { get; init; }

            public void Deconstruct(out bool isValid, out string? token)
            {
                isValid = IsValid;
                token = Token;
            }
        }

        // TODO: переделать при жобавлении шифрования/дешифрования токена
        public async Task<TokenOpaqueValidationResult> ValidateOpaqueTokenAsync(
            string token,
            ITokenStore accesTokenStore,
            ITokenBlackListStore blacklist
        )
        {
            Guid tokenUuid = DecryptOpaqueToken(token, out Guid uuid) ? uuid : Guid.Empty;
            if (await blacklist.GetAsync(tokenUuid) != null)
            {
                return new TokenOpaqueValidationResult { IsValid = false };
            }
            string? accessToken = await accesTokenStore.GetAsync(tokenUuid);
            if (accessToken == null)
            {
                return new TokenOpaqueValidationResult { IsValid = false };
            }

            return new TokenOpaqueValidationResult
            {
                IsValid = true,
                Token = accessToken
            };
        }

        public string GenerateAccessToken(Guid tokenUuid, Guid userUUID, IEnumerable<string> roles)
        {
            SymmetricSecurityKey key = AuthOptions.GetSymmetricSecurityKey(_secutiryKey);
            JwtHeader header = new(new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
            JwtPayload payload = new()
            {
                { "iss", AuthOptions.ISSUER },
                { "aud", AuthOptions.AUDIENCE },
                { "sub", userUUID.ToString() },
                { "roles", roles.ToArray() },
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

        public async Task<TokenValidationResult> ValidateAccessTokenAsync(
            string token,
            ITokenBlackListStore? blacklist = null
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

        public async Task<TokenValidationResult> ValidateRefreshTokenAsync(
            string token,
            ITokenBlackListStore? blacklist = null
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
                Guid userUUID = Guid.Parse(jwtToken.Claims.First(c => c.Type == "sub").Value);
                Guid tokenUuid = Guid.Parse(jwtToken.Claims.First(c => c.Type == "jti").Value);

                if (blacklist != null && await blacklist.GetAsync(tokenUuid) != null)
                {
                    return new TokenValidationResult
                    {
                        IsValid = false,
                        Payload = new TokenPayload
                        {
                            TokenUuid = tokenUuid,
                            UserUuid = userUUID,
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
                    }
                };
            }
            catch
            {
                return new TokenValidationResult { IsValid = false, Payload = null };
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
}
