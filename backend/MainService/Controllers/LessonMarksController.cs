using MainService.Errors;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace MainService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class LessonMarksController : ControllerBase
    {
        private readonly MainServiceContext _context;

        public LessonMarksController(MainServiceContext context)
        {
            _context = context;
        }


        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Оценки занятия найдены", typeof(PagedResult<LessonMarksResponseDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Оценки занятия не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(Summary = "Получить список оценок занятия")]
        public async Task<ActionResult<PagedResult<LessonMarksResponseDto>>> GetLessonMarks(
            [FromQuery, SwaggerParameter("UUID занятия")]
            Guid? lessonUuid = null,
            [FromQuery, SwaggerParameter("UUID студента")]
            Guid? studentUuid = null,
            [FromQuery, SwaggerParameter("Количество записей")]
            int size = 50,
            [FromQuery, SwaggerParameter("Смещение от начала списка")]
            int offset = 0
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

            // Проверяем корректность и существование перед основным запросом
            Lessons? lesson = null;
            if (lessonUuid != null)
            {
                if (lessonUuid == Guid.Empty)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.0",
                        Title = "Неверный запрос",
                        Message = "UUID занятия не может быть пустым",
                        Field = nameof(lessonUuid)
                    });
                }

                lesson = await _context.Lessons.AsNoTracking().FirstOrDefaultAsync(l => l.Uuid == lessonUuid.Value);
                if (lesson == null)
                {
                    return NotFound(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Занятие не найдено",
                        Message = $"Занятие с UUID \"{lessonUuid}\" не найдено",
                        Field = nameof(lessonUuid)
                    });
                }
            }

            Students? student = null;
            if (studentUuid != null)
            {
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

                student = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Uuid == studentUuid.Value);
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
            }

            IQueryable<LessonMarks> baseQuery = _context.LessonMarks
                .Include(lm => lm.Lesson)
                .Include(lm => lm.Mark)
                .Include(lm => lm.Student)
                .AsNoTracking();

            if (lesson != null)
            {
                baseQuery = baseQuery.Where(x => x.Lesson!.LessonId == lesson.LessonId);
            }

            if (student != null)
            {
                baseQuery = baseQuery.Where(x => x.Student!.StudentId == student.StudentId);
            }

            int total = await baseQuery.CountAsync();

            List<LessonMarksResponseDto> items = await baseQuery
                .TakeWithOffset(offset, size)
                .Select(lm => new LessonMarksResponseDto(lm))
                .ToListAsync();

            if (total == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Оценки не найдены",
                    Message = "В системе не найдено ни одной оценки занятия",
                    Field = string.Empty
                });
            }

            if (items.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.1.3",
                    Title = "Оценки не найдены",
                    Message = "В системе не найдено ни одной оценки занятия для указанных параметров запроса",
                    Field = "BODY"
                });
            }

            return Ok(new PagedResult<LessonMarksResponseDto>(
                Total: total,
                Offset: offset,
                Size: items.Count,
                Items: items
            ));
        }

        [HttpGet("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Оценка найдена", typeof(LessonMarksResponseDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Оценка не найдена", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerOperation(Summary = "Получить оценку по UUID")]
        public async Task<ActionResult<LessonMarksResponseDto>> GetLessonMark(
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
                    Message = "UUID оценки не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            LessonMarks? lm = await _context.LessonMarks
                .Include(x => x.Lesson)
                .Include(x => x.Mark)
                .Include(x => x.Student)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Uuid == uuid);

            if (lm == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Оценка не найдена",
                    Message = $"Оценка занятия с UUID \"{uuid}\" не найдена",
                    Field = nameof(uuid)
                });
            }

            return Ok(new LessonMarksResponseDto(lm));
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, "Оценка успешно создана", typeof(LessonMarksResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Некорректные данные", typeof(ApiError))]
        [SwaggerOperation(
            Summary = "Создать оценку занятия"
        )]
        public async Task<ActionResult<LessonMarksResponseDto>> CreateLessonMark([FromBody, SwaggerParameter("Данные новой оценки")] LessonMarksRequestDto createDto)
        {
            if (createDto.Uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID не может быть пустым",
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

            if (createDto.MarkUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID оценки не может быть пустым",
                    Field = nameof(createDto.MarkUuid)
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

            Lessons? lesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Uuid == createDto.LessonUuid);
            if (lesson == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Некорректные данные",
                    Message = "Занятие не найдено",
                    Field = nameof(createDto.LessonUuid)
                });
            }

            Marks? mark = await _context.Marks.FirstOrDefaultAsync(m => m.Uuid == createDto.MarkUuid);
            if (mark == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Некорректные данные",
                    Message = "Оценка не найдена",
                    Field = nameof(createDto.MarkUuid)
                });
            }

            Students? student = await _context.Students.FirstOrDefaultAsync(s => s.Uuid == createDto.StudentUuid);
            if (student == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Некорректные данные",
                    Message = "Студент не найден",
                    Field = nameof(createDto.StudentUuid)
                });
            }

            LessonMarks newLm = new()
            {
                Uuid = createDto.Uuid,
                LessonId = lesson.LessonId,
                Lesson = lesson,
                MarkId = mark.MarkId,
                Mark = mark,
                StudentId = student.StudentId,
                Student = student
            };

            _context.LessonMarks.Add(newLm);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetLessonMark),
                new { uuid = newLm.Uuid },
                new LessonMarksResponseDto(newLm)
            );
        }

        [HttpDelete("{uuid}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Оценка удалена")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Оценка не найдена", typeof(ApiError))]
        [SwaggerOperation(Summary = "Удалить оценку по UUID")]
        public async Task<IActionResult> DeleteLessonMark(
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

            LessonMarks? lm = await _context.LessonMarks.FirstOrDefaultAsync(x => x.Uuid == uuid);
            if (lm == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Оценка не найдена",
                    Message = $"Оценка с UUID \"{uuid}\" не найдена",
                    Field = nameof(uuid)
                });
            }

            _context.LessonMarks.Remove(lm);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Оценка обновлена", typeof(LessonMarksResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Оценка не найдена", typeof(ApiError))]
        [SwaggerOperation(Summary = "Обновить оценку по UUID")]
        public async Task<ActionResult<LessonMarksResponseDto>> UpdateLessonMark([SwaggerParameter("UUID оценки")] Guid uuid, [FromBody, SwaggerParameter("Данные для обновления")] LessonMarksRequestDto updateDto)
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

            LessonMarks? lm = await _context.LessonMarks.FirstOrDefaultAsync(x => x.Uuid == uuid);
            if (lm == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Оценка не найдена",
                    Message = $"Оценка с UUID \"{uuid}\" не найдена",
                    Field = nameof(uuid)
                });
            }

            if (updateDto.LessonUuid != Guid.Empty && updateDto.LessonUuid != lm.Lesson!.Uuid)
            {
                Lessons? newLesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Uuid == updateDto.LessonUuid);
                if (newLesson == null)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Некорректные данные",
                        Message = "Занятие не найдено",
                        Field = nameof(updateDto.LessonUuid)
                    });
                }
                lm.LessonId = newLesson.LessonId; lm.Lesson = newLesson;
            }

            if (updateDto.MarkUuid != Guid.Empty && updateDto.MarkUuid != lm.Mark!.Uuid)
            {
                Marks? newMark = await _context.Marks.FirstOrDefaultAsync(m => m.Uuid == updateDto.MarkUuid);
                if (newMark == null)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Некорректные данные",
                        Message = "Оценка не найдена",
                        Field = nameof(updateDto.MarkUuid)
                    });
                }
                lm.MarkId = newMark.MarkId; lm.Mark = newMark;
            }

            if (updateDto.StudentUuid != Guid.Empty && updateDto.StudentUuid != lm.Student!.Uuid)
            {
                Students? newStudent = await _context.Students.FirstOrDefaultAsync(s => s.Uuid == updateDto.StudentUuid);
                if (newStudent == null)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Некорректные данные",
                        Message = "Студент не найден",
                        Field = nameof(updateDto.StudentUuid)
                    });
                }
                lm.StudentId = newStudent.StudentId; lm.Student = newStudent;
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

            return Ok(new LessonMarksResponseDto(lm));
        }
    }
}
