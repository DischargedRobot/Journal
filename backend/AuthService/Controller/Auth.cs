using AuthService.Redis;
using Microsoft.AspNetCore.Mvc;
using AuthService.Lib.Utils;
using Microsoft.EntityFrameworkCore;
using AuthService.Model;
using AuthService.Errors;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using Swashbuckle.AspNetCore.Annotations;
using AuthService.ResponseExample;
using AuthService.Model.Auth.Dto;

namespace AuthService.Controller
{
    [ApiController]
    [Route("api/auth-service/v1/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly ILogger<AuthController> _logger;
        private readonly AuthServiceContext _context;
        private readonly RedisRefreshTokenBlackList _refreshTokenBlackList;
        private readonly RedisAccessTokenList _accessTokenList;
        private readonly RedisAccessTokenBlackList _accessTokenBlackList;
        private readonly TokenService _tokenService;
        private readonly ActivitySource _activitySource;

        public AuthController(
            ILogger<AuthController> logger,
            AuthServiceContext context,
            RedisRefreshTokenBlackList refreshTokenBlackList,
            RedisAccessTokenList accessTokenList,
            RedisAccessTokenBlackList accessTokenBlackList,
            TokenService tokenService,
            ActivitySource activitySource)
        {
            _logger = logger;
            _context = context;
            _refreshTokenBlackList = refreshTokenBlackList;
            _accessTokenList = accessTokenList;
            _accessTokenBlackList = accessTokenBlackList;
            _tokenService = tokenService;
            _activitySource = activitySource;
        }

        [HttpPost("log-in")]
        [SwaggerOperation(Summary = "Вход в систему")]
        [SwaggerResponse(StatusCodes.Status200OK, "Успешная авторизация, возвращает access token")]
        [ResponseExample(StatusCodes.Status200OK, typeof(LoginResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Неавторизован", typeof(ApiError))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Внутренняя ошибка сервера", typeof(ApiError))]
        [ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.0", "Неверный запрос", "Логин или пароль не могут быть пустыми", "Login/Password")]
        [ApiErrorExample(StatusCodes.Status401Unauthorized, "1.2.3", "Неавторизован", "Пользователь с таким логином не найден или неверный пароль", "Login/Password")]
        [ApiErrorExample(StatusCodes.Status500InternalServerError, "1.0.0", "Внутренняя ошибка сервера", "Произошла ошибка на сервере", "server")]
        public async Task<ActionResult<LoginResponse>> Login(
            [FromBody]
            LoginRequest? request
        )
        {
            string functionName = ControllerContext.ActionDescriptor.ActionName;
            try
            {
                using Activity? activity = _activitySource.StartAndLog(_logger, this);
                _logger.LogInformation(
                    "{Function}: начало операции {Path} Login={Login}",
                    functionName,
                    Request.Path,
                    request?.Login
                );

                if (request == null || string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
                {
                    _logger.LogWarning(
                        "{Function}: неверные данные запроса при попытке входа. Login={Login}",
                        functionName,
                        request?.Login
                    );
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.0",
                        Title = "Неверный запрос",
                        Message = "Логин или пароль не могут быть пустыми",
                        Field = nameof(request.Login) + "/" + nameof(request.Password)
                    });
                }

                Users? user = await _context.Users
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.Login == request.Login);

                if (user == null || !HashingPassword.VerifyPassword(request.Password, user.PasswordHash))
                {
                    _logger.LogWarning("{Function}: неудачная попытка входа для логина {Login}",
                        functionName,
                        request.Login
                    );
                    return Unauthorized(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Неавторизован",
                        Message = "Пользователь с таким логином не найден или неверный пароль",
                        Field = nameof(request.Login) + "/" + nameof(request.Password)
                    });
                }


                // Генерируем и отправляй opaque токен, в редисе сохраняем access токен с ключём, который отправили в opaque
                _logger.LogInformation("{Function}: создание access токена для пользователя {UserUuid}", functionName, user.Uuid);
                Guid tokenUuid = Guid.NewGuid();
                string accessToken = _tokenService.GenerateAccessToken(
                    tokenUuid,
                    user.Uuid,
                    user.Roles?.Select(r => r.Name) ?? Enumerable.Empty<string>()
                );
                string opaqueToken = _tokenService.GenerateOpaqueToken(tokenUuid);
                _accessTokenList.SaveAsync(tokenUuid, accessToken, TimeSpan.FromMinutes(30)).Wait();
                Response.Headers.Append("Authorization", $"Bearer {opaqueToken}");

                _logger.LogInformation("{Function}: создание рефреш токена для пользователя {UserUuid}", functionName, user.Uuid);
                string refreshToken = _tokenService.GenerateRefreshToken(user.Uuid);
                Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddDays(7),
                    Path = "/api/auth-service/v1/auth/refresh"
                });
                Response.Cookies.Append("accessToken", opaqueToken, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddMinutes(30),
                    Path = "/api/"
                });

