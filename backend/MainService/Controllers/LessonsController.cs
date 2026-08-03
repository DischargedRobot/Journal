using MainService.Enums;
using MainService.Errors;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace MainService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LessonsController : ControllerBase
    {
        private readonly MainServiceContext _context;

        public LessonsController(MainServiceContext context)
        {
            _context = context;
        }


        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Занятия найдены", typeof(PagedResult<LessonsResponseDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Занятия не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить список занятий",
            Description = "Возвращает список занятий по заданным параметрам"
        )]
        public async Task<ActionResult<PagedResult<LessonsResponseDto>>> GetLessons(
            [FromQuery, SwaggerParameter("Количество записей")]
            int size = 50,
            [FromQuery, SwaggerParameter("Смещение от начала списка")]
            int offset = 0,
            [FromQuery, SwaggerParameter("UUID типа занятия")]
            Guid? lessonTypeUuid = null,
            [FromQuery, SwaggerParameter("Начальная дата занятий (включительно)")]
            DateTime? startDate = null,
            [FromQuery, SwaggerParameter("Конечная дата занятий (включительно)")]
            DateTime? endDate = null,
            [FromQuery, SwaggerParameter("Порядок сортировки по дате занятий")]
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

            if (startDate.HasValue && startDate == DateTime.MinValue)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.1",
                    Title = "Неверный запрос",
                    Message = "Параметр startDate некорректен",
                    Field = nameof(startDate)
                });
            }

            if (endDate.HasValue && endDate == DateTime.MinValue)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.1",
                    Title = "Неверный запрос",
                    Message = "Параметр endDate некорректен",
                    Field = nameof(endDate)
                });
            }

            LessonTypes? lessonType = null;
            if (lessonTypeUuid.HasValue)
            {
                if (lessonTypeUuid == Guid.Empty)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.1",
                        Title = "Неверный запрос",
                        Message = "Параметр lessonTypeUuid некорректен",
                        Field = nameof(lessonTypeUuid)
                    });
                }

                lessonType = await _context.LessonTypes.FirstOrDefaultAsync(lt => lt.Uuid == lessonTypeUuid.Value);
                if (lessonType == null)
                {
                    return NotFound(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Тип занятия не найден",
                        Message = $"Тип занятия с UUID \"{lessonTypeUuid}\" не найден",
                        Field = nameof(lessonTypeUuid)
                    });
                }
            }

            IQueryable<Lessons> baseQuery = _context.Lessons
                .Include(l => l.LessonType)
                .Include(l => l.Discipline)
                .Where(l => lessonType == null || l.LessonType!.Uuid == lessonTypeUuid)
                .AsNoTracking();

            if (startDate.HasValue)
            {
                baseQuery = baseQuery.Where(l => l.StartDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                baseQuery = baseQuery.Where(l => l.StartDate <= endDate.Value);
            }

            int total = await baseQuery.CountAsync();

            List<LessonsResponseDto> items = await baseQuery
                .SortByKey(l => l.StartDate, sortOrder)
                .TakeWithOffset(offset, size)
                .Select(l => new LessonsResponseDto
                {
                    Uuid = l.Uuid,
                    Code = l.Code,
                    StartDate = l.StartDate,
                    Name = l.Name,
                    ShortName = l.ShortName,
                    LessonTypeUuid = l.LessonType!.Uuid,
                    DisciplineUuid = l.Discipline!.Uuid,
                    Version = l.Version
                })
                .ToListAsync();

            if (total == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Занятия не найдены",
                    Message = "В системе не найдено ни одного занятия",
                    Field = string.Empty
                });
            }

            if (items.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.1.3",
                    Title = "Занятия не найдены",
                    Message = "В системе не найдено ни одного занятия для указанных параметров запроса",
                    Field = "BODY"
                });
            }

            return Ok(new PagedResult<LessonsResponseDto>(
                Total: total,
                Offset: offset,
                Size: items.Count,
                Items: items
            ));
        }


        [HttpGet("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Занятие найдено", typeof(LessonsResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Занятие не найдено", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить занятие по UUID",
            Description = "Возвращает занятие по указанному UUID"
        )]
        public async Task<ActionResult<LessonsResponseDto>> GetLesson(
            [SwaggerParameter("UUID занятия")]
            Guid uuid
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID занятия не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            LessonsResponseDto? lesson = await _context.Lessons
                .Where(l => l.Uuid == uuid)
                .Select(l => new LessonsResponseDto
                {
                    Uuid = l.Uuid,
                    Code = l.Code,
                    StartDate = l.StartDate,
                    Name = l.Name,
                    ShortName = l.ShortName,
                    LessonTypeUuid = l.LessonType!.Uuid,
                    DisciplineUuid = l.Discipline!.Uuid,
                    Version = l.Version
                })
                .FirstOrDefaultAsync();

            if (lesson == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Занятие не найдено",
                    Message = $"Занятие с UUID \"{uuid}\" не найдено",
                    Field = nameof(uuid)
                });
            }

            return Ok(lesson);
        }


        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, "Занятие создано", typeof(LessonsResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Некорректные данные", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Создать новое занятие",
            Description = "Создаёт новое занятие"
        )]
        public async Task<ActionResult<LessonsResponseDto>> CreateLesson(
            [FromBody, SwaggerParameter("Данные нового занятия")]
            LessonsRequestDto createDto
        )
        {
            if (createDto.Code <= 0)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.1",
                    Title = "Неверный запрос",
                    Message = "Код занятия должен быть положительным",
                    Field = nameof(createDto.Code)
                });
            }

            if (createDto.StartDate == DateTime.MinValue)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.1",
                    Title = "Неверный запрос",
                    Message = "Дата начала занятия некорректна",
                    Field = nameof(createDto.StartDate)
                });
            }

            if (createDto.Name != null && createDto.Name.Trim() == string.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "Название занятия не может быть пустым",
                    Field = nameof(createDto.Name)
                });
            }

            if (createDto.LessonTypeUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID типа занятия не может быть пустым",
                    Field = nameof(createDto.LessonTypeUuid)
                });
            }

            if (createDto.DisciplineUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID дисциплины не может быть пустым",
                    Field = nameof(createDto.DisciplineUuid)
                });
            }

            LessonTypes? lessonType = await _context.LessonTypes.FirstOrDefaultAsync(lt => lt.Uuid == createDto.LessonTypeUuid);
            if (lessonType == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Тип занятия не найден",
                    Message = $"Тип занятия с UUID \"{createDto.LessonTypeUuid}\" не найден",
                    Field = nameof(createDto.LessonTypeUuid)
                });
            }

            Disciplines? discipline = await _context.Disciplines.FirstOrDefaultAsync(d => d.Uuid == createDto.DisciplineUuid);
            if (discipline == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Дисциплина не найдена",
                    Message = $"Дисциплина с UUID \"{createDto.DisciplineUuid}\" не найдена",
                    Field = nameof(createDto.DisciplineUuid)
                });
            }

            Lessons lesson = new()
            {
                Code = createDto.Code,
                StartDate = createDto.StartDate,
                Name = createDto.Name?.Trim(),
                ShortName = string.IsNullOrWhiteSpace(createDto.ShortName)
                    ? (string.IsNullOrWhiteSpace(createDto.Name)
                        ? string.Empty
                        : string.Concat(createDto.Name
                            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                            .Select(w => w[0])))
                    : createDto.ShortName.Trim(),
                LessonTypeId = lessonType.LessonTypeId,
                DisciplineId = discipline.DisciplineId
            };

            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetLesson),
                new { uuid = lesson.Uuid },
                new LessonsResponseDto(lesson)
            );
        }


        [HttpDelete("{uuid}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Занятие удалено")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Занятие не найдено", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(Summary = "Удалить занятие по UUID")]
        public async Task<IActionResult> DeleteLesson(
            [SwaggerParameter("UUID занятия")]
            Guid uuid)
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID занятия не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            Lessons? lesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Uuid == uuid);
            if (lesson == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Занятие не найдено",
                    Message = $"Занятие с UUID \"{uuid}\" не найдено",
                    Field = nameof(uuid)
                });
            }

            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPatch("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Занятие обновлено", typeof(LessonsResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Занятие не найдено", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(Summary = "Обновить занятие", Description = "Обновляет поля занятия по UUID. Все поля необязательны")]
        public async Task<ActionResult<LessonsResponseDto>> UpdateLesson(
            [SwaggerParameter("UUID занятия")]
            Guid uuid,
            [FromBody, SwaggerParameter("Данные для обновления")]
            LessonsUpdateDto updateDto
        )
        {
            // проверки перед запросом к бд
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID занятия не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            if (updateDto.Code.HasValue)
            {
                if (updateDto.Code.Value <= 0)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.1",
                        Title = "Неверный запрос",
                        Message = "Код занятия должен быть положительным",
                        Field = nameof(updateDto.Code)
                    });
                }

                int duplicateCode = updateDto.Code.Value;
                bool isDuplicateCode = await _context.Lessons
                    .AnyAsync(l => l.Code == duplicateCode && l.Uuid != uuid);

                if (isDuplicateCode)
                {
                    return Conflict(new ApiError
                    {
                        StatusCode = "0.2.2",
                        Title = "Неверный запрос",
                        Message = $"Занятие с кодом \"{duplicateCode}\" уже существует",
                        Field = nameof(updateDto.Code)
                    });
                }
            }

            if (updateDto.StartDate.HasValue && updateDto.StartDate == DateTime.MinValue)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.1",
                    Title = "Неверный запрос",
                    Message = "Параметр StartDate некорректен",
                    Field = nameof(updateDto.StartDate)
                });
            }

            LessonTypes? lessonType = null;

            if (updateDto.LessonTypeUuid.HasValue)
            {
                if (updateDto.LessonTypeUuid.Value == Guid.Empty)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.1",
                        Title = "Неверный запрос",
                        Message = "Параметр LessonTypeUuid некорректен",
                        Field = nameof(updateDto.LessonTypeUuid)
                    });
                }

                lessonType = await _context.LessonTypes.FirstOrDefaultAsync(x => x.Uuid == updateDto.LessonTypeUuid.Value);
                if (lessonType == null)
                {
                    return NotFound(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Тип занятия не найден",
                        Message = $"Тип занятия с UUID \"{updateDto.LessonTypeUuid}\" не найден",
                        Field = nameof(updateDto.LessonTypeUuid)
                    });
                }
            }

            Disciplines? discipline = null;
            if (updateDto.DisciplineUuid.HasValue)
            {
                if (updateDto.DisciplineUuid.Value == Guid.Empty)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.1",
                        Title = "Неверный запрос",
                        Message = "Параметр DisciplineUuid некорректен",
                        Field = nameof(updateDto.DisciplineUuid)
                    });
                }

                discipline = await _context.Disciplines.FirstOrDefaultAsync(x => x.Uuid == updateDto.DisciplineUuid.Value);
                if (discipline == null)
                {
                    return NotFound(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Дисциплина не найдена",
                        Message = $"Дисциплина с UUID \"{updateDto.DisciplineUuid}\" не найдена",
                        Field = nameof(updateDto.DisciplineUuid)
                    });
                }
            }

            string? lessonName = updateDto.Name;
            if (lessonName != null)
            {
                if (lessonName.Trim() == string.Empty)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.0",
                        Title = "Неверный запрос",
                        Message = "Название занятия не может быть пустым",
                        Field = nameof(updateDto.Name)
                    });
                }

                Guid duplicateLessonUuid = await _context.Lessons
                    .Where(l => l.Name == lessonName.Trim() && l.Uuid != uuid)
                    .Select(l => l.Uuid)
                    .FirstOrDefaultAsync();

                if (duplicateLessonUuid != Guid.Empty)
                {
                    return Conflict(new ApiError
                    {
                        StatusCode = "0.2.2",
                        Title = "Неверный запрос",
                        Message = $"Занятие с названием \"{lessonName}\" уже существует (UUID: {duplicateLessonUuid})",
                        Field = nameof(updateDto.Name)
                    });
                }
            }

            // Запрос к бд и последующие проверки
            Lessons? lesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Uuid == uuid);
            if (lesson == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Занятие не найдено",
                    Message = $"Занятие с UUID \"{uuid}\" не найдено",
                    Field = nameof(uuid)
                });
            }

            if (updateDto.Code.HasValue)
            {
                lesson.Code = updateDto.Code.Value;
            }

            if (updateDto.StartDate.HasValue)
            {
                lesson.StartDate = updateDto.StartDate.Value;
            }

            if (lessonName != null)
            {
                lesson.Name = lessonName;
            }

            if (updateDto.ShortName != null)
            {
                if (lessonName == null)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.0",
                        Title = "Неверный запрос",
                        Message = "Параметр ShortName не может быть обновлён без указания параметра Name, так как при обновлении ShortName до пустой строки, она будет автоматически сгенерирована из Name",
                        Field = nameof(updateDto.ShortName)
                    });
                }

                if (updateDto.ShortName.Trim() != string.Empty)
                {
                    lesson.ShortName = updateDto.ShortName.Trim();
                }
                else
                {
                    lesson.ShortName = updateDto.ShortName.Trim() != string.Empty
                    ? updateDto.ShortName.Trim()
                    : string.Concat(lessonName
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(w => w[0]));
                }
            }

            if (updateDto.LessonTypeUuid.HasValue)
            {
                lesson.LessonTypeId = lessonType!.LessonTypeId;
            }

            if (updateDto.DisciplineUuid.HasValue)
            {
                lesson.DisciplineId = discipline!.DisciplineId;
            }

            await _context.SaveChangesAsync();

            return Ok(new LessonsResponseDto(lesson));
        }
    }
}
