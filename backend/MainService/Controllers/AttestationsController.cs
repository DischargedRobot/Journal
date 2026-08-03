using MainService.Errors;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using MainService.Lib.Utils;
using System.Diagnostics;

namespace MainService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class AttestationsController : ControllerBase
    {
        private readonly MainServiceContext _context;
        private readonly ILogger<AttestationsController> _logger;
        private readonly ActivitySource _activitySource;

        public AttestationsController(MainServiceContext context, ILogger<AttestationsController> logger, System.Diagnostics.ActivitySource activitySource)
        {
            _context = context;
            _logger = logger;
            _activitySource = activitySource;
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
            string functionName = ControllerContext.ActionDescriptor.ActionName;
            try
            {
                using Activity? activity = _activitySource.StartAndLog(_logger, this);
                _logger.LogInformation("{Function} вызвано: studentUuid={StudentUuid}, disciplineUuid={DisciplineUuid}, attestationTypeUuid={AttType}, attestationMarkUuid={AttMark}, size={Size}, offset={Offset}", functionName, studentUuid, disciplineUuid, attestationTypeUuid, attestationMarkUuid, size, offset);

                if (offset < 0)
                {
                    _logger.LogWarning("{Function}: неверный offset {Offset}", functionName, offset);
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
                    _logger.LogWarning("{Function}: неверный size {Size}", functionName, size);
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
                        _logger.LogWarning("{Function}: пустой studentUuid", functionName);
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
                        _logger.LogInformation("{Function}: студент с uuid={Uuid} не найден", functionName, studentUuid);
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
                        _logger.LogWarning("{Function}: пустой disciplineUuid", functionName);
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
                        _logger.LogInformation("{Function}: дисциплина с uuid={Uuid} не найдена", functionName, disciplineUuid);
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
                        _logger.LogWarning("{Function}: пустой attestationTypeUuid", functionName);
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
                        _logger.LogInformation("{Function}: тип аттестации с uuid={Uuid} не найден", functionName, attestationTypeUuid);
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
                        _logger.LogWarning("{Function}: пустой attestationMarkUuid", functionName);
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
                        _logger.LogInformation("{Function}: оценка аттестации с uuid={Uuid} не найдена", functionName, attestationMarkUuid);
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
                    .AsNoTracking();

                if (student != null)
                {
                    baseQuery = baseQuery.Where(x => x.Student!.StudentId == student.StudentId);
                }
                if (discipline != null)
                {
                    baseQuery = baseQuery.Where(x => x.Discipline!.DisciplineId == discipline.DisciplineId);
                }
                if (attType != null)
                {
                    baseQuery = baseQuery.Where(x => x.AttestationType!.AttestationTypeId == attType.AttestationTypeId);
                }
                if (attMark != null)
                {
                    baseQuery = baseQuery.Where(x => x.AttestationMark!.AttestationMarkId == attMark.AttestationMarkId);
                }

                int total = await baseQuery.CountAsync();

                List<AttestationsResponseDto> items = await baseQuery
                    .TakeWithOffset(offset, size)
                    .Select(a => new AttestationsResponseDto
                    {
                        Uuid = a.Uuid,
                        Date = a.Date,
                        AttestationTypeUuid = a.AttestationType!.Uuid,
                        AttestationMarkUuid = a.AttestationMark != null ? a.AttestationMark.Uuid : null,
                        StudentUuid = a.Student!.Uuid,
                        DisciplineUuid = a.Discipline!.Uuid,
                        Version = a.Version
                    })
                    .ToListAsync();

                _logger.LogInformation("{Function}: найдено всего={Total}, возвращается={Count}", functionName, total, items.Count);

                if (total == 0)
                {
                    _logger.LogInformation("{Function}: не найдено записей (total=0)", functionName);
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
                    _logger.LogInformation("{Function}: нет записей по фильтру (total={Total}, offset={Offset})", functionName, total, offset);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Function}: неожиданная ошибка при получении списка аттестаций", functionName);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
                {
                    StatusCode = "1.0.0",
                    Title = "Внутренняя ошибка сервера",
                    Message = "Произошла ошибка на сервере",
                });
            }
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
            string functionName = ControllerContext.ActionDescriptor.ActionName;
            try
            {
                using Activity? activity = _activitySource.StartAndLog(_logger, this);
                _logger.LogInformation("{Function}: вызвано для uuid={Uuid}", functionName, uuid);

                if (uuid == Guid.Empty)
                {
                    _logger.LogWarning("{Function}: пустой uuid", functionName);
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.0",
                        Title = "Неверный запрос",
                        Message = "UUID не может быть пустым",
                        Field = nameof(uuid)
                    });
                }

                AttestationsResponseDto? attestation = await _context.Attestations
                    .AsNoTracking()
                    .Where(x => x.Uuid == uuid)
                    .Select(att => new AttestationsResponseDto
                    {
                        Uuid = att.Uuid,
                        Date = att.Date,
                        AttestationTypeUuid = att.AttestationType!.Uuid,
                        AttestationMarkUuid = att.AttestationMark != null ? att.AttestationMark.Uuid : null,
                        StudentUuid = att.Student!.Uuid,
                        DisciplineUuid = att.Discipline!.Uuid,
                        Version = att.Version
                    })
                    .FirstOrDefaultAsync();

                if (attestation == null)
                {
                    _logger.LogInformation("{Function}: запись с uuid={Uuid} не найдена", functionName, uuid);
                    return NotFound(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Аттестация не найдена",
                        Message = $"Аттестация с UUID \"{uuid}\" не найдена",
                        Field = nameof(uuid)
                    });
                }

                _logger.LogInformation("{Function}: возвращена запись uuid={Uuid}", functionName, uuid);
                return Ok(attestation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Function}: неожиданная ошибка при получении аттестации", functionName);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
                {
                    StatusCode = "1.0.0",
                    Title = "Внутренняя ошибка сервера",
                    Message = "Произошла ошибка на сервере",
                });
            }
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, "Аттестация создана", typeof(AttestationsResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Некорректные данные", typeof(ApiError))]
        [SwaggerOperation(Summary = "Создать аттестацию")]
        public async Task<ActionResult<AttestationsResponseDto>> CreateAttestation(
            [FromBody, SwaggerParameter("Данные новой аттестации")]
            AttestationsRequestDto createDto
        )
        {
            string functionName = ControllerContext.ActionDescriptor.ActionName;
            try
            {
                using Activity? activity = _activitySource.StartAndLog(_logger, this);
                _logger.LogInformation("{Function}: вызвано для uuid={Uuid}", functionName, createDto?.Uuid);

                if (createDto == null)
                {
                    _logger.LogWarning("{Function}: пустой createDto", functionName);
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.0",
                        Title = "Неверный запрос",
                        Message = "Тело запроса не может быть пустым",
                        Field = "BODY"
                    });
                }

                if (createDto.Uuid == Guid.Empty)
                {
                    _logger.LogWarning("{Function}: пустой createDto.Uuid", functionName);
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
                    _logger.LogWarning("{Function}: пустой AttestationTypeUuid", functionName);
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
                    _logger.LogWarning("{Function}: пустой StudentUuid", functionName);
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
                    _logger.LogWarning("{Function}: пустой DisciplineUuid", functionName);
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
                    _logger.LogInformation("{Function}: тип аттестации не найден uuid={Uuid}", functionName, createDto.AttestationTypeUuid);
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
                        _logger.LogInformation("{Function}: оценка аттестации не найдена uuid={Uuid}", functionName, createDto.AttestationMarkUuid);
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
                    _logger.LogInformation("{Function}: студент не найден uuid={Uuid}", functionName, createDto.StudentUuid);
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
                    _logger.LogInformation("{Function}: дисциплина не найдена uuid={Uuid}", functionName, createDto.DisciplineUuid);
                    return BadRequest(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Некорректные данные",
                        Message = "Дисциплина не найдена",
                        Field = nameof(createDto.DisciplineUuid)
                    });
                }

                Attestations newAttestation = new()
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

                _context.Attestations.Add(newAttestation);
                await _context.SaveChangesAsync();

                _logger.LogInformation("{Function}: создана аттестация uuid={Uuid}", functionName, newAttestation.Uuid);
                return CreatedAtAction(
                    nameof(GetAttestation),
                    new { uuid = newAttestation.Uuid },
                    new AttestationsResponseDto(newAttestation)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Function}: неожиданная ошибка при создании аттестации", functionName);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
                {
                    StatusCode = "1.0.0",
                    Title = "Внутренняя ошибка сервера",
                    Message = "Произошла ошибка на сервере",
                });
            }
        }

        [HttpDelete("{uuid}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Аттестация удалена")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Аттестация не найдена", typeof(ApiError))]
        [SwaggerOperation(Summary = "Удалить аттестацию по UUID")]
        public async Task<IActionResult> DeleteAttestation([SwaggerParameter("UUID аттестации")] Guid uuid)
        {
            string functionName = ControllerContext.ActionDescriptor.ActionName;
            try
            {
                using Activity? activity = _activitySource.StartAndLog(_logger, this);
                _logger.LogInformation("{Function}: вызвано для uuid={Uuid}", functionName, uuid);

                if (uuid == Guid.Empty)
                {
                    _logger.LogWarning("{Function}: пустой uuid", functionName);
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.0",
                        Title = "Неверный запрос",
                        Message = "UUID не может быть пустым",
                        Field = nameof(uuid)
                    });
                }

                Attestations? attestation = await _context.Attestations.FirstOrDefaultAsync(x => x.Uuid == uuid);
                if (attestation == null)
                {
                    _logger.LogInformation("{Function}: запись с uuid={Uuid} не найдена", functionName, uuid);
                    return NotFound(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Аттестация не найдена",
                        Message = $"Аттестация с UUID \"{uuid}\" не найдена",
                        Field = nameof(uuid)
                    });
                }

                _context.Attestations.Remove(attestation);
                await _context.SaveChangesAsync();

                _logger.LogInformation("{Function}: удалена запись uuid={Uuid}", functionName, uuid);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Function}: неожиданная ошибка при удалении аттестации", functionName);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
                {
                    StatusCode = "1.0.0",
                    Title = "Внутренняя ошибка сервера",
                    Message = "Произошла ошибка на сервере",
                });
            }
        }

        [HttpPatch("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Аттестация обновлена", typeof(AttestationsResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Аттестация не найдена", typeof(ApiError))]
        [SwaggerOperation(Summary = "Обновить аттестацию по UUID")]
        public async Task<ActionResult<AttestationsResponseDto>> UpdateAttestation([SwaggerParameter("UUID аттестации")] Guid uuid, [FromBody, SwaggerParameter("Данные для обновления")] AttestationsRequestDto updateDto)
        {
            string functionName = ControllerContext.ActionDescriptor.ActionName;
            try
            {
                using Activity? activity = _activitySource.StartAndLog(_logger, this);
                _logger.LogInformation("{Function}: вызвано для uuid={Uuid}", functionName, uuid);

                if (uuid == Guid.Empty)
                {
                    _logger.LogWarning("{Function}: пустой uuid", functionName);
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.0",
                        Title = "Неверный запрос",
                        Message = "UUID не может быть пустым",
                        Field = nameof(uuid)
                    });
                }

                Attestations? attestation = await _context.Attestations.FirstOrDefaultAsync(x => x.Uuid == uuid);
                if (attestation == null)
                {
                    _logger.LogInformation("{Function}: запись с uuid={Uuid} не найдена", functionName, uuid);
                    return NotFound(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Аттестация не найдена",
                        Message = $"Аттестация с UUID \"{uuid}\" не найдена",
                        Field = nameof(uuid)
                    });
                }

                if (updateDto.AttestationTypeUuid != Guid.Empty && updateDto.AttestationTypeUuid != attestation.AttestationType!.Uuid)
                {
                    AttestationTypes? newType = await _context.AttestationTypes.FirstOrDefaultAsync(t => t.Uuid == updateDto.AttestationTypeUuid);
                    if (newType == null)
                    {
                        _logger.LogInformation("{Function}: тип аттестации не найден uuid={Uuid}", functionName, updateDto.AttestationTypeUuid);
                        return BadRequest(new ApiError
                        {
                            StatusCode = "1.2.3",
                            Title = "Некорректные данные",
                            Message = "Тип аттестации не найден",
                            Field = nameof(updateDto.AttestationTypeUuid)
                        });
                    }
                    attestation.AttestationTypeId = newType.AttestationTypeId; attestation.AttestationType = newType;
                }

                if (updateDto.AttestationMarkUuid != null && updateDto.AttestationMarkUuid != Guid.Empty && (attestation.AttestationMark == null || updateDto.AttestationMarkUuid != attestation.AttestationMark.Uuid))
                {
                    AttestationMarks? newMark = await _context.AttestationMarks.FirstOrDefaultAsync(m => m.Uuid == updateDto.AttestationMarkUuid);
                    if (newMark == null)
                    {
                        _logger.LogInformation("{Function}: оценка аттестации не найдена uuid={Uuid}", functionName, updateDto.AttestationMarkUuid);
                        return BadRequest(new ApiError
                        {
                            StatusCode = "1.2.3",
                            Title = "Некорректные данные",
                            Message = "Оценка аттестации не найдена",
                            Field = nameof(updateDto.AttestationMarkUuid)
                        });
                    }
                    attestation.AttestationMarkId = newMark.AttestationMarkId; attestation.AttestationMark = newMark;
                }

                if (updateDto.StudentUuid != Guid.Empty && updateDto.StudentUuid != attestation.Student!.Uuid)
                {
                    Students? newStudent = await _context.Students.FirstOrDefaultAsync(s => s.Uuid == updateDto.StudentUuid);
                    if (newStudent == null)
                    {
                        _logger.LogInformation("{Function}: студент не найден uuid={Uuid}", functionName, updateDto.StudentUuid);
                        return BadRequest(new ApiError
                        {
                            StatusCode = "1.2.3",
                            Title = "Некорректные данные",
                            Message = "Студент не найден",
                            Field = nameof(updateDto.StudentUuid)
                        });
                    }
                    attestation.StudentId = newStudent.StudentId; attestation.Student = newStudent;
                }

                if (updateDto.DisciplineUuid != Guid.Empty && updateDto.DisciplineUuid != attestation.Discipline!.Uuid)
                {
                    Disciplines? newDiscipline = await _context.Disciplines.FirstOrDefaultAsync(d => d.Uuid == updateDto.DisciplineUuid);
                    if (newDiscipline == null)
                    {
                        _logger.LogInformation("{Function}: дисциплина не найдена uuid={Uuid}", functionName, updateDto.DisciplineUuid);
                        return BadRequest(new ApiError
                        {
                            StatusCode = "1.2.3",
                            Title = "Некорректные данные",
                            Message = "Дисциплина не найдена",
                            Field = nameof(updateDto.DisciplineUuid)
                        });
                    }
                    attestation.DisciplineId = newDiscipline.DisciplineId; attestation.Discipline = newDiscipline;
                }

                if (updateDto.Date != attestation.Date)
                {
                    attestation.Date = updateDto.Date;
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    _logger.LogWarning("{Function}: конфликт версий при сохранении uuid={Uuid}", functionName, uuid);
                    return Conflict(new ApiError
                    {
                        StatusCode = "1.0.3",
                        Title = "Конфликт версий",
                        Message = "Данные были изменены кем-то другим. Попробуйте обновить и повторить запрос.",
                        Field = string.Empty
                    });
                }

                _logger.LogInformation("{Function}: обновлена запись uuid={Uuid}", functionName, uuid);
                return Ok(new AttestationsResponseDto(attestation));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Function}: неожиданная ошибка при обновлении аттестации", functionName);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
                {
                    StatusCode = "1.0.0",
                    Title = "Внутренняя ошибка сервера",
                    Message = "Произошла ошибка на сервере",
                });
            }
        }
    }
}
