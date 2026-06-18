using AuthService.Errors;
using AuthService.Model;
using AuthService.ResponseExample;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Annotations;

using System.Diagnostics;

namespace AuthService.Controller
{
    [ApiController]
    [Route("auth-service/v1/[controller]")]
    [Produces("application/json")]
    public class SessionsController : ControllerBase
    {
        private readonly AuthServiceContext _context;
        private readonly ILogger<SessionsController> _logger;
        private readonly ActivitySource _activitySource;

        public SessionsController(AuthServiceContext context, ILogger<SessionsController> logger, ActivitySource activitySource)
        {
            _context = context;
            _logger = logger;
            _activitySource = activitySource;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Сессии найдены", typeof(PagedResult<SessionsResponseDto>))]
        [ResponseExample(StatusCodes.Status200OK, typeof(PagedResult<SessionsResponseDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "Параметр offset не может быть отрицательным", nameof(offset))]
        [ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "Параметр size не может быть отрицательным", nameof(size))]
        [ApiErrorExample(StatusCodes.Status404NotFound, "1.0.3", "Сессии не найдены", "В системе не найдено ни одной сессии", "")]
        [ApiErrorExample(StatusCodes.Status404NotFound, "1.1.3", "Сессии не найдены", "В системе не найдено ни одной сессии для указанных параметров запроса", "BODY")]
        [SwaggerOperation(Summary = "Получить список сессий (опционально фильтр по пользователю)")]
        public async Task<ActionResult<PagedResult<SessionsResponseDto>>> GetSessions(
            [FromQuery] int size = 100,
            [FromQuery] int offset = 0,
            [FromQuery] Guid? userUuid = null
        )
        {
            string functionName = ControllerContext.ActionDescriptor.ActionName;
            using Activity? activity = _activitySource.StartActivity(functionName);
            _logger.LogInformation("{Function}: вызвано size={Size} offset={Offset} userUuid={UserUuid}", functionName, size, offset, userUuid);
            try
            {

                if (offset < 0)
                {
                    _logger.LogWarning("{Function}: параметр offset отрицательный: {Offset}", functionName, offset);
                    return BadRequest(new ApiError(
                        "0.2.1",
                        "Неверный запрос",
                        "Параметр offset не может быть отрицательным",
                        nameof(offset)
                    ));
                }

                if (size < 0)
                {
                    _logger.LogWarning("{Function}: параметр size отрицательный: {Size}", functionName, size);
                    return BadRequest(new ApiError(
                        "0.2.1",
                        "Неверный запрос",
                        "Параметр size не может быть отрицательным",
                        nameof(size)
                    ));
                }

                IQueryable<Sessions> baseQuery = _context.Sessions.Include(s => s.User).AsNoTracking();

                if (userUuid != null)
                {
                    baseQuery = baseQuery.Where(s => s.User != null && s.User.Uuid == userUuid);
                }

                Task<int> totalRecord = baseQuery.CountAsync();

                List<SessionsResponseDto> items = await baseQuery
                    .OrderByDescending(s => s.CreatedAt)
                    .Skip(offset)
                    .Take(size)
                    .Select(s => new SessionsResponseDto
                    {
                        SessionId = s.SessionId,
                        CreatedAt = s.CreatedAt,
                        ExpiresAt = s.ExpiresAt,
                        RefreshTokenUuid = s.RefreshTokenUuid,
                        UserId = s.UserId,
                        UserUuid = s.User != null ? s.User.Uuid : Guid.Empty,
                        UserAgent = s.UserAgent,
                        BrowserName = s.BrowserName,
                        BrowserVersion = s.BrowserVersion,
                        OsName = s.OsName,
                    })
                    .ToListAsync();

                int total = await totalRecord;

                if (total == 0)
                {
                    _logger.LogInformation("{Function}: всего записей = 0", functionName);
                    return NotFound(new ApiError(
                        "1.0.3",
                        "Сессии не найдены",
                        "В системе не найдено ни одной сессии",
                        string.Empty
                    ));
                }
                if (items.Count == 0)
                {
                    _logger.LogInformation("{Function}: для запроса не найдено записей", functionName);
                    return NotFound(new ApiError(
                        "1.1.3",
                        "Сессии не найдены",
                        "В системе не найдено ни одной сессии для указанных параметров запроса",
                        "BODY"
                    ));
                }

                PagedResult<SessionsResponseDto> result = new(
                    Total: total,
                    Offset: offset,
                    Size: items.Count,
                    Items: items
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Function}: неожиданная ошибка при получении списка сессий", functionName);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
                {
                    StatusCode = "1.0.0",
                    Title = "Внутренняя ошибка сервера",
                    Message = "Произошла ошибка на сервере",
                });
            }
        }

