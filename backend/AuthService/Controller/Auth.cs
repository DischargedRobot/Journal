using AuthService.Redis;
using AuthService.Controller.Dto;
using Microsoft.AspNetCore.Mvc;
using AuthService.Lib.Utils;
using Microsoft.EntityFrameworkCore;
using AuthService.Model;
using System.IdentityModel.Tokens.Jwt;
using AuthService.Errors;

namespace AuthService.Controller
{
    [ApiController]
    [Route("api/auth-service/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthServiceContext _context;
        private readonly RedisRefreshTokenBlackList _refreshTokenBlackList;
        private readonly RedisAccessTokenBlackList _accessTokenBlackList;
        private readonly TokenService _tokenService;

        public AuthController(AuthServiceContext context, RedisRefreshTokenBlackList refreshTokenBlackList, RedisAccessTokenBlackList accessTokenBlackList, TokenService tokenService)
        {
            _context = context;
            _refreshTokenBlackList = refreshTokenBlackList;
            _accessTokenBlackList = accessTokenBlackList;
            _tokenService = tokenService;
        }

        [HttpPost("log-in")]
        public IActionResult Login([FromForm] LoginRequest? request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "Логин или пароль не могут быть пустыми",
                    Field = nameof(request.Login) + "/" + nameof(request.Password)
                });
            }

            Users? user = _context.Users
            .Include(u => u.Roles)
            .FirstOrDefault(u => u.Login == request.Login);

            if (user == null || !HashingPassword.VerifyPassword(request.Password, user.PasswordHash))
            {
                return Unauthorized(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Неавторизован",
                    Message = "Пользователь с таким логином не найден или неверный пароль",
                    Field = nameof(request.Login) + "/" + nameof(request.Password)
                });
            }

            string accessToken = _tokenService.GenerateAccessToken(
                user.Uuid,
                user.Roles?.Select(r => r.Name) ?? Enumerable.Empty<string>()
            );
            Response.Headers.Append("Authorization", $"Bearer {accessToken}");

            string refreshToken = _tokenService.GenerateRefreshToken(user.Uuid);

            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/api/auth-service/v1/auth/refresh"
            });

            return Ok(new { accessToken });
        }

        [HttpPost("log-out")]
        public async Task<IActionResult> Logout()
        {
            string? authHeader = Request.Headers["Authorization"]
                .FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                string accessToken = authHeader.Substring("Bearer ".Length).Trim();
                try
                {
                    JwtSecurityTokenHandler handler = new();
                    JwtSecurityToken jwt = handler.ReadJwtToken(accessToken);
                    string? sub = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                    string? expired = jwt.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
                    string? jti = jwt.Claims.FirstOrDefault(c => c.Type == "jti")?.Value;
                    if (Guid.TryParse(sub, out Guid userUuid)
                        && long.TryParse(expired, out long expUnix)
                        && Guid.TryParse(jti, out Guid tokenUuid))
                    {
                        DateTimeOffset exp = DateTimeOffset.FromUnixTimeSeconds(expUnix);
                        TimeSpan ttl = exp - DateTimeOffset.UtcNow;
                        if (ttl > TimeSpan.Zero)
                        {
                            await _accessTokenBlackList.SaveAsync(tokenUuid, userUuid, ttl);
                        }
                    }
                }
                catch
                {
                }
            }

            if (Request.Cookies.TryGetValue("refreshToken", out string? refreshToken) && !string.IsNullOrWhiteSpace(refreshToken))
            {
                try
                {
                    JwtSecurityTokenHandler handler = new();
                    JwtSecurityToken jwt = handler.ReadJwtToken(refreshToken);
                    string? sub = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                    string? expired = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;
                    string? jti = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
                    if (Guid.TryParse(sub, out Guid userUuid)
                        && long.TryParse(expired, out long expUnix)
                        && Guid.TryParse(jti, out Guid tokenUuid))
                    {
                        DateTimeOffset exp = DateTimeOffset.FromUnixTimeSeconds(expUnix);
                        TimeSpan ttl = exp - DateTimeOffset.UtcNow;
                        if (ttl > TimeSpan.Zero)
                        {
                            await _refreshTokenBlackList.SaveAsync(tokenUuid, userUuid, ttl);
                        }
                    }
                }
                catch
                {
                }

                Response.Cookies.Append("refreshToken", string.Empty, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(-1),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Path = "/api/auth-service/v1/auth/refresh"
                });
            }

            return NoContent();
        }
    }
}