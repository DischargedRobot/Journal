using MainService.Enums;
using MainService.Errors;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace MainService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AttestationTypesController : ControllerBase
    {
        private readonly MainServiceContext _context;

        public AttestationTypesController(MainServiceContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Типы аттестаций найдены", typeof(PagedResult<AttestationTypesResponseDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Типы аттестаций не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить список всех типов аттестаций",
            Description = "Возвращает список всех типов аттестаций в системе"
        )]
        public async Task<ActionResult<PagedResult<AttestationTypesResponseDto>>> GetAttestationTypes(
            [FromQuery, SwaggerParameter("Количество записей")]
            int size = 50,
            [FromQuery, SwaggerParameter("Смещение от начала списка")]
            int offset = 0,
            [FromQuery, SwaggerParameter("Фильтр по названию")]
            string? filterName = null,
            [FromQuery, SwaggerParameter("Порядок сортировки по имени")]
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

            IQueryable<AttestationTypes> baseQuery = _context.AttestationTypes
                .Where(at => filterName == null || at.Name.Contains(filterName))
                .AsNoTracking();

            Task<int> totalRecord = baseQuery.CountAsync();

            List<AttestationTypesResponseDto> items = await baseQuery
                .SortByKey(at => at.Name, sortOrder)
                .TakeWithOffset(offset, size)
                .Select(at => new AttestationTypesResponseDto(at))
                .ToListAsync();

            int total = await totalRecord;

            if (total == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Типы аттестаций не найдены",
                    Message = "В системе не найдено ни одного типа аттестаций",
                    Field = string.Empty
                });
            }

            if (items.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Типы аттестаций не найдены",
                    Message = "В системе не найдено ни одного типа аттестаций для указанных параметров запроса",
                    Field = string.Empty
                });
            }

            return Ok(new PagedResult<AttestationTypesResponseDto>(
                Total: total,
                Offset: offset,
                Size: items.Count,
                Items: items
            ));
        }

        [HttpGet("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Тип аттестации найден", typeof(AttestationTypesResponseDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Тип аттестации не найден", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Получить тип аттестации по UUID",
            Description = "Возвращает тип аттестации по указанному UUID"
        )]
        public async Task<ActionResult<AttestationTypesResponseDto>> GetAttestationType(
            [SwaggerParameter("UUID типа аттестации")]
            Guid uuid
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID типа аттестации не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            AttestationTypes? attestationType = await _context.AttestationTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(at => at.Uuid == uuid);

            if (attestationType == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Тип аттестации не найден",
                    Message = $"Тип аттестации с UUID \"{uuid}\" не найден",
                    Field = nameof(uuid)
                });
            }

            return Ok(new AttestationTypesResponseDto(attestationType));
        }
    }
}
