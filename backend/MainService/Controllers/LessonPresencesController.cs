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
    public class LessonPresencesController : ControllerBase
    {
        private readonly MainServiceContext _context;

        public LessonPresencesController(MainServiceContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Посещения занятий найдены", typeof(PagedResult<LessonPresencesResponseDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Посещения занятий не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Получить список посещений занятий студента",
            Description = "Возвращает список посещений занятий по заданным параметрам"
        )]
        public async Task<ActionResult<PagedResult<LessonPresencesResponseDto>>> GetLessonPresences(
            [FromQuery, SwaggerParameter("UUID студента")]
            Guid studentUuid,
            [FromQuery, SwaggerParameter("Количество записей")]
            int size = 50,
            [FromQuery, SwaggerParameter("Смещение от начала списка")]
            int offset = 0,
            [FromQuery, SwaggerParameter("Начальная дата занятия (включительно)")]
            DateTime? startDate = null,
            [FromQuery, SwaggerParameter("UUID статуса присутствия")]
            Guid? presenceStatusUuid = null,
            [FromQuery, SwaggerParameter("Флаг присутствия (true/false)")]
            bool? isPresent = null,
            [FromQuery, SwaggerParameter("Порядок сортировки по дате занятия")]
            SortOrder sortOrder = SortOrder.Ascending
        )
        {
            // Валидация входных UUID до основных запросов к БД
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
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "Параметр startDate некорректен",
                    Field = nameof(startDate)
                });
            }

            if (studentUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID студента не может быть пустым",
                    Field = nameof(studentUuid)
                });
            }

            if (presenceStatusUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID статуса присутствия не может быть пустым",
                    Field = nameof(presenceStatusUuid)
                });
            }

            IQueryable<LessonPresences> baseQuery = _context.LessonPresences
                .Include(lp => lp.Lesson)
                .Include(lp => lp.Student)
                .Include(lp => lp.PresenceStatus)
                .Where(lp => ( lp.Student!.Uuid == studentUuid)
                             && (presenceStatusUuid == null || lp.PresenceStatus!.Uuid == presenceStatusUuid)
                             && (isPresent == null || lp.IsPresent == isPresent))
                .AsNoTracking();
            DateTime effectiveStart = startDate ?? DateTime.UtcNow;
            baseQuery = baseQuery.Where(lp => lp.Lesson!.StartDate >= effectiveStart);
            Task<int> totalRecord = baseQuery.CountAsync();

            List<LessonPresencesResponseDto> items = await baseQuery
                .SortByKey(lp => lp.Lesson!.StartDate, sortOrder)
                .TakeWithOffset(offset, size)
                .Select(lp => new LessonPresencesResponseDto
                {
                    Uuid = lp.Uuid,
                    IsPresent = lp.IsPresent,
                    LessonUuid = lp.Lesson!.Uuid,
                    StudentUuid = lp.Student!.Uuid,
                    PresenceStatusUuid = lp.PresenceStatus!.Uuid,
                    Version = lp.Version
                })
                .ToListAsync();
            int total = await totalRecord;

            if (total == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Посещения занятий не найдены",
                    Message = "В системе не найдено ни одного посещения занятия",
                    Field = string.Empty
                });
            }

            if (items.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Посещения занятий не найдены",
                    Message = "В системе не найдено ни одного посещения занятия для указанных параметров запроса",
                    Field = string.Empty
                });
            }

            return Ok(new PagedResult<LessonPresencesResponseDto>(
                Total: total,
                Offset: offset,
                Size: items.Count,
                Items: items
            ));
        }

        [HttpGet("studentInDiscipline")]
        [SwaggerResponse(StatusCodes.Status200OK, "Посещения студента найдены", typeof(PagedResult<LessonPresencesResponseDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Студент или посещения не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Получить посещения по студенту на дисциплине",
            Description = "Возвращает список посещений для указанного UUID студента на указанной дисциплине"
        )]
        public async Task<ActionResult<PagedResult<LessonPresencesResponseDto>>> GetLessonPresencesByStudent(
            [FromQuery, SwaggerParameter("UUID студента")]
            Guid studentUuid,
            [FromQuery, SwaggerParameter("UUID дисциплины (обязательно)")]
            Guid disciplineUuid,
            [FromQuery, SwaggerParameter("Начальная дата занятия (включительно)")]
            DateTime? startDate = null,
            [FromQuery, SwaggerParameter("Количество записей")]
            int size = 50,
            [FromQuery, SwaggerParameter("Смещение от начала списка")]
            int offset = 0,
            [FromQuery, SwaggerParameter("Порядок сортировки по дате занятия")]
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

            if (studentUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID студента не может быть пустым",
                    Field = nameof(studentUuid)
                });
            }

            if (disciplineUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID дисциплины не может быть пустым",
                    Field = nameof(disciplineUuid)
                });
            }

            if (startDate.HasValue && startDate == DateTime.MinValue)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "Параметр startDate некорректен",
                    Field = nameof(startDate)
                });
            }

            Students? student = await _context.Students.FirstOrDefaultAsync(s => s.Uuid == studentUuid);
            if (student == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Студент не найден",
                    Message = $"Студент с UUID \"{studentUuid}\" не найден",
                    Field = nameof(studentUuid)
                });
            }

            Disciplines? discipline = await _context.Disciplines.FirstOrDefaultAsync(d => d.Uuid == disciplineUuid);
            if (discipline == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Некорректные данные",
                    Message = "Дисциплина с указанным UUID не найдена",
                    Field = nameof(disciplineUuid)
                });
            }

            IQueryable<LessonPresences> baseQuery = _context.LessonPresences
                .Include(lp => lp.Lesson)
                .Include(lp => lp.PresenceStatus)
                .Where(lp => lp.StudentId == student.StudentId
                             && lp.Lesson!.DisciplineId == discipline.DisciplineId)
                .AsNoTracking();

            DateTime start = startDate ?? DateTime.UtcNow;
            baseQuery = baseQuery.Where(lp => lp.Lesson!.StartDate >= start);

            Task<int> totalRecord = baseQuery.CountAsync();

            List<LessonPresencesResponseDto> items = await baseQuery
                .SortByKey(lp => lp.Lesson!.StartDate, sortOrder)
                .TakeWithOffset(offset, size)
                .Select(lp => new LessonPresencesResponseDto
                {
                    Uuid = lp.Uuid,
                    IsPresent = lp.IsPresent,
                    LessonUuid = lp.Lesson!.Uuid,
                    StudentUuid = student.Uuid,
                    PresenceStatusUuid = lp.PresenceStatus!.Uuid,
                    Version = lp.Version
                })
                .ToListAsync();

            int total = await totalRecord;

            if (total == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Посещения не найдены",
                    Message = "В системе не найдено ни одного посещения занятия",
                    Field = string.Empty
                });
            }

            if (items.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Посещения не найдены",
                    Message = "В системе не найдено ни одного посещения занятия для указанного студента по заданной дисциплине и параметрам запроса",
                    Field = string.Empty
                });
            }

            return Ok(new PagedResult<LessonPresencesResponseDto>(
                Total: total,
                Offset: offset,
                Size: items.Count,
                Items: items
            ));
        }


        [HttpGet("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Посещение найдено", typeof(LessonPresencesResponseDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Посещение не найдено", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Получить посещение занятия по UUID",
            Description = "Возвращает посещение занятия по указанному UUID"
        )]
        public async Task<ActionResult<LessonPresencesResponseDto>> GetLessonPresence(
            [SwaggerParameter("UUID посещения")]
            Guid uuid
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID посещения занятия не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            LessonPresences? lp = await _context.LessonPresences
                .Include(x => x.Lesson)
                .Include(x => x.Student)
                .Include(x => x.PresenceStatus)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Uuid == uuid);

            if (lp == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Посещение не найдено",
                    Message = $"Посещение занятия с UUID \"{uuid}\" не найдено",
                    Field = nameof(uuid)
                });
            }

            return Ok(new LessonPresencesResponseDto(lp));
        }


        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, "Посещение успешно создано", typeof(LessonPresencesResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Некорректные данные", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Создать новое посещение занятия",
            Description = "Создаёт запись о посещении занятия"
        )]
        public async Task<ActionResult<LessonPresencesResponseDto>> CreateLessonPresence(
            [FromBody, SwaggerParameter("Данные нового посещения")]
            LessonPresencesRequestDto createDto
        )
        {
            if (createDto.Uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID посещения не может быть пустым",
                    Field = nameof(createDto.Uuid)
                });
            }

            if (createDto.LessonUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID занятия не может быть пустым",
                    Field = nameof(createDto.LessonUuid)
                });
            }

            if (createDto.StudentUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID студента не может быть пустым",
                    Field = nameof(createDto.StudentUuid)
                });
            }

            if (createDto.PresenceStatusUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID статуса присутствия не может быть пустым",
                    Field = nameof(createDto.PresenceStatusUuid)
                });
            }

            Lessons? lesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Uuid == createDto.LessonUuid);
            if (lesson == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Некорректные данные",
                    Message = "Занятие с указанным UUID не найдено",
                    Field = nameof(createDto.LessonUuid)
                });
            }

            Students? student = await _context.Students.FirstOrDefaultAsync(s => s.Uuid == createDto.StudentUuid);
            if (student == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Некорректные данные",
                    Message = "Студент с указанным UUID не найден",
                    Field = nameof(createDto.StudentUuid)
                });
            }

            PresenceStatuses? presenceStatus = await _context.PresenceStatuses.FirstOrDefaultAsync(p => p.Uuid == createDto.PresenceStatusUuid);
            if (presenceStatus == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Некорректные данные",
                    Message = "Статус присутствия с указанным UUID не найден",
                    Field = nameof(createDto.PresenceStatusUuid)
                });
            }

            LessonPresences newLp = new LessonPresences
            {
                Uuid = createDto.Uuid,
                IsPresent = createDto.IsPresent,
                LessonId = lesson.LessonId,
                Lesson = lesson,
                StudentId = student.StudentId,
                Student = student,
                PresenceStatusId = presenceStatus.PresenceStatusId,
                PresenceStatus = presenceStatus
            };

            _context.LessonPresences.Add(newLp);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetLessonPresence),
                new { uuid = newLp.Uuid },
                new LessonPresencesResponseDto(newLp)
            );
        }


        [HttpDelete("{uuid}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Посещение удалено")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Посещение не найдено", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(Summary = "Удалить посещение по UUID")]
        public async Task<IActionResult> DeleteLessonPresence(
            [SwaggerParameter("UUID посещения")]
            Guid uuid
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID посещения не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            LessonPresences? lp = await _context.LessonPresences
                .FirstOrDefaultAsync(x => x.Uuid == uuid);

            if (lp == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Посещение не найдено",
                    Message = $"Посещение занятия с UUID \"{uuid}\" не найдено",
                    Field = nameof(uuid)
                });
            }

            _context.LessonPresences.Remove(lp);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPatch("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Посещение обновлено", typeof(LessonPresencesResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Посещение не найдено", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Обновить посещение",
            Description = "Обновляет данные посещения по UUID. Все поля необязательны"
        )]
        public async Task<ActionResult<LessonPresencesResponseDto>> UpdateLessonPresence(
            [SwaggerParameter("UUID посещения")]
            Guid uuid,
            [FromBody, SwaggerParameter("Данные для обновления")]
            LessonPresencesUpdateDto updateDto
        )
        {
            // Валидация входных полей DTO до основного запроса на получение записи
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID посещения не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            if (updateDto.LessonUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID занятия не может быть пустым",
                    Field = nameof(updateDto.LessonUuid)
                });
            }

            if (updateDto.StudentUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID студента не может быть пустым",
                    Field = nameof(updateDto.StudentUuid)
                });
            }

            if (updateDto.PresenceStatusUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID статуса присутствия не может быть пустым",
                    Field = nameof(updateDto.PresenceStatusUuid)
                });
            }

            Lessons? newLesson = null;
            Students? newStudent = null;
            PresenceStatuses? newPresenceStatus = null;

            if (updateDto.LessonUuid != Guid.Empty)
            {
                newLesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Uuid == updateDto.LessonUuid);
                if (newLesson == null)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Некорректные данные",
                        Message = "Занятие с указанным UUID не найдено",
                        Field = nameof(updateDto.LessonUuid)
                    });
                }
            }

            if (updateDto.StudentUuid != Guid.Empty)
            {
                newStudent = await _context.Students.FirstOrDefaultAsync(s => s.Uuid == updateDto.StudentUuid);
                if (newStudent == null)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Некорректные данные",
                        Message = "Студент с указанным UUID не найден",
                        Field = nameof(updateDto.StudentUuid)
                    });
                }
            }

            if (updateDto.PresenceStatusUuid != Guid.Empty)
            {
                newPresenceStatus = await _context.PresenceStatuses.FirstOrDefaultAsync(p => p.Uuid == updateDto.PresenceStatusUuid);
                if (newPresenceStatus == null)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Некорректные данные",
                        Message = "Статус присутствия с указанным UUID не найден",
                        Field = nameof(updateDto.PresenceStatusUuid)
                    });
                }
            }

            LessonPresences? lp = await _context.LessonPresences
                .FirstOrDefaultAsync(x => x.Uuid == uuid);

            if (lp == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Посещение не найдено",
                    Message = $"Посещение занятия с UUID \"{uuid}\" не найдено",
                    Field = nameof(uuid)
                });
            }

            if (newLesson != null && lp.LessonId != newLesson.LessonId)
            {
                lp.LessonId = newLesson.LessonId;
                lp.Lesson = newLesson;
            }

            if (newStudent != null && lp.StudentId != newStudent.StudentId)
            {
                lp.StudentId = newStudent.StudentId;
                lp.Student = newStudent;
            }

            if (newPresenceStatus != null && lp.PresenceStatusId != newPresenceStatus.PresenceStatusId)
            {
                lp.PresenceStatusId = newPresenceStatus.PresenceStatusId;
                lp.PresenceStatus = newPresenceStatus;
            }

            if (updateDto.IsPresent != null && lp.IsPresent != updateDto.IsPresent)
            {
                lp.IsPresent = updateDto.IsPresent.Value;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Конфликт версий",
                    Message = "Данные были изменены кем-то другим. Попробуйте обновить и повторить запрос.",
                    Field = string.Empty
                });
            }
            catch (DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Ошибка базы данных",
                    Message = "Не удалось сохранить изменения",
                    Field = string.Empty
                });
            }

            return Ok(new LessonPresencesResponseDto(lp));
        }


    }
}
