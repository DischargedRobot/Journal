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
    public class AttestationsController : ControllerBase
    {
        private readonly MainServiceContext _context;

        public AttestationsController(MainServiceContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Аттестации найдены", typeof(PagedResult<AttestationsResponseDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Аттестации не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(Summary = "Получить список аттестаций")]
        public async Task<ActionResult<PagedResult<AttestationsResponseDto>>> GetAttestations(
            [FromQuery, SwaggerParameter("UUID студента")]
            Guid? studentUuid = null,
            [FromQuery, SwaggerParameter("UUID дисциплины")]
            Guid? disciplineUuid = null,
            [FromQuery, SwaggerParameter("UUID типа аттестации")]
            Guid? attestationTypeUuid = null,
            [FromQuery, SwaggerParameter("UUID оценки аттестации")]
            Guid? attestationMarkUuid = null,
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

            Disciplines? discipline = null;
            if (disciplineUuid != null)
            {
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

                discipline = await _context.Disciplines.AsNoTracking().FirstOrDefaultAsync(d => d.Uuid == disciplineUuid.Value);
                if (discipline == null)
                {
                    return NotFound(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Дисциплина не найдена",
                        Message = $"Дисциплина с UUID \"{disciplineUuid}\" не найдена",
                        Field = nameof(disciplineUuid)
                    });
                }
            }

            AttestationTypes? attType = null;
            if (attestationTypeUuid != null)
            {
                if (attestationTypeUuid == Guid.Empty)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.0",
                        Title = "Неверный запрос",
                        Message = "UUID типа аттестации не может быть пустым",
                        Field = nameof(attestationTypeUuid)
                    });
                }

                attType = await _context.AttestationTypes.AsNoTracking().FirstOrDefaultAsync(t => t.Uuid == attestationTypeUuid.Value);
                if (attType == null)
                {
                    return NotFound(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Тип аттестации не найден",
                        Message = $"Тип аттестации с UUID \"{attestationTypeUuid}\" не найден",
                        Field = nameof(attestationTypeUuid)
                    });
                }
            }

            AttestationMarks? attMark = null;
            if (attestationMarkUuid != null)
            {
                if (attestationMarkUuid == Guid.Empty)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.0",
                        Title = "Неверный запрос",
                        Message = "UUID оценки аттестации не может быть пустым",
                        Field = nameof(attestationMarkUuid)
                    });
                }

                attMark = await _context.AttestationMarks.AsNoTracking().FirstOrDefaultAsync(m => m.Uuid == attestationMarkUuid.Value);
                if (attMark == null)
                {
                    return NotFound(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Оценка аттестации не найдена",
                        Message = $"Оценка аттестации с UUID \"{attestationMarkUuid}\" не найдена",
                        Field = nameof(attestationMarkUuid)
                    });
                }
            }

            IQueryable<Attestations> baseQuery = _context.Attestations
                .Include(a => a.AttestationType)
                .Include(a => a.AttestationMark)
                .Include(a => a.Student)
                .Include(a => a.Discipline)
                .AsNoTracking();

            if (student != null) baseQuery = baseQuery.Where(x => x.Student!.StudentId == student.StudentId);
            if (discipline != null) baseQuery = baseQuery.Where(x => x.Discipline!.DisciplineId == discipline.DisciplineId);
            if (attType != null) baseQuery = baseQuery.Where(x => x.AttestationType!.AttestationTypeId == attType.AttestationTypeId);
            if (attMark != null) baseQuery = baseQuery.Where(x => x.AttestationMark!.AttestationMarkId == attMark.AttestationMarkId);

            Task<int> totalRecord = baseQuery.CountAsync();

            List<AttestationsResponseDto> items = await baseQuery
                .Skip(offset)
                .Take(size)
                .Select(a => new AttestationsResponseDto(a))
                .ToListAsync();

            int total = await totalRecord;

            if (total == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Аттестации не найдены",
                    Message = "В системе не найдено ни одной аттестации",
                    Field = string.Empty
                });
            }

            if (items.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.1.3",
                    Title = "Аттестации не найдены",
                    Message = "В системе не найдено ни одной аттестации для указанных параметров запроса",
                    Field = "BODY"
                });
            }

            return Ok(new PagedResult<AttestationsResponseDto>(
                Total: total,
                Offset: offset,
                Size: items.Count,
                Items: items
            ));
        }

        [HttpGet("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Аттестация найдена", typeof(AttestationsResponseDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Аттестация не найдена", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerOperation(Summary = "Получить аттестацию по UUID")]
        public async Task<ActionResult<AttestationsResponseDto>> GetAttestation(
            [SwaggerParameter("UUID аттестации")]
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

            Attestations? a = await _context.Attestations
                .Include(x => x.AttestationType)
                .Include(x => x.AttestationMark)
                .Include(x => x.Student)
                .Include(x => x.Discipline)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Uuid == uuid);

            if (a == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Аттестация не найдена",
                    Message = $"Аттестация с UUID \"{uuid}\" не найдена",
                    Field = nameof(uuid)
                });
            }

            return Ok(new AttestationsResponseDto(a));
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, "Аттестация создана", typeof(AttestationsResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Некорректные данные", typeof(ApiError))]
        [SwaggerOperation(Summary = "Создать аттестацию")]
        public async Task<ActionResult<AttestationsResponseDto>> CreateAttestation([FromBody, SwaggerParameter("Данные новой аттестации")] AttestationsRequestDto createDto)
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

            if (createDto.AttestationTypeUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID типа аттестации не может быть пустым",
                    Field = nameof(createDto.AttestationTypeUuid)
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

            AttestationTypes? type = await _context.AttestationTypes.FirstOrDefaultAsync(t => t.Uuid == createDto.AttestationTypeUuid);
            if (type == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Некорректные данные",
                    Message = "Тип аттестации не найден",
                    Field = nameof(createDto.AttestationTypeUuid)
                });
            }

            AttestationMarks? mark = null;
            if (createDto.AttestationMarkUuid != null && createDto.AttestationMarkUuid != Guid.Empty)
            {
                mark = await _context.AttestationMarks.FirstOrDefaultAsync(m => m.Uuid == createDto.AttestationMarkUuid);
                if (mark == null)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Некорректные данные",
                        Message = "Оценка аттестации не найдена",
                        Field = nameof(createDto.AttestationMarkUuid)
                    });
                }
            }

            Students? studentEntity = await _context.Students.FirstOrDefaultAsync(s => s.Uuid == createDto.StudentUuid);
            if (studentEntity == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Некорректные данные",
                    Message = "Студент не найден",
                    Field = nameof(createDto.StudentUuid)
                });
            }

            Disciplines? disciplineEntity = await _context.Disciplines.FirstOrDefaultAsync(d => d.Uuid == createDto.DisciplineUuid);
            if (disciplineEntity == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Некорректные данные",
                    Message = "Дисциплина не найдена",
                    Field = nameof(createDto.DisciplineUuid)
                });
            }

            Attestations newA = new()
            {
                Uuid = createDto.Uuid,
                Date = createDto.Date,
                AttestationTypeId = type.AttestationTypeId,
                AttestationType = type,
                AttestationMarkId = mark?.AttestationMarkId,
                AttestationMark = mark,
                StudentId = studentEntity.StudentId,
                Student = studentEntity,
                DisciplineId = disciplineEntity.DisciplineId,
                Discipline = disciplineEntity
            };

            _context.Attestations.Add(newA);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAttestation), new { uuid = newA.Uuid }, new AttestationsResponseDto(newA));
        }

        [HttpDelete("{uuid}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Аттестация удалена")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Аттестация не найдена", typeof(ApiError))]
        [SwaggerOperation(Summary = "Удалить аттестацию по UUID")]
        public async Task<IActionResult> DeleteAttestation([SwaggerParameter("UUID аттестации")] Guid uuid)
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

            Attestations? a = await _context.Attestations.FirstOrDefaultAsync(x => x.Uuid == uuid);
            if (a == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Аттестация не найдена",
                    Message = $"Аттестация с UUID \"{uuid}\" не найдена",
                    Field = nameof(uuid)
                });
            }

            _context.Attestations.Remove(a);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Аттестация обновлена", typeof(AttestationsResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Аттестация не найдена", typeof(ApiError))]
        [SwaggerOperation(Summary = "Обновить аттестацию по UUID")]
        public async Task<ActionResult<AttestationsResponseDto>> UpdateAttestation([SwaggerParameter("UUID аттестации")] Guid uuid, [FromBody, SwaggerParameter("Данные для обновления")] AttestationsRequestDto updateDto)
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

            Attestations? a = await _context.Attestations.FirstOrDefaultAsync(x => x.Uuid == uuid);
            if (a == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Аттестация не найдена",
                    Message = $"Аттестация с UUID \"{uuid}\" не найдена",
                    Field = nameof(uuid)
                });
            }

            if (updateDto.AttestationTypeUuid != Guid.Empty && updateDto.AttestationTypeUuid != a.AttestationType!.Uuid)
            {
                AttestationTypes? newType = await _context.AttestationTypes.FirstOrDefaultAsync(t => t.Uuid == updateDto.AttestationTypeUuid);
                if (newType == null)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Некорректные данные",
                        Message = "Тип аттестации не найден",
                        Field = nameof(updateDto.AttestationTypeUuid)
                    });
                }
                a.AttestationTypeId = newType.AttestationTypeId; a.AttestationType = newType;
            }

            if (updateDto.AttestationMarkUuid != null && updateDto.AttestationMarkUuid != Guid.Empty && (a.AttestationMark == null || updateDto.AttestationMarkUuid != a.AttestationMark.Uuid))
            {
                AttestationMarks? newMark = await _context.AttestationMarks.FirstOrDefaultAsync(m => m.Uuid == updateDto.AttestationMarkUuid);
                if (newMark == null)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Некорректные данные",
                        Message = "Оценка аттестации не найдена",
                        Field = nameof(updateDto.AttestationMarkUuid)
                    });
                }
                a.AttestationMarkId = newMark.AttestationMarkId; a.AttestationMark = newMark;
            }

            if (updateDto.StudentUuid != Guid.Empty && updateDto.StudentUuid != a.Student!.Uuid)
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
                a.StudentId = newStudent.StudentId; a.Student = newStudent;
            }

            if (updateDto.DisciplineUuid != Guid.Empty && updateDto.DisciplineUuid != a.Discipline!.Uuid)
            {
                Disciplines? newDiscipline = await _context.Disciplines.FirstOrDefaultAsync(d => d.Uuid == updateDto.DisciplineUuid);
                if (newDiscipline == null)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Некорректные данные",
                        Message = "Дисциплина не найдена",
                        Field = nameof(updateDto.DisciplineUuid)
                    });
                }
                a.DisciplineId = newDiscipline.DisciplineId; a.Discipline = newDiscipline;
            }

            if (updateDto.Date != a.Date)
            {
                a.Date = updateDto.Date;
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

            return Ok(new AttestationsResponseDto(a));
        }
    }
}
