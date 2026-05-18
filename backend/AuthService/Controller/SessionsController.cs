using AuthService.Errors;
using AuthService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace AuthService.Controller
{
    [ApiController]
    [Route("api/auth-service/v1/[controller]")]
    [Produces("application/json")]
    public class SessionsController : ControllerBase
    {
        private readonly AuthServiceContext _context;

        public SessionsController(AuthServiceContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Сессии найдены", typeof(PagedResult<SessionsResponseDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerOperation(Summary = "Получить список сессий (опционально фильтр по пользователю)")]
        public async Task<ActionResult<PagedResult<SessionsResponseDto>>> GetSessions(
            [FromQuery] int size = 100,
            [FromQuery] int offset = 0,
            [FromQuery] Guid? userUuid = null
        )
        {
            if (offset < 0)
            {
                return BadRequest(new ApiError("0.2.1", "Неверный запрос", "Параметр offset не может быть отрицательным", nameof(offset)));
            }

            if (size < 0)
            {
                return BadRequest(new ApiError("0.2.1", "Неверный запрос", "Параметр size не может быть отрицательным", nameof(size)));
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
                return NotFound(new ApiError("1.0.3", "Сессии не найдены", "В системе не найдено ни одной сессии", string.Empty));
            }
            if (items.Count == 0)
            {
                return NotFound(new ApiError("1.1.3", "Сессии не найдены", "В системе не найдено ни одной сессии для указанных параметров запроса", "BODY"));
            }

            PagedResult<SessionsResponseDto> result = new(
                Total: total,
                Offset: offset,
                Size: items.Count,
                Items: items
            );

            return Ok(result);
        }

        [HttpGet("{refreshTokenUuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Сессия найдена", typeof(SessionsResponseDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Сессия не найдена", typeof(ApiError))]
        [SwaggerOperation(Summary = "Получить сессию по UUID refresh-токена")]
        public async Task<IActionResult> GetSession(Guid refreshTokenUuid)
        {
            if (refreshTokenUuid == Guid.Empty)
                return BadRequest(new ApiError("0.2.1", "Неверный запрос", "UUID не может быть пустым", nameof(refreshTokenUuid)));

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
                return NotFound(new ApiError("1.2.3", "Сессия не найдена", "Сессия с указанным UUID refresh-токена не найдена", nameof(refreshTokenUuid)));

            return Ok(session);
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, "Сессия создана", typeof(SessionsResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Некорректные данные", typeof(ApiError))]
        [SwaggerOperation(Summary = "Создать новую сессию")]
        public async Task<IActionResult> CreateSession([FromBody] SessionsCreateDto createDto)
        {
            if (createDto == null)
            {
                return BadRequest(new ApiError(
                    "0.2.1",
                    "Неверный запрос",
                    "Неверный формат данных",
                    "BODY"));
            }

            if (createDto.UserUuid == Guid.Empty)
            {
                return BadRequest(new ApiError(
                    "0.2.1",
                    "Неверный запрос",
                    "UserUuid обязателен",
                    nameof(createDto.UserUuid)));
            }

            if (createDto.RefreshTokenUuid == Guid.Empty)
            {
                return BadRequest(new ApiError(
                    "0.2.1",
                    "Неверный запрос",
                    "RefreshTokenUuid обязателен",
                    nameof(createDto.RefreshTokenUuid)));
            }
            
			Users? user = await _context.Users.FirstOrDefaultAsync(u => u.Uuid == createDto.UserUuid);
            if (user == null)
                return BadRequest(new ApiError("0.2.1", "Неверный запрос", "Пользователь не найден", nameof(createDto.UserUuid)));

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

            return CreatedAtAction(nameof(GetSession), new { refreshTokenUuid = session.RefreshTokenUuid }, response);
        }

        [HttpPatch("{refreshTokenUuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Сессия обновлена", typeof(SessionsResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Сессия не найдена", typeof(ApiError))]
        [SwaggerOperation(Summary = "Частично обновить сессию по UUID refresh-токена")]
        public async Task<IActionResult> UpdateSession(Guid refreshTokenUuid, [FromBody] SessionsUpdateDto? request)
        {
            if (refreshTokenUuid == Guid.Empty)
                return BadRequest(new ApiError("0.2.1", "Неверный запрос", "UUID не может быть пустым", nameof(refreshTokenUuid)));

            if (request == null)
                return BadRequest(new ApiError("0.2.1", "Неверный запрос", "Неверный формат данных", "BODY"));

            Sessions? session = await _context.Sessions.FirstOrDefaultAsync(s => s.RefreshTokenUuid == refreshTokenUuid);
            if (session == null)
                return NotFound(new ApiError("1.2.3", "Сессия не найдена", "Сессия с указанным UUID refresh-токена не найдена", nameof(refreshTokenUuid)));

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

            return Ok(resp);
        }

        [HttpDelete("{refreshTokenUuid}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Сессия удалена")]
        [SwaggerOperation(Summary = "Удалить сессию по UUID refresh-токена")]
        public async Task<IActionResult> DeleteSession(Guid refreshTokenUuid)
        {
            if (refreshTokenUuid == Guid.Empty)
                return BadRequest(new ApiError("0.2.1", "Неверный запрос", "UUID не может быть пустым", nameof(refreshTokenUuid)));

            Sessions? session = await _context.Sessions.FirstOrDefaultAsync(s => s.RefreshTokenUuid == refreshTokenUuid);
            if (session == null)
                return NotFound(new ApiError("1.2.3", "Сессия не найдена", "Сессия с указанным UUID refresh-токена не найдена", nameof(refreshTokenUuid)));

            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
