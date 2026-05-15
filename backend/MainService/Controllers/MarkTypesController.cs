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
    public class MarkTypesController : ControllerBase
    {
        private readonly MainServiceContext _context;

        public MarkTypesController(MainServiceContext context)
        {
            _context = context;
        }


        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Типы оценок найдены", typeof(PagedResult<MarkTypesResponseDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Типы оценок не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(Summary = "Получить список типов оценок")]
        public async Task<ActionResult<PagedResult<MarkTypesResponseDto>>> GetMarkTypes(
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
            IQueryable<MarkTypes> baseQuery = _context.MarkTypes
                .Where(m => filterName == null || m.Name.Contains(filterName))
                .AsNoTracking();

            Task<int> totalTask = baseQuery.CountAsync();

            List<MarkTypesResponseDto> items = await baseQuery
                .SortByKey(m => m.Name, sortOrder)
                .TakeWithOffset(offset, size)
                .Select(m => new MarkTypesResponseDto(m))
                .ToListAsync();

            int total = await totalTask;

            if (total == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Типы оценок не найдены",
                    Message = "В системе не найдено ни одного типа оценки",
                    Field = string.Empty
                });
            }

            if (items.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Типы оценок не найдены",
                    Message = "В системе не найдено ни одного типа оценки для указанных параметров запроса",
                    Field = string.Empty
                });
            }

            return Ok(new PagedResult<MarkTypesResponseDto>(total, offset, items.Count, items));
        }


        [HttpGet("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Тип оценки найден", typeof(MarkTypesResponseDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Тип оценки не найден", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Получить тип оценки по UUID"
        )]
        public async Task<ActionResult<MarkTypesResponseDto>> GetMarkType(
            [SwaggerParameter("UUID типа оценки")]
            Guid uuid
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            MarkTypes? markType = await _context.MarkTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Uuid == uuid);

            if (markType == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Тип оценки не найден",
                    Message = $"Тип оценки с UUID \"{uuid}\" не найден",
                    Field = nameof(uuid)
                });
            }

            return Ok(new MarkTypesResponseDto(markType));
        }


        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, "Тип оценки создан", typeof(MarkTypesResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Некорректные данные", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Создать новый тип оценки"
        )]
        public async Task<ActionResult<MarkTypesResponseDto>> CreateMarkType(
            [FromBody, SwaggerParameter("Данные нового типа оценки")]
            MarkTypesCreateDto createDto
        )
        {
            if (string.IsNullOrWhiteSpace(createDto.Name))
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "Название не может быть пустым",
                    Field = nameof(createDto.Name)
                });
            }

            MarkTypes newEntity = new()
            {
                Uuid = Guid.NewGuid(),
                Name = createDto.Name.Trim(),
            };

            _context.MarkTypes.Add(newEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetMarkType),
                new { uuid = newEntity.Uuid },
                new MarkTypesResponseDto(newEntity)
            );
        }


        [HttpDelete("{uuid}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Тип оценки удалён")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Тип оценки не найден", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Удалить тип оценки по UUID"
        )]
        public async Task<IActionResult> DeleteMarkType(
            [SwaggerParameter("UUID типа оценки")]
            Guid uuid
            )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            MarkTypes? entity = await _context.MarkTypes.FirstOrDefaultAsync(m => m.Uuid == uuid);
            if (entity == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Тип оценки не найден",
                    Message = $"Тип оценки с UUID \"{uuid}\" не найден",
                    Field = nameof(uuid)
                });
            }

            _context.MarkTypes.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Тип оценки обновлён", typeof(MarkTypesResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Тип оценки не найден", typeof(ApiError))]
        [SwaggerOperation(Summary = "Частично обновить тип оценки")]
        public async Task<ActionResult<MarkTypesResponseDto>> UpdateMarkType(
            [SwaggerParameter("UUID типа оценки")] Guid uuid,
            [FromBody, SwaggerParameter("Данные для обновления")] MarkTypesUpdateDto updateDto)
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            MarkTypes? entity = await _context.MarkTypes.FirstOrDefaultAsync(m => m.Uuid == uuid);
            if (entity == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Тип оценки не найден",
                    Message = $"MarkType with UUID \"{uuid}\" not found",
                    Field = nameof(uuid)
                });
            }

            if (updateDto.Name != null)
            {
                if (updateDto.Name.Trim() == string.Empty)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.0",
                        Title = "Неверный запрос",
                        Message = "Название не может быть пустым",
                        Field = nameof(updateDto.Name)
                    });
                }

                entity.Name = updateDto.Name.Trim();
            }

            await _context.SaveChangesAsync();

            return Ok(new MarkTypesResponseDto(entity));
        }
    }
}
