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
using Swashbuckle.AspNetCore.Annotations;

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
        [ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.0", "Неверный запрос", "Логин или пароль не могут быть пустыми", "Login/Password")]
        [ApiErrorExample(StatusCodes.Status401Unauthorized, "1.2.3", "Неавторизован", "Пользователь с таким логином не найден или неверный пароль", "Login/Password")]
        public IActionResult Login([FromBody] LoginRequest? request)
        {
            try
            {
                using Activity? activity = _activitySource.StartAndLog(_logger, this);

                if (request == null || string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
                {
                    _logger.LogWarning("Вход: неверные данные запроса при попытке входа. Login={Login}", request?.Login);
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
                    _logger.LogWarning("Вход: неудачная попытка входа для логина {Login}", request.Login);
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
                _logger.LogError(ex, "Вход: непредвиденная ошибка");
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
                using Activity? activity = _activitySource.StartAndLog(_logger, this);

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
                        _logger.LogError(ex, "Выход: ошибка обработки access-токена");
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
                        _logger.LogError(ex, "Выход: ошибка обработки refresh-токена");
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
                _logger.LogError(ex, "Выход: непредвиденная ошибка");
                return StatusCode(500, new ApiError
                {
                    StatusCode = "1.0.0",
                    Title = "Внутренняя ошибка сервера",
                    Message = "Произошла ошибка на сервере"
                });
            }
        }

        [HttpPost("check-authtoken")]
        [ApiErrorExample(StatusCodes.Status400BadRequest, "2.4.0", "Неверный запрос", "Заголовок Authorization не может быть пустым", "Authorization")]
        [ApiErrorExample(StatusCodes.Status400BadRequest, "2.4.2", "Неверный запрос", "Заголовок Authorization должен быть в формате 'Bearer {token}'", "Authorization")]
        [ApiErrorExample(StatusCodes.Status401Unauthorized, "2.2.2", "Недействительный токен", "Токен не прошёл проверку", "exp")]
        [ApiErrorExample(StatusCodes.Status401Unauthorized, "2.2.1", "Недействительный токен", "Токен был отозван", "blacklist")]
        public async Task<IActionResult> CheckAuthtoken()
        {
            try
            {
                using Activity? activity = _activitySource.StartAndLog(_logger, this);

                string? authHeader = Request.Headers.Authorization.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(authHeader))
                {
                    _logger.LogWarning("Проверка токена: пустой заголовок Authorization");
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
                    _logger.LogWarning("Проверка токена: неверный формат заголовка Authorization");
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
                    _logger.LogWarning(ex, "Проверка токена: ошибка валидации токена");
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
                _logger.LogError(ex, "Проверка токена: непредвиденная ошибка");
                return StatusCode(500, new ApiError
                {
                    StatusCode = "1.0.0",
                    Title = "Внутренняя ошибка сервера",
                    Message = "Произошла ошибка на сервере"
                });
            }
        }

        [HttpPost("register")]
        [SwaggerResponse(StatusCodes.Status201Created, "Пользователь создан")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Конфликт: логин занят", typeof(ApiError))]
        [ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "Неверный формат данных", "BODY")]
        [ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.0", "Неверный запрос", "Логин или пароль не могут быть пустыми", "Login/Password")]
        [ApiErrorExample(StatusCodes.Status409Conflict, "1.1.1", "Конфликт", "Пользователь с таким логином уже существует", "Login")]
        [SwaggerOperation(Summary = "Регистрация нового пользователя")]
        public async Task<IActionResult> Register([FromBody] UsersCreateDto? request)
        {
            try
            {
                using Activity? activity = _activitySource.StartAndLog(_logger, this);

                _logger.LogInformation("Регистрация: попытка для логина {Login}", request?.Login);
                if (request == null)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.1",
                        Title = "Неверный запрос",
                        Message = "Неверный формат данных",
                        Field = "BODY"
                    });
                }

                if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.0",
                        Title = "Неверный запрос",
                        Message = "Логин или пароль не могут быть пустыми",
                        Field = "Login/Password"
                    });
                }
                if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.1",
                        Title = "Неверный запрос",
                        Message = "FirstName и LastName обязательны",
                        Field = "FirstName, LastName"
                    });
                }
                bool exists = await _context.Users.AnyAsync(u => u.Login == request.Login);
                if (exists)
                {
                    _logger.LogWarning("Регистрация: логин уже существует {Login}", request.Login);
                    return Conflict(new ApiError
                    {
                        StatusCode = "1.1.1",
                        Title = "Конфликт",
                        Message = "Пользователь с таким логином уже существует",
                        Field = "Login"
                    });
                }

                Users user = new()
                {
                    Uuid = Guid.NewGuid(),
                    Login = request.Login.Trim(),
                    PasswordHash = HashingPassword.ComputeHash(request.Password, string.Empty),
                    Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
                    FirstName = request.FirstName.Trim(),
                    LastName = request.LastName.Trim(),
                    Patronymic = string.IsNullOrWhiteSpace(request.Patronymic) ? null : request.Patronymic.Trim(),
                    TokenVersion = 0,
                    Roles = new List<Roles>()
                };

                if (request.RolesUuid != null)
                {
                    Guid[] roleUuids = request.RolesUuid.Distinct().ToArray();
                    List<Roles> foundRoles = await _context.Roles.Where(r => roleUuids.Contains(r.Uuid)).ToListAsync();
                    if (foundRoles.Count != roleUuids.Length)
                    {
                        Guid[] missing = roleUuids.Except(foundRoles.Select(r => r.Uuid)).ToArray();
                        _logger.LogWarning(
                            "Регистрация: указаны несуществующие роли {Missing}",
                            string.Join(",", missing)
                        );
                        return BadRequest(new ApiError
                        {
                            StatusCode = "0.2.3",
                            Title = "Неверный запрос",
                            Message = "Одна или несколько ролей не найдены",
                            Field = nameof(request.RolesUuid),
                            Details = string.Join(", ", missing)
                        });
                    }
                    user.Roles = foundRoles;
                }

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Выдаём токены сразу после регистрации
                Guid tokenUuid = Guid.NewGuid();
                string accessToken = _tokenService.GenerateAccessToken(tokenUuid, user.Uuid, user.Roles?.Select(r => r.Name) ?? Enumerable.Empty<string>());
                string refreshToken = _tokenService.GenerateRefreshToken(user.Uuid);
                Response.Headers.Append("Authorization", $"Bearer {accessToken}");
                Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict, Path = "/api/auth-service/v1/auth/refresh" });

                _logger.LogInformation("Регистрация: пользователь создан {UserUuid}", user.Uuid);
                return Created(string.Empty, new { accessToken });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Регистрация: непредвиденная ошибка");
                return StatusCode(500, new ApiError
                {
                    StatusCode = "1.0.0",
                    Title = "Внутренняя ошибка сервера",
                    Message = "Произошла ошибка на сервере"
                });
            }
        }

        [HttpPost("refresh")]
        [SwaggerOperation(Summary = "Обновить access token по refresh token")]
        [SwaggerResponse(StatusCodes.Status200OK, "Новый access token выдан")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Недействительный refresh token", typeof(ApiError))]
        [ApiErrorExample(StatusCodes.Status401Unauthorized, "2.4.0", "Недействительный запрос", "Refresh token не предоставлен", "refreshToken")]
        [ApiErrorExample(StatusCodes.Status401Unauthorized, "2.2.2", "Недействительный токен", "Refresh token не прошёл проверку", "refreshToken")]
        [ApiErrorExample(StatusCodes.Status401Unauthorized, "2.2.1", "Недействительный токен", "Refresh token был отозван", "blacklist")]
        public async Task<IActionResult> Refresh()
        {
            try
            {
                // Устанавливаем имя сервиса и функции в заголовки сразу после старта activity
                string serviceName = _activitySource?.Name ?? "auth-service";
                string functionName = ControllerContext.ActionDescriptor.ActionName;
                using Activity? activity = _activitySource?.StartActivity($"{serviceName}.{functionName}", ActivityKind.Server);
                _logger.LogInformation("Начало операции {Operation} {Path}", functionName, Request.Path);

                _logger.LogInformation("Обновление токена: попытка; токен в cookie присутствует={HasCookie} ", Request.Cookies.ContainsKey("refreshToken"));
                string? refreshToken = null;
                // берём из cookie
                if (Request.Cookies.TryGetValue("refreshToken", out string? cookieValue) && !string.IsNullOrWhiteSpace(cookieValue))
                {
                    refreshToken = cookieValue;
                }

                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    _logger.LogWarning("Обновление токена: refresh-токен не предоставлен");
                    return Unauthorized(new ApiError
                    {
                        StatusCode = "2.4.0",
                        Title = "Недействительный запрос",
                        Message = "Refresh token не предоставлен",
                        Field = "refreshToken"
                    });
                }

                bool valid = _tokenService.ValidateToken(refreshToken, out Guid tokenUuid, out Guid userUuid, out IEnumerable<string> roles);
                if (!valid)
                {
                    _logger.LogWarning("Обновление токена: проверка токена не пройдена");
                    return Unauthorized(new ApiError
                    {
                        StatusCode = "2.2.2",
                        Title = "Недействительный токен",
                        Message = "Refresh token не прошёл проверку",
                        Field = "refreshToken"
                    });
                }

                if (await _refreshTokenBlackList.GetAsync(tokenUuid) != null)
                {
                    _logger.LogWarning("Обновление токена: токен в чёрном списке {TokenUuid}", tokenUuid);
                    return Unauthorized(new ApiError
                    {
                        StatusCode = "2.2.1",
                        Title = "Недействительный токен",
                        Message = "Refresh token был отозван",
                        Field = "blacklist"
                    });
                }

                // Генерируем новый access token и новый refresh token (ротация)
                Guid newAccessUuid = Guid.NewGuid();
                string newAccessToken = _tokenService.GenerateAccessToken(newAccessUuid, userUuid, roles);
                string newRefreshToken = _tokenService.GenerateRefreshToken(userUuid);

                Response.Headers.Append("Authorization", $"Bearer {newAccessToken}");
                Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict, Path = "/api/auth-service/v1/auth/refresh" });

                _logger.LogInformation("Обновление токена: выдан новый access-токен для пользователя {UserUuid}", userUuid);
                return Ok(new { accessToken = newAccessToken });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Refresh: unexpected error");
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