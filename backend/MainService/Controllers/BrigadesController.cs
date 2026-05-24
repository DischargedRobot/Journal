using MainService.Errors;
using MainService.Lib.Utils;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace MainService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BrigadesController : ControllerBase
    {
        private readonly MainServiceContext _context;
        private readonly ILogger<BrigadesController> _logger;
        private readonly System.Diagnostics.ActivitySource _activitySource;

        public BrigadesController(MainServiceContext context, ILogger<BrigadesController> logger, System.Diagnostics.ActivitySource activitySource)
        {
            _context = context;
            _logger = logger;
            _activitySource = activitySource;
        }


        [HttpGet("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Бригада найдена", typeof(BrigadesResponseDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Бригада не найдена", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Получить бригаду по UUID",
            Description = "Возвращает бригаду по указанному UUID"
        )]
        public async Task<ActionResult<BrigadesResponseDto>> GetBrigade(
            [SwaggerParameter("UUID бригады")]
            Guid uuid)
        {
            string functionName = ControllerContext.ActionDescriptor.ActionName;
            using var activity = _activitySource.StartAndLog(_logger, this);
            _logger.LogInformation("{Function}: GetBrigade uuid={Uuid}", functionName, uuid);

            // проверка запроса клиента
            if (uuid == Guid.Empty)
            {
                _logger.LogWarning("{Function}: пустой uuid", functionName);
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID бригады не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            BrigadesResponseDto? brigade = await _context.Brigades
            .Where(b => b.Uuid == uuid)
            .Select(b => new BrigadesResponseDto(b))
                .FirstOrDefaultAsync(b => b.Uuid == uuid);

            // проверка ответа БД
            if (brigade == null)
            {
                _logger.LogInformation("{Function}: бригада не найдена uuid={Uuid}", functionName, uuid);
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Бригада не найдена",
                    Message = $"Бригада с UUID \"{uuid}\" не найдена",
                    Field = nameof(uuid)
                });
            }


            _logger.LogInformation("{Function}: возвращаем бригаду uuid={Uuid}", functionName, uuid);
            return Ok(brigade);
        }


        [HttpGet("group/{groupUuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Бригады найдены", typeof(IEnumerable<BrigadesResponseDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Бригады не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Получить бригады по группе",
            Description = "Возвращает список бригад для указанной группы"
        )]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(BrigadesResponseDto))]
        public async Task<ActionResult<IEnumerable<BrigadesResponseDto>>> GetBrigadesByGroup(
            [SwaggerParameter("UUID группы")]
            Guid groupUuid
        )
        {
            string functionName = ControllerContext.ActionDescriptor.ActionName;
            using var activity = _activitySource.StartAndLog(_logger, this);
            _logger.LogInformation("{Function}: GetBrigadesByGroup groupUuid={GroupUuid}", functionName, groupUuid);

            // проверка запроса клиента
            if (groupUuid == Guid.Empty)
            {
                _logger.LogWarning("{Function}: пустой groupUuid", functionName);
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID группы не может быть пустым",
                    Field = nameof(groupUuid)
                });
            }

            Groups? group = await _context.Groups.FirstOrDefaultAsync(g => g.Uuid == groupUuid);
            // проверка ответа БД
            if (group == null)
            {
                _logger.LogInformation("{Function}: группа не найдена groupUuid={GroupUuid}", functionName, groupUuid);
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Группа не найдена",
                    Message = $"Группа с UUID \"{groupUuid}\" не найдена",
                    Field = nameof(groupUuid)
                });
            }

            List<BrigadesResponseDto> brigades = await _context.Brigades
                .Where(b => b.GroupId == group.GroupId)
                .Select(b => new BrigadesResponseDto
                {
                    Uuid = b.Uuid,
                    Name = b.Name,
                    IsTemplateForGroup = b.IsTemplateForGroup,
                    GroupUuid = b.Group != null ? b.Group.Uuid : null,
                    StudentsUuids = b.Students.Select(s => s.Uuid).ToArray(), // может не транслироваться
                    DisciplinesUuids = b.Disciplines!.Select(d => d.Uuid).ToArray(),
                    Version = b.Version
                })
                .AsNoTracking()
                .ToListAsync();

            // проверка ответа БД
            if (brigades.Count == 0)
            {
                _logger.LogInformation("{Function}: бригады не найдены для groupUuid={GroupUuid}", functionName, groupUuid);
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Бригады не найдены",
                    Message = $"Бригады для группы с UUID \"{groupUuid}\" не найдены",
                    Field = nameof(groupUuid)
                });
            }

            _logger.LogInformation("{Function}: найдено {Count} бригад для groupUuid={GroupUuid}", functionName, brigades.Count, groupUuid);
            return Ok(brigades);
        }


        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, "Бригада успешно создана", typeof(BrigadesResponseDto))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(BrigadesResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Некорректные данные", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Создать новую бригаду"
        )]
        public async Task<ActionResult<BrigadesResponseDto>> CreateBrigade(
            [FromBody, SwaggerParameter("Данные новой бригады")]
            BrigadesCreateDto createDto
        )
        {
            {
                string functionName = ControllerContext.ActionDescriptor.ActionName;
                using var activity = _activitySource.StartAndLog(_logger, this);
                _logger.LogInformation("{Function}: CreateBrigade", functionName);

                // проверка запроса клиента
                if (string.IsNullOrWhiteSpace(createDto.Name))
                {
                    _logger.LogWarning("{Function}: пустое имя бригады", functionName);
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.0",
                        Title = "Неверный запрос",
                        Message = "Название бригады не может быть пустым",
                        Field = nameof(createDto.Name)
                    });
                }

                if (createDto.GroupUuid == Guid.Empty)
                {
                    _logger.LogWarning("{Function}: пустой GroupUuid", functionName);
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.0",
                        Title = "Неверный запрос",
                        Message = "GroupUuid не может быть пустым",
                        Field = nameof(createDto.GroupUuid)
                    });
                }

                if (createDto.StudentsUuids == null || createDto.StudentsUuids.Length == 0)
                {
                    _logger.LogWarning("{Function}: нет студентов в createDto", functionName);
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.0",
                        Title = "Неверный запрос",
                        Message = "Необходимо указать хотя бы одного студента",
                        Field = nameof(createDto.StudentsUuids)
                    });
                }

                Groups? group = await _context.Groups.FirstOrDefaultAsync(g => g.Uuid == createDto.GroupUuid);
                // проверка ответа БД
                if (group == null)
                {
                    _logger.LogInformation("{Function}: группа не найдена groupUuid={GroupUuid}", functionName, createDto.GroupUuid);
                    return BadRequest(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Некорректные данные",
                        Message = "Группа с указанным UUID не найдена",
                        Field = nameof(createDto.GroupUuid)
                    });
                }

                List<Students> students = await _context.Students
                    .Where(s => createDto.StudentsUuids.Contains(s.Uuid))
                    .ToListAsync();

                if (students.Count != createDto.StudentsUuids.Length)
                {
                    Guid[] notFoundStudents = createDto.StudentsUuids.Except(students.Select(s => s.Uuid)).ToArray();
                    _logger.LogWarning("{Function}: некоторые студенты не найдены: {Missing}", functionName, string.Join(", ", notFoundStudents));
                    return BadRequest(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Некорректные данные",
                        Message = "Один или несколько студентов с указанными UUID не найдены",
                        Details = string.Join(", ", notFoundStudents),
                        Field = nameof(createDto.StudentsUuids)
                    });
                }

                List<Disciplines> disciplines = new();
                if (createDto.DisciplinesUuids != null && createDto.DisciplinesUuids.Length > 0)
                {
                    disciplines = await _context.Disciplines
                        .Where(d => createDto.DisciplinesUuids.Contains(d.Uuid))
                        .ToListAsync();

                    if (disciplines.Count != createDto.DisciplinesUuids.Length)
                    {
                        Guid[] notFoundDisciplines = createDto.DisciplinesUuids.Except(disciplines.Select(d => d.Uuid)).ToArray();
                        _logger.LogWarning("{Function}: некоторые дисциплины не найдены: {Missing}", functionName, string.Join(", ", notFoundDisciplines));
                        return BadRequest(new ApiError
                        {
                            StatusCode = "1.2.3",
                            Title = "Некорректные данные",
                            Message = "Одна или несколько дисциплин с указанными UUID не найдены",
                            Details = string.Join(", ", notFoundDisciplines),
                            Field = nameof(createDto.DisciplinesUuids)
                        });
                    }
                }

                Brigades newBrigade = new()
                {
                    Uuid = Guid.NewGuid(),
                    Name = createDto.Name.Trim(),
                    IsTemplateForGroup = createDto.IsTemplateForGroup,
                    GroupId = group.GroupId,
                    Students = students,
                    Disciplines = disciplines
                };

                _context.Brigades.Add(newBrigade);
                await _context.SaveChangesAsync();

                newBrigade.Group = group;

                _logger.LogInformation("{Function}: создана бригада uuid={Uuid}", functionName, newBrigade.Uuid);
                return CreatedAtAction(
                    nameof(GetBrigade),
                    new { uuid = newBrigade.Uuid },
                    new BrigadesResponseDto(newBrigade)
                );
            }

        }
        [HttpDelete("{uuid}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Бригада удалена")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Бригада не найдена", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Удалить бригаду по UUID"
        )]
        public async Task<IActionResult> DeleteBrigade(
            [SwaggerParameter("UUID бригады")]
            Guid uuid
        )
        {
            string functionName = ControllerContext.ActionDescriptor.ActionName;
            using var activity = _activitySource.StartAndLog(_logger, this);
            _logger.LogInformation("{Function}: DeleteBrigade uuid={Uuid}", functionName, uuid);

            // проверка запроса клиента
            if (uuid == Guid.Empty)
            {
                _logger.LogWarning("{Function}: пустой uuid", functionName);
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID бригады не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            Brigades? brigade = await _context.Brigades.FirstOrDefaultAsync(b => b.Uuid == uuid);

            // проверка ответа БД
            if (brigade == null)
            {
                _logger.LogInformation("{Function}: бригада не найдена uuid={Uuid}", functionName, uuid);
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Бригада не найдена",
                    Message = $"Бригада с UUID \"{uuid}\" не найдена",
                    Field = nameof(uuid)
                });
            }

            _context.Brigades.Remove(brigade);
            await _context.SaveChangesAsync();

            _logger.LogInformation("{Function}: бригада удалена uuid={Uuid}", functionName, uuid);
            return NoContent();
        }

        [HttpPatch("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Бригада обновлена", typeof(BrigadesResponseDto))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(BrigadesResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Бригада не найдена", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Обновить бригаду",
            Description = "Обновляет данные бригады по её UUID. Все поля необязательны"
        )]
        public async Task<ActionResult<BrigadesResponseDto>> UpdateBrigade(
            [SwaggerParameter("UUID бригады")]
            Guid uuid,
            [FromBody, SwaggerParameter("Данные для обновления")]
            BrigadesUpdateDto updateDto
        )
        {
            string functionName = ControllerContext.ActionDescriptor.ActionName;
            using var activity = _activitySource.StartAndLog(_logger, this);
            _logger.LogInformation("{Function}: UpdateBrigade uuid={Uuid}", functionName, uuid);

            // проверка запроса клиента
            if (uuid == Guid.Empty)
            {
                _logger.LogWarning("{Function}: пустой uuid", functionName);
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID бригады не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            if (updateDto.Name != null && updateDto.Name.Trim() == string.Empty)
            {
                _logger.LogWarning("{Function}: пустое имя в updateDto", functionName);
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "Название бригады не может быть пустым",
                    Field = nameof(updateDto.Name)
                });
            }

            if (updateDto.GroupUuid != null && updateDto.GroupUuid == Guid.Empty)
            {
                _logger.LogWarning("{Function}: пустой GroupUuid в updateDto", functionName);
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "GroupUuid не может быть пустым",
                    Field = nameof(updateDto.GroupUuid)
                });
            }

            if (updateDto.StudentsUuids != null && updateDto.StudentsUuids.Length == 0)
            {
                _logger.LogWarning("{Function}: пустой список студентов в updateDto", functionName);
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "Необходимо указать хотя бы одного студента",
                    Field = nameof(updateDto.StudentsUuids)
                });
            }

            // проверка ответа БД
            Groups? group = null;
            List<Students>? students = null;
            List<Disciplines>? disciplines = null;

            if (updateDto.GroupUuid != null)
            {
                group = await _context.Groups.FirstOrDefaultAsync(g => g.Uuid == updateDto.GroupUuid);
                if (group == null)
                {
                    _logger.LogInformation("{Function}: группа не найдена groupUuid={GroupUuid}", functionName, updateDto.GroupUuid);
                    return BadRequest(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Некорректные данные",
                        Message = "Группа с указанным UUID не найдена",
                        Field = nameof(updateDto.GroupUuid)
                    });
                }
            }

            if (updateDto.StudentsUuids != null)
            {
                students = await _context.Students
                    .Where(s => updateDto.StudentsUuids.Contains(s.Uuid))
                    .ToListAsync();

                if (students.Count != updateDto.StudentsUuids.Length)
                {
                    Guid[] notFoundStudents = updateDto.StudentsUuids.Except(students.Select(s => s.Uuid)).ToArray();
                    _logger.LogWarning("{Function}: некоторые студенты не найдены: {Missing}", functionName, string.Join(", ", notFoundStudents));
                    return BadRequest(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Некорректные данные",
                        Message = "Один или несколько студентов с указанными UUID не найдены",
                        Details = string.Join(", ", notFoundStudents),
                        Field = nameof(updateDto.StudentsUuids)
                    });
                }
            }

            if (updateDto.DisciplinesUuids != null)
            {
                disciplines = await _context.Disciplines
                    .Where(d => updateDto.DisciplinesUuids.Contains(d.Uuid))
                    .ToListAsync();

                if (disciplines.Count != updateDto.DisciplinesUuids.Length)
                {
                    Guid[] notFoundDisciplines = updateDto.DisciplinesUuids.Except(disciplines.Select(d => d.Uuid)).ToArray();
                    _logger.LogWarning("{Function}: некоторые дисциплины не найдены: {Missing}", functionName, string.Join(", ", notFoundDisciplines));
                    return BadRequest(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Некорректные данные",
                        Message = "Одна или несколько дисциплин с указанными UUID не найдены",
                        Details = string.Join(", ", notFoundDisciplines),
                        Field = nameof(updateDto.DisciplinesUuids)
                    });
                }
            }

            Brigades? brigade = await _context.Brigades
                .Include(b => b.Group)
                .Include(b => b.Students)
                .Include(b => b.Disciplines)
                .FirstOrDefaultAsync(b => b.Uuid == uuid);

            if (brigade == null)
            {
                _logger.LogInformation("{Function}: бригада не найдена uuid={Uuid}", functionName, uuid);
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Бригада не найдена",
                    Message = $"Бригада с UUID \"{uuid}\" не найдена",
                    Field = nameof(uuid)
                });
            }

            if (updateDto.Name != null)
            {
                brigade.Name = updateDto.Name.Trim();
            }

            if (group != null)
            {
                brigade.GroupId = group.GroupId;
                brigade.Group = group;
            }

            if (students != null)
            {
                brigade.Students = students;
            }

            if (disciplines != null)
            {
                brigade.Disciplines = disciplines;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("{Function}: бригада обновлена uuid={Uuid}", functionName, uuid);
            return Ok(new BrigadesResponseDto(brigade));
        }
    }
}
