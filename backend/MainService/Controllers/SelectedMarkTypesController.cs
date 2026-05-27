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
    public class SelectedMarkTypesController : ControllerBase
    {
        private readonly MainServiceContext _context;

        public SelectedMarkTypesController(MainServiceContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Найден список выбранных типов оценок", typeof(PagedResult<SelectedMarkTypesResponseDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Данные не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(Summary = "Получить список выбранных типов оценок")]
        public async Task<ActionResult<PagedResult<SelectedMarkTypesResponseDto>>> GetSelectedMarkTypes(
            [FromQuery, SwaggerParameter("UUID типа занятия (lesson type)")]
            Guid? lessonTypeUuid = null,
            [FromQuery, SwaggerParameter("UUID типа оценки (mark type)")]
            Guid? markTypeUuid = null,
            [FromQuery, SwaggerParameter("UUID дисциплины")]
            Guid? disciplineUuid = null,
            [FromQuery, SwaggerParameter("Количество записей")]
            int size = 50,
            [FromQuery, SwaggerParameter("Смещение от начала списка")]
            int offset = 0,
            [FromQuery, SwaggerParameter("Порядок сортировки")]
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

            IQueryable<SelectedMarkTypes> baseQuery = _context.SelectedMarkTypes
                .Where(s => (lessonTypeUuid == null || s.LessonType!.Uuid == lessonTypeUuid)
                            && (markTypeUuid == null || s.MarkType!.Uuid == markTypeUuid)
                            && (disciplineUuid == null || s.Disciplines!.Uuid == disciplineUuid))
                .AsNoTracking();

            Task<int> totalRecord = baseQuery.CountAsync();

            List<SelectedMarkTypesResponseDto> items = await baseQuery
                .SortByKey(s => s.LessonTypeId, sortOrder)
                .TakeWithOffset(offset, size)
                .Select(s => new SelectedMarkTypesResponseDto
                {
                    LessonTypeUuid = s.LessonType!.Uuid,
                    MarkTypeUuid = s.MarkType!.Uuid,
                    DisciplineUuid = s.Disciplines!.Uuid,
                    Version = s.Version
                })
                .ToListAsync();

            int total = await totalRecord;

            if (total == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Данные не найдены",
                    Message = "В системе не найдено ни одной записи SelectedMarkTypes",
                    Field = string.Empty
                });
            }

            if (items.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.1.3",
                    Title = "Данные не найдены",
                    Message = "Ни одной записи не найдено для указанных параметров запроса",
                    Field = "BODY"
                });
            }

            return Ok(new PagedResult<SelectedMarkTypesResponseDto>(total, offset, items.Count, items));
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, "SelectedMarkTypes создан", typeof(SelectedMarkTypesResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Некорректные данные", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(Summary = "Создать запись SelectedMarkTypes")]
        public async Task<ActionResult<SelectedMarkTypesResponseDto>> CreateSelectedMarkTypes(
            [FromBody, SwaggerParameter("Данные для создания")] SelectedMarkTypesCreateDto createDto
        )
        {
            if (createDto.LessonTypeUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID LessonType не может быть пустым",
                    Field = nameof(createDto.LessonTypeUuid)
                });
            }

            LessonTypes? lessonType = await _context.LessonTypes.FirstOrDefaultAsync(l => l.Uuid == createDto.LessonTypeUuid);
            if (lessonType == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Некорректные данные",
                    Message = "LessonType с указанным UUID не найден",
                    Field = nameof(createDto.LessonTypeUuid)
                });
            }

            if (createDto.MarkTypeUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID MarkType не может быть пустым",
                    Field = nameof(createDto.MarkTypeUuid)
                });
            }
            MarkTypes? markType = await _context.MarkTypes.FirstOrDefaultAsync(m => m.Uuid == createDto.MarkTypeUuid);
            if (markType == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Некорректные данные",
                    Message = "MarkType с указанным UUID не найден",
                    Field = nameof(createDto.MarkTypeUuid)
                });
            }

            if (createDto.DisciplineUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID Discipline не может быть пустым",
                    Field = nameof(createDto.DisciplineUuid)
                });
            }
            Disciplines? discipline = await _context.Disciplines.FirstOrDefaultAsync(d => d.Uuid == createDto.DisciplineUuid);
            if (discipline == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Некорректные данные",
                    Message = "Discipline с указанным UUID не найден",
                    Field = nameof(createDto.DisciplineUuid)
                });

            }

            // Проверка на существующую запись с такими же ключами
            SelectedMarkTypes? exists = await _context.SelectedMarkTypes.FirstOrDefaultAsync(s =>
                s.LessonTypeId == lessonType.LessonTypeId
                && s.MarkTypeId == markType.MarkTypeId
                && s.DisciplineId == discipline.DisciplineId
            );

            if (exists != null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "1.1.4",
                    Title = "Конфликт",
                    Message = "Запись с такими ключами уже существует",
                    Field = "BODY"
                });
            }

            SelectedMarkTypes newItem = new()
            {
                LessonTypeId = lessonType.LessonTypeId,
                MarkTypeId = markType.MarkTypeId,
                DisciplineId = discipline.DisciplineId
            };

            _context.SelectedMarkTypes.Add(newItem);
            await _context.SaveChangesAsync();

            SelectedMarkTypesResponseDto response = new SelectedMarkTypesResponseDto(newItem);

            return CreatedAtAction(nameof(GetSelectedMarkTypes), null, response);
        }

        [HttpDelete]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Запись удалена")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Запись не найдена", typeof(ApiError))]
        [SwaggerOperation(Summary = "Удалить запись SelectedMarkTypes по ключам")]
        public async Task<IActionResult> DeleteSelectedMarkTypes(
            [FromQuery, SwaggerParameter("UUID типа занятия (lesson type)")]
            Guid lessonTypeUuid,
            [FromQuery, SwaggerParameter("UUID типа оценки (mark type)")]
            Guid? markTypeUuid = null
        )
        {
            if (lessonTypeUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID LessonType не может быть пустым",
                    Field = nameof(lessonTypeUuid)
                });
            }

            LessonTypes? lessonType = await _context.LessonTypes.FirstOrDefaultAsync(l => l.Uuid == lessonTypeUuid);
            if (lessonType == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Не найдено",
                    Message = "LessonType с указанным UUID не найден",
                    Field = nameof(lessonTypeUuid)
                });
            }

            MarkTypes? markType = null;
            if (markTypeUuid != null && markTypeUuid != Guid.Empty)
            {
                markType = await _context.MarkTypes.FirstOrDefaultAsync(m => m.Uuid == markTypeUuid);
                if (markType == null)
                {
                    return NotFound(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Не найдено",
                        Message = "MarkType с указанным UUID не найден",
                        Field = nameof(markTypeUuid)
                    });
                }
            }

            SelectedMarkTypes? item = await _context.SelectedMarkTypes.FirstOrDefaultAsync(s =>
                s.LessonTypeId == lessonType.LessonTypeId
                && s.MarkTypeId == (markType != null ? markType.MarkTypeId : null)
            );

            if (item == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Не найдено",
                    Message = "Запись SelectedMarkTypes не найдена",
                    Field = "BODY"
                });
            }

            _context.SelectedMarkTypes.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
