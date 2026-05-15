using MainService.Errors;

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

        public BrigadesController(MainServiceContext context)
        {
            _context = context;
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
            // проверка запроса клиента
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID бригады не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            Brigades? brigadeEntity = await _context.Brigades
                .Include(b => b.Students)
                .Include(b => b.Disciplines)
                .Include(b => b.Group)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Uuid == uuid);

            // проверка ответа БД
            if (brigadeEntity == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Бригада не найдена",
                    Message = $"Бригада с UUID \"{uuid}\" не найдена",
                    Field = nameof(uuid)
                });
            }

            BrigadesResponseDto brigade = new(brigadeEntity);

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
        public async Task<ActionResult<IEnumerable<BrigadesResponseDto>>> GetBrigadesByGroup(
            [SwaggerParameter("UUID группы")]
            Guid groupUuid
        )
        {
            // проверка запроса клиента
            if (groupUuid == Guid.Empty)
            {
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
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Бригады не найдены",
                    Message = $"Бригады для группы с UUID \"{groupUuid}\" не найдены",
                    Field = nameof(groupUuid)
                });
            }

            return Ok(brigades);
        }


        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, "Бригада успешно создана", typeof(BrigadesResponseDto))]
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
            // проверка запроса клиента
            if (string.IsNullOrWhiteSpace(createDto.Name))
            {
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
                return BadRequest(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Некорректные данные",
                    Message = "Один или несколько студентов с указанными UUID не найдены",
                    Details = string.Join(", ", notFoundStudents),
                    Field = nameof(createDto.StudentsUuids)
                });
            }

            List<Disciplines> disciplines = [];
            if (createDto.DisciplinesUuids != null && createDto.DisciplinesUuids.Length > 0)
            {
                disciplines = await _context.Disciplines
                    .Where(d => createDto.DisciplinesUuids.Contains(d.Uuid))
                    .ToListAsync();

                if (disciplines.Count != createDto.DisciplinesUuids.Length)
                {
                    Guid[] notFoundDisciplines = createDto.DisciplinesUuids.Except(disciplines.Select(d => d.Uuid)).ToArray();
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

            return CreatedAtAction(
                nameof(GetBrigade),
                new { uuid = newBrigade.Uuid },
                new BrigadesResponseDto(newBrigade)
            );
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
            // проверка запроса клиента
            if (uuid == Guid.Empty)
            {
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

            return NoContent();
        }

        [HttpPatch("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Бригада обновлена", typeof(BrigadesResponseDto))]
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
            // проверка запроса клиента
            if (uuid == Guid.Empty)
            {
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

            if (updateDto.IsTemplateForGroup != brigade.IsTemplateForGroup)
            {
                brigade.IsTemplateForGroup = updateDto.IsTemplateForGroup;
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

            return Ok(new BrigadesResponseDto(brigade));
        }
    }
}
