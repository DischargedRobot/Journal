using AuthService.Redis;
using AuthService.Controller.Dto;
using Microsoft.AspNetCore.Mvc;
using AuthService.Lib.Utils;
using Microsoft.EntityFrameworkCore;
using AuthService.Model;
using System.IdentityModel.Tokens.Jwt;
using AuthService.Errors;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using Serilog.Context;

namespace AuthService.Controller
{
    [ApiController]
    [Route("api/auth-service/v1/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly ILogger<AuthController> _logger;
        private readonly AuthServiceContext _context;
        private readonly RedisRefreshTokenBlackList _refreshTokenBlackList;
        private readonly RedisAccessTokenBlackList _accessTokenBlackList;
        private readonly TokenService _tokenService;
        private readonly ActivitySource _activitySource;

        public AuthController(
            ILogger<AuthController> logger,
            AuthServiceContext context,
            RedisRefreshTokenBlackList refreshTokenBlackList,
            RedisAccessTokenBlackList accessTokenBlackList,
            TokenService tokenService,
            ActivitySource activitySource)
        {
            _logger = logger;
            _context = context;
            _refreshTokenBlackList = refreshTokenBlackList;
            _accessTokenBlackList = accessTokenBlackList;
            _tokenService = tokenService;
            _activitySource = activitySource;
        }

        [HttpPost("log-in")]
        public IActionResult Login([FromForm] LoginRequest? request)
        {
            try
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

                Guid tokenUuid = Guid.NewGuid();
                string accessToken = _tokenService.GenerateAccessToken(
                    tokenUuid,
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login: unexpected error");
                return StatusCode(500, new ApiError
                {
                    StatusCode = "1.0.0",
                    Title = "Внутренняя ошибка сервера",
                    Message = "Произошла ошибка на сервере"
                });
            }
        }

        [HttpPost("log-out")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                using Activity? activity = _activitySource?.StartActivity("auth-service.Logout", ActivityKind.Server);

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
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Logout: error processing access token");
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
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Logout: error processing refresh token");
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout: unexpected error");
                return StatusCode(500, new ApiError
                {
                    StatusCode = "1.0.0",
                    Title = "Внутренняя ошибка сервера",
                    Message = "Произошла ошибка на сервере"
                });
            }
        }

        [HttpPost("check-authtoken")]
        public async Task<IActionResult> CheckAuthtoken()
        {
            try
            {

                // Устанавливаем имя сервиса и функции в заголовки сразу после старта activity
                string serviceName = _activitySource?.Name
                    ?? "auth-service";

                string functionName = ControllerContext.ActionDescriptor.ActionName;
                using Activity? activity = _activitySource?.StartActivity($"{serviceName}.{functionName}", ActivityKind.Server);

                string? authHeader = Request.Headers.Authorization.FirstOrDefault();

                if (string.IsNullOrWhiteSpace(authHeader))
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "2.4.0",
                        Title = "Неверный запрос",
                        Message = "Заголовок Authorization не может быть пустым",
                        Field = "Authorization"
                    });
                }
                if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "2.4.2",
                        Title = "Неверный запрос",
                        Message = "Заголовок Authorization должен быть в формате 'Bearer {token}'",
                        Field = "Authorization"
                    });
                }

                string token = authHeader.Substring("Bearer ".Length).Trim();
                try
                {

                    string traceParentValue = string.Empty;
                    if (Request.Headers.TryGetValue("traceparent", out Microsoft.Extensions.Primitives.StringValues tp))
                    {
                        traceParentValue = tp.ToString();
                        Response.Headers.TraceParent = tp;
                    }
                    else if (Activity.Current != null)
                    {
                        traceParentValue = Activity.Current.Id ?? string.Empty;
                        if (!string.IsNullOrEmpty(traceParentValue))
                        {
                            Response.Headers.TraceParent = traceParentValue;
                        }
                    }

                    string traceId = Activity.Current?.TraceId.ToString() ?? string.Empty;

                    //  traceparent в контекст логирования, 
                    // чтобы Enrich.FromLogContext() включил их в {Properties}
                    _logger.LogInformation(
                        "CheckAuthtoken start {Operation} {Path}",
                        functionName,
                        Request.Path);

                    _tokenService.ValidateToken(token, out Guid tokenUuid, out Guid userUUID, out IEnumerable<string> roles);
                    if (await _accessTokenBlackList.GetAsync(tokenUuid) != null)
                    {
                        return Unauthorized(new ApiError
                        {
                            StatusCode = "2.2.1",
                            Title = "Недействительный токен",
                            Message = "Токен был отозван",
                            Field = "blacklist"
                        });
                    }

                    else if (Activity.Current != null)
                    {
                        Response.Headers.TraceParent = Activity.Current.Id;
                    }

                    if (!string.IsNullOrEmpty(traceId))
                    {
                        Response.Headers["X-Trace-Id"] = traceId;
                    }
                    return Ok(new { valid = true });
                }
                catch (SecurityTokenException ex)
                {
                    _logger.LogWarning(ex, "CheckAuthtoken: token validation failed");
                    return Unauthorized(new ApiError
                    {
                        StatusCode = "2.2.2",
                        Title = "Недействительный токен",
                        Message = ex.Message,
                        Field = "exp"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CheckAuthtoken: unexpected error");
                return StatusCode(500, new ApiError
                {
                    StatusCode = "1.0.0",
                    Title = "Внутренняя ошибка сервера",
                    Message = "Произошла ошибка на сервере"
                });
            }
        }
    }

    // [HttpPost("register")]
    // public async Task<IActionResult> Register()
    // {
    //     // Implementation for user registration
    // }

    // [HttpPost("refresh")]
    // public async Task<IActionResult> Refresh()
    // {
    //     // Implementation for token refresh
    // }
}