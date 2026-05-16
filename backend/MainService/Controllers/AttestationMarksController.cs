using MainService.Errors;
using MainService.Enums;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace MainService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class AttestationMarksController : ControllerBase
    {
        private readonly MainServiceContext _context;

        public AttestationMarksController(MainServiceContext context)
        {
            _context = context;
        }


        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Оценки аттестаций найдены", typeof(PagedResult<AttestationMarksResponseDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Оценки аттестаций не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить список оценок аттестаций",
            Description = "Возвращает список оценок аттестаций в системе"
        )]
        public async Task<ActionResult<PagedResult<AttestationMarksResponseDto>>> GetAttestationMarks(
            [FromQuery, SwaggerParameter("Количество записей")]
            int size = 50,
            [FromQuery, SwaggerParameter("Смещение от начала списка")]
            int offset = 0,
            [FromQuery, SwaggerParameter("Фильтр по значению оценки")]
            string? filterMark = null,
            [FromQuery, SwaggerParameter("Порядок сортировки по значению оценки")]
            SortOrder sortOrder = SortOrder.Ascending
        )
        {
            if (offset < 0)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.1",
                    Title = "Неверный запрос",
                    Message = "Параметр offset не может быть отрицательным",
                    Field = nameof(offset)
                });
            }

            if (size < 0)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.1",
                    Title = "Неверный запрос",
                    Message = "Параметр size не может быть отрицательным",
                    Field = nameof(size)
                });
            }

            IQueryable<AttestationMarks> baseQuery = _context.AttestationMarks
                .Where(am => filterMark == null || am.Mark.Contains(filterMark))
                .Include(am => am.AttestationType)
                .AsNoTracking();

            Task<int> totalRecord = baseQuery.CountAsync();

            List<AttestationMarksResponseDto> items = await baseQuery
                .SortByKey(am => am.Mark, sortOrder)
                .TakeWithOffset(offset, size)
                .Select(am => new AttestationMarksResponseDto(am))
                .ToListAsync();

            int total = await totalRecord;

            if (total == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Оценки аттестаций не найдены",
                    Message = "В системе не найдено ни одной оценки аттестации",
                    Field = string.Empty
                });
            }

            if (items.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.1.3",
                    Title = "Оценки аттестаций не найдены",
                    Message = "В системе не найдено ни одной оценки аттестации для указанных параметров запроса",
                    Field = "BODY"
                });
            }

            return Ok(new PagedResult<AttestationMarksResponseDto>(
                Total: total,
                Offset: offset,
                Size: items.Count,
                Items: items
            ));
        }


        [HttpGet("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Оценка аттестации найдена", typeof(AttestationMarksResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [ApiErrorExample(400,
           "0.2.0",
            "Неверный запрос",
            "UUID оценки аттестации не может быть пустым",
            "uuid"
        )]
        [ApiErrorExample(400,
           "0.2.0",
            "Неверный запрос",
            "UUID оценки аттестации не может быть пустым",
            "uuidы"
        )]
        [SwaggerOperation(
            Summary = "Получить оценку аттестации по UUID",
            Description = "Возвращает оценку аттестации по указанному UUID"
        )]
        public async Task<ActionResult<AttestationMarksResponseDto>> GetAttestationMark(
            [SwaggerParameter("UUID оценки аттестации")]
            Guid uuid
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID оценки аттестации не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            AttestationMarks? attestationMark = await _context.AttestationMarks
                .Include(am => am.AttestationType)
                .AsNoTracking()
                .FirstOrDefaultAsync(am => am.Uuid == uuid);

            if (attestationMark == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Оценка аттестации не найдена",
                    Message = $"Оценка аттестации с UUID \"{uuid}\" не найдена",
                    Field = nameof(uuid)
                });
            }

            return Ok(new AttestationMarksResponseDto(attestationMark));
        }
    }
}
