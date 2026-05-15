using MainService.Errors;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using MainService.Enums;

namespace MainService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MarksController : ControllerBase
    {
        private readonly MainServiceContext _context;

        public MarksController(MainServiceContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Оценки найдены", typeof(PagedResult<MarksResponseDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Оценки не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить список оценок"
        )]
        public async Task<ActionResult<PagedResult<MarksResponseDto>>> GetMarks(
            [FromQuery, SwaggerParameter("Количество записей")]
            int size = 50,
            [FromQuery, SwaggerParameter("Смещение от начала списка")]
            int offset = 0,
            [FromQuery, SwaggerParameter("Фильтр по значению")]
            string? filterValue = null,
            [FromQuery, SwaggerParameter("Фильтр по UUID типа оценки")]
            Guid? markTypeUuid = null,
            [FromQuery, SwaggerParameter("Порядок сортировки по значению")]
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

            MarkTypes? markType = null;
            if (markTypeUuid != null)
            {
                if (markTypeUuid == Guid.Empty)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.0",
                        Title = "Неверный запрос",
                        Message = "UUID типа оценки не может быть пустым",
                        Field = nameof(markTypeUuid)
                    });
                }

                markType = await _context.MarkTypes.FirstOrDefaultAsync(mt => mt.Uuid == markTypeUuid.Value);
                if (markType == null)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Некорректные данные",
                        Message = $"Тип оценки с UUID \"{markTypeUuid}\" не найден",
                        Field = nameof(markTypeUuid)
                    });
                }
            }

            IQueryable<Marks> baseQuery = _context.Marks
                .AsNoTracking()
                .Where(m => (filterValue == null || m.Value.Contains(filterValue))
                    && (markType == null || m.MarkTypeId == markType.MarkTypeId));
            Task<int> totalRecord = baseQuery.CountAsync();

            List<MarksResponseDto> items = await baseQuery
                .SortByKey(m => m.Value, sortOrder)
                .TakeWithOffset(offset, size)
                .Select(m => new MarksResponseDto
                {
                    Uuid = m.Uuid,
                    Value = m.Value,
                    MarkTypeUuid = markType != null ? markType.Uuid : m.MarkType!.Uuid,
                    Version = m.Version
                })
                .ToListAsync();
            int total = await totalRecord;

            if (total == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Оценки не найдены",
                    Message = "В системе не найдено ни одной оценки",
                    Field = string.Empty
                });
            }

            if (items.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Оценки не найдены",
                    Message = "В системе не найдено ни одной оценки для указанных параметров запроса",
                    Field = string.Empty
                });
            }

            return Ok(new PagedResult<MarksResponseDto>(total, offset, items.Count, items));
        }

        [HttpGet("type/{markTypeUuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Оценки найдены", typeof(PagedResult<MarksResponseDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Оценки не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Получить оценки по UUID типа оценки"
        )]
        public async Task<ActionResult<PagedResult<MarksResponseDto>>> GetMarksByType(
            [SwaggerParameter("UUID типа оценки")]
            Guid markTypeUuid,
            [FromQuery, SwaggerParameter("Смещение от начала списка")]
            int offset = 0,
            [FromQuery, SwaggerParameter("Количество записей")]
            int size = 50,
            [FromQuery, SwaggerParameter("Порядок сортировки по значению")]
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

            if (markTypeUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID типа оценки не может быть пустым",
                    Field = nameof(markTypeUuid)
                });
            }

            MarkTypes? markType = await _context.MarkTypes.FirstOrDefaultAsync(mt => mt.Uuid == markTypeUuid);
            if (markType == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Тип оценки не найден",
                    Message = $"Тип оценки с UUID \"{markTypeUuid}\" не найден",
                    Field = nameof(markTypeUuid)
                });
            }

            IQueryable<Marks> baseQuery = _context.Marks
                .AsNoTracking()
                .Where(m => m.MarkTypeId == markType.MarkTypeId);

            Task<int> totalRecord = baseQuery.CountAsync();

            List<MarksResponseDto> items = await baseQuery
                .SortByKey(m => m.Value, sortOrder)
                .TakeWithOffset(offset, size)
                .Select(m => new MarksResponseDto
                {
                    Uuid = m.Uuid,
                    Value = m.Value,
                    MarkTypeUuid = markType.Uuid,
                    Version = m.Version
                })
                .ToListAsync();

            int total = await totalRecord;

            if (total == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Оценки не найдены",
                    Message = "В системе не найдено ни одной оценки для указанного типа",
                    Field = string.Empty
                });
            }

            if (items.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Оценки не найдены",
                    Message = "В системе не найдено ни одной оценки для указанных параметров запроса",
                    Field = string.Empty
                });
            }

            return Ok(new PagedResult<MarksResponseDto>(
                Total: total,
                Offset: offset,
                Size: items.Count,
                Items: items
            ));
        }

        [HttpGet("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Оценка найдена", typeof(MarksResponseDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Оценка не найдена", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(Summary = "Получить оценку по UUID")]
        public async Task<ActionResult<MarksResponseDto>> GetMark(
            [SwaggerParameter("UUID оценки")]
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

            Marks? mark = await _context.Marks
                .Include(m => m.MarkType)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Uuid == uuid);

            if (mark == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Оценка не найдена",
                    Message = $"Оценка с UUID \"{uuid}\" не найдена",
                    Field = nameof(uuid)
                });
            }

            return Ok(new MarksResponseDto(mark));
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, "Оценка создана", typeof(MarksResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Некорректные данные", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Конфликт", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status409Conflict, typeof(ApiError409ConflictExample))]
        [SwaggerOperation(
            Summary = "Создать новую оценку"
        )]
        public async Task<ActionResult<MarksResponseDto>> CreateMark([
            FromBody, SwaggerParameter("Данные новой оценки")] MarksCreateDto createDto)
        {
            if (createDto == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.1.0",
                    Title = "Неверный запрос",
                    Message = "Тело запроса не может быть пустым",
                    Field = "BODY"
                });
            }

            if (string.IsNullOrWhiteSpace(createDto.Value))
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "Значение оценки не может быть пустым",
                    Field = nameof(createDto.Value)
                });
            }

            if (createDto.MarkTypeUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID типа оценки не может быть пустым",
                    Field = nameof(createDto.MarkTypeUuid)
                });
            }

            MarkTypes? markType = await _context.MarkTypes.FirstOrDefaultAsync(mt => mt.Uuid == createDto.MarkTypeUuid);
            if (markType == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Некорректные данные",
                    Message = "Тип оценки с указанным UUID не найден",
                    Field = nameof(createDto.MarkTypeUuid)
                });
            }

            bool isExists = await _context.Marks.AnyAsync(m => m.Value == createDto.Value.Trim() && m.MarkTypeId == markType.MarkTypeId);
            if (isExists)
            {
                return Conflict(new ApiError
                {
                    StatusCode = "1.2.1",
                    Title = "Неверный запрос",
                    Message = "Оценка с таким значением и типом уже существует",
                    Field = nameof(createDto.Value)
                });
            }

            Marks newMark = new()
            {
                Uuid = Guid.NewGuid(),
                Value = createDto.Value.Trim(),
                MarkTypeId = markType.MarkTypeId
            };

            _context.Marks.Add(newMark);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetMark),
                new { uuid = newMark.Uuid },
                new MarksResponseDto
                {
                    Uuid = newMark.Uuid,
                    Value = newMark.Value,
                    MarkTypeUuid = markType.Uuid,
                    Version = newMark.Version
                }
            );
        }

        [HttpDelete("{uuid}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Оценка удалена")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Оценка не найдена", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(Summary = "Удалить оценку по UUID")]
        public async Task<IActionResult> DeleteMark([SwaggerParameter("UUID оценки")] Guid uuid)
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

            Marks? mark = await _context.Marks.FirstOrDefaultAsync(m => m.Uuid == uuid);
            if (mark == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Оценка не найдена",
                    Message = $"Оценка с UUID \"{uuid}\" не найдена",
                    Field = nameof(uuid)
                });
            }

            _context.Marks.Remove(mark);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Оценка обновлена", typeof(MarksResponseDto))]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Конфликт", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status409Conflict, typeof(ApiError409ConflictExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Оценка не найдена", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(Summary = "Обновить оценку", Description = "Обновляет оценку по её UUID. Поля: Value, MarkTypeUuid, Version (для контроля версий)")]
        public async Task<ActionResult<MarksResponseDto>> UpdateMark(
            [SwaggerParameter("UUID оценки")] Guid uuid,
            [FromBody, SwaggerParameter("Данные для обновления")] MarksRequestDto updateDto
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

            if (updateDto == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.1.0",
                    Title = "Неверный запрос",
                    Message = "Тело запроса не может быть пустым",
                    Field = "BODY"
                });
            }

            if (string.IsNullOrWhiteSpace(updateDto.Value))
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "Значение оценки не может быть пустым",
                    Field = nameof(updateDto.Value)
                });
            }

            Marks? mark = await _context.Marks.FirstOrDefaultAsync(m => m.Uuid == uuid);
            if (mark == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Оценка не найдена",
                    Message = $"Оценка с UUID \"{uuid}\" не найдена",
                    Field = nameof(uuid)
                });
            }

            // проверка версии
            if (updateDto.Version != mark.Version)
            {
                return Conflict(new ApiError
                {
                    StatusCode = "1.2.2",
                    Title = "Несоответствие версии",
                    Message = "Версия ресурса изменилась. Обновите ресурс и повторите запрос.",
                    Field = nameof(updateDto.Version)
                });
            }

            // проверка типа оценки
            if (updateDto.MarkTypeUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID типа оценки не может быть пустым",
                    Field = nameof(updateDto.MarkTypeUuid)
                });
            }

            MarkTypes? markType = await _context.MarkTypes.FirstOrDefaultAsync(mt => mt.Uuid == updateDto.MarkTypeUuid);
            if (markType == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Некорректные данные",
                    Message = "Тип оценки с указанным UUID не найден",
                    Field = nameof(updateDto.MarkTypeUuid)
                });
            }

            // проверка уникальности (Value + MarkType)
            bool duplicate = await _context.Marks.AnyAsync(m => m.Value == updateDto.Value.Trim()
                && m.MarkTypeId == markType.MarkTypeId
                && m.Uuid != uuid);
            if (duplicate)
            {
                return Conflict(new ApiError
                {
                    StatusCode = "1.2.1",
                    Title = "Неверный запрос",
                    Message = "Оценка с таким значением и типом уже существует",
                    Field = nameof(updateDto.Value)
                });
            }

            mark.Value = updateDto.Value.Trim();
            mark.MarkTypeId = markType.MarkTypeId;

            await _context.SaveChangesAsync();

            return Ok(new MarksResponseDto
            {
                Uuid = mark.Uuid,
                Value = mark.Value,
                MarkTypeUuid = markType.Uuid,
                Version = mark.Version
            });
        }
    }
}