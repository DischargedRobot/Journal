using AuthService.Redis;
using AuthService.Controller.Dto;
using Microsoft.AspNetCore.Mvc;
using AuthService.Models;
using AuthService.Lib.Utils;
using Microsoft.EntityFrameworkCore;

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

        [HttpPost("login")]
        public IActionResult Login([FromForm] LoginRequest? request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
            {
                // TODO: заменить на ошибку из маин сервиса
                return BadRequest(
                    new { error = "Логин или пароль не могут быть пустыми" }
                );
            }

            Users? user = _context.Users
            .Include(u => u.Roles)
            .FirstOrDefault(u => u.Login == request.Login);
            if (user == null)
            {
                return Unauthorized();
            }

            if (!HashingPassword.VerifyPassword(request.Password, user.PasswordHash))
            {
                return Unauthorized();
            }

            // TODO: заменить на реальную генерацию токенов
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
    }
}