        [HttpGet("{refreshTokenUuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Сессия найдена", typeof(SessionsResponseDto))]
        [ResponseExample(StatusCodes.Status200OK, typeof(SessionsResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "UUID не может быть пустым", nameof(refreshTokenUuid))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Сессия не найдена", typeof(ApiError))]
        [ApiErrorExample(StatusCodes.Status404NotFound, "1.2.3", "Сессия не найдена", "Сессия с указанным UUID refresh-токена не найдена", nameof(refreshTokenUuid))]
        [SwaggerOperation(Summary = "Получить сессию по UUID refresh-токена")]
        public async Task<IActionResult> GetSession(Guid refreshTokenUuid)
        {
            string functionName = ControllerContext.ActionDescriptor.ActionName;
            using Activity? activity = _activitySource.StartActivity(functionName);
            _logger.LogInformation("{Function}: вызвано refreshTokenUuid={Refresh}", functionName, refreshTokenUuid);
            try
            {
                if (refreshTokenUuid == Guid.Empty)
                {
                    _logger.LogWarning("{Function}: пустой refreshTokenUuid", functionName);
                    return BadRequest(new ApiError("0.2.1", "Неверный запрос", "UUID не может быть пустым", nameof(refreshTokenUuid)));
                }

                var session = await _context.Sessions
                    .Include(s => s.User)
                    .Where(s => s.RefreshTokenUuid == refreshTokenUuid)
                    .Select(s => new SessionsResponseDto
                    {
                        SessionId = s.SessionId,
                        CreatedAt = s.CreatedAt,
                        ExpiresAt = s.ExpiresAt,
                        RefreshTokenUuid = s.RefreshTokenUuid,
                        UserId = s.UserId,
                        UserUuid = s.User != null ? s.User.Uuid : Guid.Empty,
                        UserAgent = s.UserAgent,
                        BrowserName = s.BrowserName,
                        BrowserVersion = s.BrowserVersion,
                        OsName = s.OsName,
                    })
                    .FirstOrDefaultAsync();

                if (session == null)
                {
                    _logger.LogInformation("{Function}: сессия не найдена refreshTokenUuid={Refresh}", functionName, refreshTokenUuid);
                    return NotFound(new ApiError(
                        "1.2.3",
                        "Сессия не найдена",
                        "Сессия с указанным UUID refresh-токена не найдена",
                        nameof(refreshTokenUuid)
                        ));
                }

                _logger.LogInformation("{Function}: возвращена сессия refreshTokenUuid={Refresh}", functionName, refreshTokenUuid);
                return Ok(session);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Function}: неожиданная ошибка при получении сессии", functionName);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
                {
                    StatusCode = "1.0.0",
                    Title = "Внутренняя ошибка сервера",
                    Message = "Произошла ошибка на сервере",
                });
            }
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, "Сессия создана", typeof(SessionsResponseDto))]
        [ResponseExample(StatusCodes.Status201Created, typeof(SessionsResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Некорректные данные", typeof(ApiError))]
        [ApiErrorExample(StatusCodes.Status400BadRequest, "0.1.0", "Неверный запрос", "Тело запроса не может быть пустым", "BODY")]
        [ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "UserUuid обязателен", nameof(SessionsCreateDto.UserUuid))]
        [ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "RefreshTokenUuid обязателен", nameof(SessionsCreateDto.RefreshTokenUuid))]
        [ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "Пользователь не найден", nameof(SessionsCreateDto.UserUuid))]
        [SwaggerOperation(Summary = "Создать новую сессию")]
        public async Task<IActionResult> CreateSession(
            [FromBody, SwaggerParameter("Тело запроса: данные для создания сессии")]
            SessionsCreateDto createDto)
        {
            string functionName = ControllerContext.ActionDescriptor.ActionName;
            using Activity? activity = _activitySource.StartActivity(functionName);
            _logger.LogInformation("{Function}: вызвано", functionName);
            try
            {
                if (createDto == null)
                {
                    _logger.LogWarning("{Function}: пустой createDto", functionName);
                    return BadRequest(new ApiError(
                        "0.1.0",
                        "Неверный запрос",
                        "Тело запроса не может быть пустым",
                        "BODY"));
                }

                if (createDto.UserUuid == Guid.Empty)
                {
                    _logger.LogWarning("{Function}: UserUuid пустой", functionName);
                    return BadRequest(new ApiError(
                        "0.2.1",
                        "Неверный запрос",
                        "UserUuid обязателен",
                        nameof(createDto.UserUuid)));
                }

                if (createDto.RefreshTokenUuid == Guid.Empty)
                {
                    _logger.LogWarning("{Function}: RefreshTokenUuid пустой", functionName);
                    return BadRequest(new ApiError(
                        "0.2.1",
                        "Неверный запрос",
                        "RefreshTokenUuid обязателен",
                        nameof(createDto.RefreshTokenUuid)));
                }

                Users? user = await _context.Users.FirstOrDefaultAsync(u => u.Uuid == createDto.UserUuid);
                if (user == null)
                {
                    _logger.LogWarning("{Function}: пользователь не найден {UserUuid}", functionName, createDto.UserUuid);
                    return BadRequest(new ApiError(
                        "0.2.1",
                        "Неверный запрос",
                        "Пользователь не найден",
                        nameof(createDto.UserUuid)));
                }

                Sessions session = new()
                {
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = createDto.ExpiresAt,
                    RefreshTokenUuid = createDto.RefreshTokenUuid,
                    UserId = user.UserId,
                    UserAgent = createDto.UserAgent,
                    BrowserName = createDto.BrowserName,
                    BrowserVersion = createDto.BrowserVersion,
                    OsName = createDto.OsName,
                };

                _context.Sessions.Add(session);
                await _context.SaveChangesAsync();

                var response = new SessionsResponseDto
                {
                    SessionId = session.SessionId,
                    CreatedAt = session.CreatedAt,
                    ExpiresAt = session.ExpiresAt,
                    RefreshTokenUuid = session.RefreshTokenUuid,
                    UserId = session.UserId,
                    UserUuid = user.Uuid,
                    UserAgent = session.UserAgent,
                    BrowserName = session.BrowserName,
                    BrowserVersion = session.BrowserVersion,
                    OsName = session.OsName,
                };

                _logger.LogInformation("{Function}: создана сессия {response}", functionName, response);
                return CreatedAtAction(nameof(GetSession), new { refreshTokenUuid = session.RefreshTokenUuid }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Function}: неожиданная ошибка при создании сессии", functionName);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
                {
                    StatusCode = "1.0.0",
                    Title = "Внутренняя ошибка сервера",
                    Message = "Произошла ошибка на сервере",
                });
            }
        }

        [HttpPatch("{refreshTokenUuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Сессия обновлена", typeof(SessionsResponseDto))]
        [ResponseExample(StatusCodes.Status200OK, typeof(SessionsResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "UUID не может быть пустым", nameof(refreshTokenUuid))]
        [ApiErrorExample(StatusCodes.Status400BadRequest, "0.1.0", "Неверный запрос", "Тело запроса не может быть пустым", "BODY")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Сессия не найдена", typeof(ApiError))]
        [ApiErrorExample(StatusCodes.Status404NotFound, "1.2.3", "Сессия не найдена", "Сессия с указанным UUID refresh-токена не найдена", nameof(refreshTokenUuid))]
        [SwaggerOperation(Summary = "Частично обновить сессию по UUID refresh-токена")]
        public async Task<IActionResult> UpdateSession(Guid refreshTokenUuid, [FromBody] SessionsUpdateDto? request)
        {
            string functionName = ControllerContext.ActionDescriptor.ActionName;
            using Activity? activity = _activitySource.StartActivity(functionName);
            _logger.LogInformation("{Function}: вызвано refreshTokenUuid={Refresh}", functionName, refreshTokenUuid);
            try
            {
                if (refreshTokenUuid == Guid.Empty)
                {
                    _logger.LogWarning("{Function}: пустой refreshTokenUuid", functionName);
                    return BadRequest(new ApiError(
                        "0.2.1",
                        "Неверный запрос",
                        "UUID не может быть пустым",
                        nameof(refreshTokenUuid)
                    ));
                }

                if (request == null)
                {
                    _logger.LogWarning("{Function}: пустой request", functionName);
                    return BadRequest(new ApiError(
                        "0.1.0",
                        "Неверный запрос",
                        "Тело запроса не может быть пустым",
                        "BODY"
                    ));
                }

                Sessions? session = await _context.Sessions.FirstOrDefaultAsync(s => s.RefreshTokenUuid == refreshTokenUuid);
                if (session == null)
                {
                    _logger.LogInformation("{Function}: сессия не найдена {Refresh}", functionName, refreshTokenUuid);
                    return NotFound(new ApiError(
                        "1.2.3",
                        "Сессия не найдена",
                        "Сессия с указанным UUID refresh-токена не найдена",
                        nameof(refreshTokenUuid)
                    ));
                }

                if (request.ExpiresAt != null)
                    session.ExpiresAt = request.ExpiresAt.Value;
                if (request.UserAgent != null)
                    session.UserAgent = request.UserAgent;
                if (request.BrowserName != null)
                    session.BrowserName = request.BrowserName;
                if (request.BrowserVersion != null)
                    session.BrowserVersion = request.BrowserVersion;
                if (request.OsName != null)
                    session.OsName = request.OsName;

                await _context.SaveChangesAsync();

                var resp = new SessionsResponseDto
                {
                    SessionId = session.SessionId,
                    CreatedAt = session.CreatedAt,
                    ExpiresAt = session.ExpiresAt,
                    RefreshTokenUuid = session.RefreshTokenUuid,
                    UserId = session.UserId,
                    UserUuid = session.User != null ? session.User.Uuid : Guid.Empty,
                    UserAgent = session.UserAgent,
                    BrowserName = session.BrowserName,
                    BrowserVersion = session.BrowserVersion,
                    OsName = session.OsName,
                };

                _logger.LogInformation("{Function}: обновлена сессия refreshTokenUuid={Refresh}", functionName, refreshTokenUuid);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Function}: неожиданная ошибка при обновлении сессии", functionName);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
                {
                    StatusCode = "1.0.0",
                    Title = "Внутренняя ошибка сервера",
                    Message = "Произошла ошибка на сервере",
                });
            }
        }

        [HttpDelete("{refreshTokenUuid}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Сессия удалена")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "UUID не может быть пустым", nameof(refreshTokenUuid))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Сессия не найдена", typeof(ApiError))]
        [ApiErrorExample(StatusCodes.Status404NotFound, "1.2.3", "Сессия не найдена", "Сессия с указанным UUID refresh-токена не найдена", nameof(refreshTokenUuid))]
        [SwaggerOperation(Summary = "Удалить сессию по UUID refresh-токена")]
        public async Task<IActionResult> DeleteSession(Guid refreshTokenUuid)
        {
            string functionName = ControllerContext.ActionDescriptor.ActionName;
            using Activity? activity = _activitySource.StartActivity(functionName);
            _logger.LogInformation("{Function}: вызвано refreshTokenUuid={Refresh}", functionName, refreshTokenUuid);
            try
            {
                if (refreshTokenUuid == Guid.Empty)
                {
                    _logger.LogWarning("{Function}: пустой refreshTokenUuid", functionName);
                    return BadRequest(new ApiError(
                        "0.2.1", "Неверный запрос",
                        "UUID не может быть пустым",
                        nameof(refreshTokenUuid))
                        );
                }

                Sessions? session = await _context.Sessions.FirstOrDefaultAsync(s => s.RefreshTokenUuid == refreshTokenUuid);
                if (session == null)
                {
                    _logger.LogInformation("{Function}: сессия не найдена {Refresh}", functionName, refreshTokenUuid);
                    return NotFound(new ApiError(
                        "1.2.3",
                        "Сессия не найдена",
                        "Сессия с указанным UUID refresh-токена не найдена",
                        nameof(refreshTokenUuid))
                    );
                }

                _context.Sessions.Remove(session);
                await _context.SaveChangesAsync();

                _logger.LogInformation("{Function}: удалена сессия refreshTokenUuid={Refresh}", functionName, refreshTokenUuid);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Function}: неожиданная ошибка при удалении сессии", functionName);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
                {
                    StatusCode = "1.0.0",
                    Title = "Внутренняя ошибка сервера",
                    Message = "Произошла ошибка на сервере",
                });
            }
        }
    }
}