                _logger.LogInformation("{Function}: успешная авторизация для пользователя {UserUuid}", functionName, user.Uuid);
                return Ok(new LoginResponse { AccessToken = opaqueToken });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Function}: непредвиденная ошибка", functionName);
                return StatusCode(500, new ApiError
                {
                    StatusCode = "1.0.0",
                    Title = "Внутренняя ошибка сервера",
                    Message = "Произошла ошибка на сервере"
                });
            }
        }

        [HttpPost("log-out")]
        [SwaggerOperation(Summary = "Выход из системы")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Выход выполнен")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Внутренняя ошибка сервера", typeof(ApiError))]
        [ApiErrorExample(StatusCodes.Status500InternalServerError, "1.0.0", "Внутренняя ошибка сервера", "Произошла ошибка на сервере", "server")]
        public async Task<IActionResult> Logout()
        {
            string functionName = ControllerContext.ActionDescriptor.ActionName;
            try
            {
                using Activity? activity = _activitySource.StartAndLog(_logger, this);
                _logger.LogInformation("{Function}: начало операции {Path}", functionName, Request.Path);

                string? authHeader = Request.Headers["Authorization"]
                    .FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    string opaqueToken = authHeader.Substring("Bearer ".Length).Trim();
                    _logger.LogInformation("{Function}: проверка opaque-токена {OpaqueToken}", functionName, opaqueToken);
                    TokenService.TokenOpaqueValidationResult opaqueResult = await _tokenService.ValidateOpaqueTokenAsync(
                        opaqueToken,
                        _accessTokenList
                    );

                    if (opaqueResult.IsValid)
                    {
                        _logger.LogInformation("{Function}: проверка opaque-токена пройдена {OpaqueToken}", functionName, opaqueToken);
                    }
                    else
                    {
                        _logger.LogWarning("{Function}: проверка opaque-токена не пройдена {OpaqueToken}", functionName, opaqueToken);
                        return Unauthorized(new ApiError
                        {
                            StatusCode = "2.3.1",
                            Title = "Недействительный токен",
                            Message = "Access token не прошёл проверку",
                            Field = "Authorization"
                        });
                    }
                    string accessToken = opaqueResult.Token;
                    TokenService.TokenValidationResult resultCheckingAccessToken = await _tokenService.ValidateAccessTokenAsync(accessToken, _accessTokenBlackList);

                    if (resultCheckingAccessToken.IsValid)
                    {
                        await _accessTokenBlackList.SaveAsync(resultCheckingAccessToken.Payload.TokenUuid, resultCheckingAccessToken.Payload.UserUuid, TimeSpan.FromMinutes(30));
                        _logger.LogInformation(
                            "{Function}: access-токен добавлен в чёрный список tokenUuid={TokenUuid} userUuid={UserUuid}",
                            functionName,
                            resultCheckingAccessToken.Payload.TokenUuid,
                            resultCheckingAccessToken.Payload.UserUuid
                        );
                    }
                    else
                    {
                        _logger.LogError(
                            null,
                            "{Function}: ошибка обработки access-токена {Token}",
                            functionName,
                            accessToken
                        );
                        return Unauthorized(new ApiError
                        {
                            StatusCode = "2.3.1",
                            Title = "Недействительный токен",
                            Message = "Access token не прошёл проверку",
                            Field = "Authorization"
                        });
                    }

                    if (Request.Cookies.TryGetValue("refreshToken", out string? refreshToken) && !string.IsNullOrWhiteSpace(refreshToken))
                    {
                        _logger.LogInformation("{Function}: проверка refresh-токена {RefreshToken}", functionName, refreshToken);
                        TokenService.TokenValidationResult resultCheckingRefreshToken = await _tokenService.ValidateRefreshTokenAsync(
                            refreshToken,
                            _refreshTokenBlackList
                        );
                        if (resultCheckingRefreshToken.IsValid)
                        {
                            Guid tokenUuid = resultCheckingRefreshToken.Payload.TokenUuid;
                            Guid userUuid = resultCheckingRefreshToken.Payload.UserUuid;
                            await _refreshTokenBlackList.SaveAsync(tokenUuid, userUuid, TimeSpan.FromDays(7));
                            _logger.LogInformation(
                                "{Function}: refresh-токен добавлен в чёрный список tokenUuid={TokenUuid} userUuid={UserUuid}",
                                functionName,
                                tokenUuid,
                                userUuid
                            );
                        }
                        else
                        {
                            _logger.LogError(
                                null,
                                "{Function}: ошибка обработки refresh-токена {Token}",
                                functionName,
                                refreshToken
                            );
                            return Unauthorized(new ApiError
                            {
                                StatusCode = "2.4.1",
                                Title = "Недействительный токен",
                                Message = "Refresh token не прошёл проверку",
                                Field = "Authorization"
                            });
                        }
                    }

                    _logger.LogInformation("{Function}: завершена успешно", functionName);
                    return NoContent();
                }
                else
                {
                    _logger.LogWarning(
                        "{Function}: заголовок Authorization не предоставлен или имеет неверный формат",
                        functionName
                    );
                    return BadRequest(new ApiError
                    {
                        StatusCode = "2.3.0",
                        Title = "Неверный запрос",
                        Message = "Заголовок Authorization не предоставлен или имеет неверный формат",
                        Field = "Authorization"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Function}: непредвиденная ошибка", functionName);
                return StatusCode(500, new ApiError
                {
                    StatusCode = "1.0.0",
                    Title = "Внутренняя ошибка сервера",
                    Message = "Произошла ошибка на сервере"
                });
            }
        }

        [HttpPost("check-authtoken")]
        [SwaggerOperation(Summary = "Проверка access token")]
        [SwaggerResponse(StatusCodes.Status200OK, "Токен действителен")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Недействительный токен", typeof(ApiError))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Внутренняя ошибка сервера", typeof(ApiError))]
        [ApiErrorExample(StatusCodes.Status400BadRequest, "2.4.0", "Неверный запрос", "Заголовок Authorization не может быть пустым", "Authorization")]
        [ApiErrorExample(StatusCodes.Status400BadRequest, "2.4.2", "Неверный запрос", "Заголовок Authorization должен быть в формате 'Bearer {token}'", "Authorization")]
        [ApiErrorExample(StatusCodes.Status401Unauthorized, "2.2.2", "Недействительный токен", "Токен не прошёл проверку", "exp")]
        [ApiErrorExample(StatusCodes.Status401Unauthorized, "2.2.1", "Недействительный токен", "Токен был отозван", "blacklist")]
        [ApiErrorExample(StatusCodes.Status500InternalServerError, "1.0.0", "Внутренняя ошибка сервера", "Произошла ошибка на сервере", "server")]
        public async Task<IActionResult> CheckAuthtoken()
        {
            string functionName = ControllerContext.ActionDescriptor.ActionName;
            try
            {
                using Activity? activity = _activitySource.StartAndLog(_logger, this);

                string? authHeader = Request.Headers.Authorization.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(authHeader))
                {
                    _logger.LogWarning("{Function}: пустой заголовок Authorization", functionName);
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
                    _logger.LogWarning("{Function}: неверный формат заголовка Authorization", functionName);
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

                    // проверка пришедшего непрозрачного токена от клиента
                    TokenService.TokenOpaqueValidationResult opaqueResult = await _tokenService.ValidateOpaqueTokenAsync(
                        token,
                        _accessTokenList
                    );
                    if (!opaqueResult.IsValid)
                    {
                        _logger.LogWarning("{Function}: проверка opaque-токена не пройдена", functionName);
                        return Unauthorized(new ApiError
                        {
                            StatusCode = "2.2.2",
                            Title = "Недействительный токен",
                            Message = "Токен не прошёл проверку",
                            Field = "exp"
                        });
                    }

                    // проверка аccess токена который соответствует этому непрозрачному токену
                    TokenService.TokenValidationResult result = await _tokenService.ValidateAccessTokenAsync(opaqueResult.Token, _accessTokenBlackList);
                    if (!result.IsValid)
                    {
                        _logger.LogWarning("{Function}: проверка токена не пройдена", functionName);
                        return Unauthorized(new ApiError
                        {
                            StatusCode = "2.2.2",
                            Title = "Недействительный токен",
                            Message = "Токен не прошёл проверку",
                            Field = "exp"
                        });
                    }

                    if (await _accessTokenBlackList.GetAsync(result.Payload.TokenUuid) != null)
                    {
                        _logger.LogWarning("{Function}: токен был отозван", functionName);
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
                    _logger.LogInformation("{Function}: токен действителен tokenUuid={TokenUuid} userUuid={UserUuid}", functionName, result.Payload.TokenUuid, result.Payload.UserUuid);
                    return Ok(new { valid = true });
                }
                catch (SecurityTokenException ex)
                {
                    _logger.LogWarning(
                        ex,
                        "{Function}: ошибка валидации токена",
                        functionName
                    );
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
                _logger.LogError(ex, "{Function}: непредвиденная ошибка", functionName);
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
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Внутренняя ошибка сервера", typeof(ApiError))]
        [ApiErrorExample(StatusCodes.Status400BadRequest, "0.1.0", "Неверный запрос", "Тело запроса не может быть пустым", "BODY")]
        [ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.0", "Неверный запрос", "Логин или пароль не могут быть пустыми", "Login/Password")]
        [ApiErrorExample(StatusCodes.Status409Conflict, "1.1.1", "Конфликт", "Пользователь с таким логином уже существует", "Login")]
        [ApiErrorExample(StatusCodes.Status500InternalServerError, "1.0.0", "Внутренняя ошибка сервера", "Произошла ошибка на сервере", "server")]
        [SwaggerOperation(Summary = "Регистрация нового пользователя")]
        public async Task<IActionResult> Register([FromBody] UsersCreateDto? request)
        {
            string functionName = ControllerContext.ActionDescriptor.ActionName;
            try
            {
                using Activity? activity = _activitySource.StartAndLog(_logger, this);

                _logger.LogInformation("{Function}: попытка для логина {Login}", functionName, request?.Login);
                if (request == null)
                {
                    _logger.LogWarning("{Function}: тело запроса не может быть пустым при регистрации", functionName);
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.1.0",
                        Title = "Неверный запрос",
                        Message = "Тело запроса не может быть пустым",
                        Field = "BODY"
                    });
                }

                if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
                {
                    _logger.LogWarning("{Function}: пустой логин или пароль при регистрации", functionName);
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.0",
                        Title = "Неверный запрос",
                        Message = "Логин или пароль не могут быть пустыми",
                        Field = "Login/Password"
                    });
                }

                if (string.IsNullOrWhiteSpace(request.FirstName))
                {
                    _logger.LogWarning("{Function}: FirstName не предоставлено при регистрации Login={Login}", functionName, request.Login);
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.1",
                        Title = "Неверный запрос",
                        Message = "FirstName обязательна",
                        Field = "FirstName"
                    });
                }

                if (string.IsNullOrWhiteSpace(request.LastName))
                {
                    _logger.LogWarning(
                        "{Function}: LastName не предоставлено при регистрации Login={Login}",
                        functionName,
                        request.Login
                    );
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.1",
                        Title = "Неверный запрос",
                        Message = "LastName обязательна",
                        Field = "LastName"
                    });
                }

                bool exists = await _context.Users.AnyAsync(u => u.Login == request.Login);
                if (exists)
                {
                    _logger.LogWarning("{Function}: логин уже существует {Login}", functionName, request.Login);
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
                    PasswordHash = HashingPassword.ComputeHash(request.Password),
                    Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
                    FirstName = request.FirstName.Trim(),
                    LastName = request.LastName.Trim(),
                    Patronymic = string.IsNullOrWhiteSpace(request.Patronymic) ? null : request.Patronymic.Trim(),
                    TokenVersion = 0,
                    Roles = []
                };

                if (request.RolesUuid != null)
                {
                    Guid[] roleUuids = request.RolesUuid.Distinct().ToArray();
                    List<Roles> foundRoles = await _context.Roles.Where(r => roleUuids.Contains(r.Uuid)).ToListAsync();
                    if (foundRoles.Count != roleUuids.Length)
                    {
                        Guid[] missing = roleUuids.Except(foundRoles.Select(r => r.Uuid)).ToArray();
                        _logger.LogWarning(
                            "{Function}: указаны несуществующие роли {Missing}",
                            functionName,
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
                    _logger.LogInformation("{Function}: назначены роли пользователю {UserUuid} Roles={Roles}", functionName, user.Uuid, string.Join(",", foundRoles.Select(r => r.Name)));
                }

                _logger.LogInformation("{Function}: сохраняем пользователя Login={Login} userUuid={UserUuid}", functionName, user.Login, user.Uuid);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Выдаём токены сразу после регистрации
                // Генерируем и отправляй opaque токен, 
                // в редисе сохраняем access токен с ключём, который отправили в opaque
                _logger.LogInformation("{Function}: создание access токена для пользователя {UserUuid}", functionName, user.Uuid);
                Guid tokenUuid = Guid.NewGuid();
                string accessToken = _tokenService.GenerateAccessToken(
                    tokenUuid,
                    user.Uuid,
                    user.Roles?.Select(r => r.Name) ?? Enumerable.Empty<string>()
                );
                string opaqueToken = _tokenService.GenerateOpaqueToken(tokenUuid);
                _accessTokenList.SaveAsync(tokenUuid, accessToken, TimeSpan.FromMinutes(30)).Wait();
                Response.Headers.Append("Authorization", $"Bearer {opaqueToken}");

                _logger.LogInformation("{Function}: создание рефреш токена для пользователя {UserUuid}", functionName, user.Uuid);
                string refreshToken = _tokenService.GenerateRefreshToken(user.Uuid);
                Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddDays(7),
                    Path = "/api/auth-service/v1/auth/refresh"
                });

                _logger.LogInformation("{Function}: пользователь создан {UserUuid}", functionName, user.Uuid);
                return Created(string.Empty, new UsersResponseDto(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Function}: непредвиденная ошибка", functionName);
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
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Внутренняя ошибка сервера", typeof(ApiError))]
        [ApiErrorExample(StatusCodes.Status401Unauthorized, "2.4.0", "Недействительный запрос", "Refresh token не предоставлен", "refreshToken")]
        [ApiErrorExample(StatusCodes.Status401Unauthorized, "2.2.2", "Недействительный токен", "Refresh token не прошёл проверку", "refreshToken")]
        [ApiErrorExample(StatusCodes.Status401Unauthorized, "2.2.1", "Недействительный токен", "Refresh token был отозван", "blacklist")]
        [ApiErrorExample(StatusCodes.Status500InternalServerError, "1.0.0", "Внутренняя ошибка сервера", "Произошла ошибка на сервере", "server")]
        public async Task<IActionResult> Refresh()
        {
            string functionName = ControllerContext.ActionDescriptor.ActionName;
            try
            {
                // Устанавливаем имя сервиса и функции в заголовки сразу после старта activity
                using Activity? activity = _activitySource.StartAndLog(_logger, this);

                _logger.LogInformation("{Function}: начало проверки рефреш токена из cookie", functionName);
                string? refreshToken = null;
                // берём из cookie
                if (Request.Cookies.TryGetValue("refreshToken", out string? cookieValue) && !string.IsNullOrWhiteSpace(cookieValue))
                {
                    _logger.LogInformation("{Function}: Рефреш токен присутствует={HasCookie}", functionName, Request.Cookies.ContainsKey("refreshToken"));
                    refreshToken = cookieValue;
                }

                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    _logger.LogWarning("{Function}: refresh-токен не предоставлен", functionName);
                    return Unauthorized(new ApiError
                    {
                        StatusCode = "2.4.0",
                        Title = "Недействительный запрос",
                        Message = "Refresh token не предоставлен",
                        Field = "refreshToken"
                    });
                }

                TokenService.TokenValidationResult result = await _tokenService.ValidateRefreshTokenAsync(
                    refreshToken,
                    _refreshTokenBlackList
                );
                if (!result.IsValid)
                {
                    _logger.LogWarning("{Function}: проверка токена не пройдена", functionName);
                    return Unauthorized(new ApiError
                    {
                        StatusCode = "2.4.1",
                        Title = "Недействительный токен",
                        Message = "Refresh token не прошёл проверку",
                        Field = "refreshToken"
                    });
                }

                if (await _refreshTokenBlackList.GetAsync(result.Payload.TokenUuid) != null)
                {
                    _logger.LogWarning("{Function}: токен в чёрном списке {TokenUuid}", functionName, result.Payload.TokenUuid);
                    return Unauthorized(new ApiError
                    {
                        StatusCode = "2.4.1",
                        Title = "Недействительный токен",
                        Message = "Refresh token был отозван",
                        Field = "blacklist"
                    });
                }

                Guid userUuid = result.Payload.UserUuid;
                IEnumerable<string> roles = result.Payload.Roles;
                // Генерируем новый access token и новый refresh token (ротация)
                Guid tokenUuid = Guid.NewGuid();
                string accessToken = _tokenService.GenerateAccessToken(
                    tokenUuid,
                    userUuid,
                    roles
                );
                string opaqueToken = _tokenService.GenerateOpaqueToken(tokenUuid);
                _accessTokenList.SaveAsync(tokenUuid, accessToken, TimeSpan.FromMinutes(30)).Wait();
                Response.Headers.Append("Authorization", $"Bearer {opaqueToken}");

                _logger.LogInformation("{Function}: создание рефреш токена для пользователя {UserUuid}", functionName, userUuid);
                string newRefreshToken = _tokenService.GenerateRefreshToken(userUuid);
                Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddDays(7),
                    Path = "/api/auth-service/v1/auth/refresh"
                });
                _logger.LogInformation("{Function}: выдан новый access-токен для пользователя {UserUuid}", functionName, userUuid);
                return Ok(new { accessToken = opaqueToken });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Function}: unexpected error", functionName);
                return StatusCode(500, new ApiError
                {
                    StatusCode = "1.0.0",
                    Title = "Внутренняя ошибка сервера",
                    Message = "Произошла ошибка на сервере"
                });
            }
        }
    }
}