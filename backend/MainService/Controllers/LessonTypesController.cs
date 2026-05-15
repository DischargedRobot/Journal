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
    public class LessonTypesController : ControllerBase
    {
        private readonly MainServiceContext _context;

        public LessonTypesController(MainServiceContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Типы занятий найдены", typeof(PagedResult<LessonTypesResponseDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Типы занятий не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить список всех типов занятий",
            Description = "Возвращает список всех типов занятий в системе"
        )]
        public async Task<ActionResult<PagedResult<LessonTypesResponseDto>>> GetLessonTypes(
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
            IQueryable<LessonTypes> baseQuery = _context.LessonTypes
                .Where(lt => filterName == null || lt.Name.Contains(filterName))
                .AsNoTracking();

            Task<int> totalRecord = baseQuery.CountAsync();

            List<LessonTypesResponseDto> items = await baseQuery
                .SortByKey(lt => lt.Name, sortOrder)
                .TakeWithOffset(offset, size)
                .Select(lt => new LessonTypesResponseDto(lt))
                .ToListAsync();

            int total = await totalRecord;

            if (total == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Типы занятий не найдены",
                    Message = "В системе не найдено ни одного типа занятий",
                    Field = string.Empty
                });
            }

            if (items.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Типы занятий не найдены",
                    Message = "В системе не найдено ни одного типа занятий для указанных параметров запроса",
                    Field = string.Empty
                });
            }

            return Ok(new PagedResult<LessonTypesResponseDto>(
                Total: total,
                Offset: offset,
                Size: items.Count,
                Items: items
            ));
        }

        [HttpGet("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Тип занятия найден", typeof(LessonTypesResponseDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Тип занятия не найден", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Получить тип занятия по UUID",
            Description = "Возвращает тип занятия по указанному UUID"
        )]
        public async Task<ActionResult<LessonTypesResponseDto>> GetLessonType(
            [SwaggerParameter("UUID типа занятия")]
            Guid uuid
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID типа занятия не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            LessonTypes? lessonType = await _context.LessonTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(lt => lt.Uuid == uuid);

            if (lessonType == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Тип занятия не найден",
                    Message = $"Тип занятия с UUID \"{uuid}\" не найден",
                    Field = nameof(uuid)
                });
            }

            return Ok(new LessonTypesResponseDto(lessonType));
        }
    }
}
