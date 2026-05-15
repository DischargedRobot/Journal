using MainService.Errors;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace MainService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PresenceStatusesController : ControllerBase
    {
        private readonly MainServiceContext _context;

        public PresenceStatusesController(MainServiceContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Статусы присутствия найдены", typeof(IEnumerable<PresenceStatusesResponseDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Статусы присутствия не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить список всех статусов присутствия",
            Description = "Возвращает список всех статусов присутствия в системе"
        )]
        public async Task<ActionResult<IEnumerable<PresenceStatusesResponseDto>>> GetPresenceStatuses()
        {
            List<PresenceStatuses> statuses = await _context.PresenceStatuses
                .Include(ps => ps.LessonPresences)
                .AsNoTracking()
                .ToListAsync();

            if (statuses.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Статусы присутствия не найдены",
                    Message = "В системе не найдено ни одного статуса присутствия",
                    Field = string.Empty
                });
            }

            return Ok(statuses.Select(s => new PresenceStatusesResponseDto(s)));
        }

        [HttpGet("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Статус присутствия найден", typeof(PresenceStatusesResponseDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Статус присутствия не найден", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Получить статус присутствия по UUID",
            Description = "Возвращает статус присутствия по указанному UUID"
        )]
        public async Task<ActionResult<PresenceStatusesResponseDto>> GetPresenceStatus(
            [SwaggerParameter("UUID статуса присутствия")]
            Guid uuid
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID статуса присутствия не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            PresenceStatuses? status = await _context.PresenceStatuses
                .Include(ps => ps.LessonPresences)
                .AsNoTracking()
                .FirstOrDefaultAsync(ps => ps.Uuid == uuid);

            if (status == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Статус присутствия не найден",
                    Message = $"Статус присутствия с UUID \"{uuid}\" не найден",
                    Field = nameof(uuid)
                });
            }

            return Ok(new PresenceStatusesResponseDto(status));
        }
    }
}
