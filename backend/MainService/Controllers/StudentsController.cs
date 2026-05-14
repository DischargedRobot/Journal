using MainService.Errors;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

using MainService.Enums;

namespace MainService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly MainServiceContext _context;

        public StudentsController(MainServiceContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Студенты найдены", typeof(PagedResult<StudentsResponseDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Студенты не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Получить список студентов",
            Description = "Возвращает список студентов по заданным параметрам"
        )]
        public async Task<ActionResult<PagedResult<StudentsResponseDto>>> GetStudents(
            [FromQuery, SwaggerParameter("Количество записей")]
            int size = 50,
            [FromQuery, SwaggerParameter("Смещение от начала списка")]
            int offset = 0,
            [FromQuery, SwaggerParameter("ФИО")]
            string? filterFullName = null,
            [FromQuery, SwaggerParameter("Порядок сортировки по идентификатору студента")]
            SortOrder sortOrder = SortOrder.Ascending
        )
        {
            IQueryable<Students> baseQuery = _context.Students
                .Where(s => filterFullName == null
                    || s.StudentPerson!.User!.FirstName.Contains(filterFullName)
                    || s.StudentPerson.User.LastName.Contains(filterFullName)
                    || (s.StudentPerson.User.Patronymic != null && s.StudentPerson.User.Patronymic.Contains(filterFullName)))
                .Include(s => s.Group)
                .Include(s => s.StudentPerson)
                .AsNoTracking();

            Task<int> totalTask = baseQuery.CountAsync();
            IQueryable<StudentsResponseDto> itemsQuery = baseQuery
                .SortByKey(s => s.StudentId, sortOrder)
                .TakeWithOffset(offset, size)
                .Select(s => new StudentsResponseDto
                {
                    Uuid = s.Uuid,
                    StudentCode = s.StudentCode,
                    FirstName = s.StudentPerson!.User!.FirstName,
                    LastName = s.StudentPerson.User.LastName,
                    Patronymic = s.StudentPerson.User.Patronymic ?? string.Empty,
                    GroupUuid = s.Group!.Uuid,
                    BrigadesUuids = Array.Empty<Guid>(),
                    Version = s.Version
                });

            List<StudentsResponseDto> studentsDtoList = await itemsQuery.ToListAsync();
            int total = await totalTask;

            if (total == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Студенты не найдены",
                    Message = "В системе не найдено ни одного студента",
                    Field = string.Empty
                });
            }

            if (studentsDtoList.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Студенты не найдены",
                    Message = "В системе не найдено ни одного студента для указанных параметров запроса",
                    Field = string.Empty
                });
            }

            return Ok(new PagedResult<StudentsResponseDto>(total, offset, studentsDtoList.Count, studentsDtoList));
        }

        [HttpGet("{Uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Студент найден", typeof(StudentsResponseDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Студент не найден", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Получить студента по ID",
            Description = "Возвращает студента по указанному ID"
        )]
        public async Task<IActionResult> GetStudent(Guid uuid)
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID студента не может быть пустым",
                    Field = nameof(uuid)
                });
            }
            Students? student = await _context.Students.FindAsync(uuid);
            if (student == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Студент не найден",
                    Message = $"Студент с UUID \"{uuid}\" не найден",
                    Field = nameof(uuid)
                });
            }
            return Ok(new StudentsResponseDto(student));
        }

        // [JsonConverter(typeof(JsonStringEnumConverter))]
        // public enum SortBy
        // {
        //     Group,
        //     FullName,
        // }

        [HttpGet("group/{groupUuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Студенты найдены", typeof(PagedResult<StudentsResponseDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Студенты не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Получить студентов по группе",
            Description = "Возвращает список студентов по указанной группе"
        )]
        public async Task<ActionResult<PagedResult<StudentsResponseDto>>> GetStudentsByGroup(
            Guid groupUuid,
            [FromQuery] int offset = 0,
            [FromQuery] int size = 50,
            [FromQuery, SwaggerParameter("ФИО")]
            string? filterFullName = null,
            [FromQuery, SwaggerParameter("Порядок сортировки по идентификатору студента")] SortOrder sortOrder = SortOrder.Ascending
        // [FromQuery] SortBy[]? sortBy = null,
        // [FromQuery] SortOrder[]? sortOrder = null
        )
        {
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
            Groups? group = await _context.Groups
            .FirstOrDefaultAsync(g => g.Uuid == groupUuid);
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

            if (offset < 0)
            {
                offset = 0;
            }

            if (size <= 0)
            {
                size = 50;
            }

            IQueryable<Students> baseQuery = _context.Students
                .Where(s => s.GroupId == group.GroupId
                // TODO: подумать нужен ли тут фильтр и как именно будет идти фильтраци на клиенте
                // (каждый ввод = запрос или по кнопке)
                && (filterFullName == null
                || s.StudentPerson!.User!.FirstName.Contains(filterFullName)
                || s.StudentPerson.User.LastName.Contains(filterFullName)
                || (s.StudentPerson.User.Patronymic != null && s.StudentPerson.User.Patronymic.Contains(filterFullName))))
                .Include(s => s.Group)
                .Include(s => s.StudentPerson)
                .AsNoTracking();

            Task<int> totalTask = baseQuery.CountAsync();
            IQueryable<StudentsResponseDto> itemsQuery = baseQuery
                .SortByKey(s => s.StudentId, sortOrder)
                .TakeWithOffset(offset, size)
                .Select(s => new StudentsResponseDto
                {
                    Uuid = s.Uuid,
                    StudentCode = s.StudentCode,
                    FirstName = s.StudentPerson!.User!.FirstName,
                    LastName = s.StudentPerson.User.LastName,
                    Patronymic = s.StudentPerson.User.Patronymic ?? string.Empty,
                    GroupUuid = s.Group!.Uuid,
                    BrigadesUuids = Array.Empty<Guid>(),
                    Version = s.Version
                });

            List<StudentsResponseDto> studentsDtoList = await itemsQuery.ToListAsync();
            int total = await totalTask;

            if (studentsDtoList.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Студенты не найдены",
                    Message = $"Студенты в группе с UUID \"{groupUuid}\" не найдены",
                    Field = nameof(groupUuid)
                });
            }

            return Ok(new PagedResult<StudentsResponseDto>(total, offset, studentsDtoList.Count, studentsDtoList));
        }


        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, "Студент успешно создан", typeof(StudentsResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Некорректные данные", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Создать нового студента"
        )]
        public async Task<ActionResult<StudentsResponseDto>> CreateStudent(
            [FromBody, SwaggerParameter("Данные нового студента")]
            StudentsCreateDto createDto
        )
        {
            if (createDto.GroupUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "Имя студента не может быть пустым",
                    Field = nameof(createDto.FirstName)
                });
            }

            if (string.IsNullOrWhiteSpace(createDto.LastName))
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "Фамилия студента не может быть пустой",
                    Field = nameof(createDto.LastName)
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

            Groups? group = await _context.Groups.FirstOrDefaultAsync(g => g.Uuid == createDto.GroupUuid);
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

            if (createDto.StudentCode != null)
            {
                bool codeExists = await _context.Students.AnyAsync(s => s.StudentCode == createDto.StudentCode.Value);
                if (codeExists)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.1",
                        Title = "Неверный запрос",
                        Message = $"Студент с кодом {createDto.StudentCode.Value} уже существует",
                        Field = nameof(createDto.StudentCode)
                    });
                }
            }

            Users newUser = new()
            {
                Uuid = Guid.NewGuid(),
                FirstName = createDto.FirstName.Trim(),
                LastName = createDto.LastName.Trim(),
                Patronymic = string.IsNullOrWhiteSpace(createDto.Patronymic) ? null : createDto.Patronymic.Trim(),
                UserUuid = Guid.NewGuid().ToString()
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            StudentPersons newStudentPerson = new()
            {
                Uuid = Guid.NewGuid(),
                UserId = newUser.UserId,
                User = newUser
            };

            _context.StudentPersons.Add(newStudentPerson);
            await _context.SaveChangesAsync();

            Students newStudent = new()
            {
                Uuid = Guid.NewGuid(),
                StudentCode = createDto.StudentCode ?? 0,
                StudentPersonId = newStudentPerson.StudentPersonId,
                StudentPerson = newStudentPerson,
                GroupId = group.GroupId
            };

            _context.Students.Add(newStudent);
            await _context.SaveChangesAsync();

            newStudent.Group = group;

            return CreatedAtAction(
                nameof(GetStudent),
                new { uuid = newStudent.Uuid },
                new StudentsResponseDto(newStudent)
            );
        }


        [HttpDelete("{uuid}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Студент удалён")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Студент не найден", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Удалить студента по идентификатору"
        )]
        public async Task<IActionResult> DeleteStudent(
            [SwaggerParameter("UUID студента")]
            Guid uuid
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID студента не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            Students? student = await _context.Students
                .Include(s => s.StudentPerson)
                    .ThenInclude(sp => sp!.User)
                .FirstOrDefaultAsync(s => s.Uuid == uuid);

            if (student == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Студент не найден",
                    Message = $"Студент с UUID \"{uuid}\" не найден",
                    Field = nameof(uuid)
                });
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPatch("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Студент обновлён", typeof(StudentsResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Студент не найден", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Обновить студента",
            Description = "Обновляет данные студента по его UUID. Все поля необязательны"
        )]
        public async Task<ActionResult<StudentsResponseDto>> UpdateStudent(
            [SwaggerParameter("UUID студента")]
            Guid uuid,
            [FromBody, SwaggerParameter("Данные для обновления")]
            StudentsUpdateDto updateDto
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID студента не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            if (updateDto.FirstName != null && updateDto.FirstName.Trim() == string.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "Имя студента не может быть пустым",
                    Field = nameof(updateDto.FirstName)
                });
            }

            if (updateDto.LastName != null && updateDto.LastName.Trim() == string.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "Фамилия студента не может быть пустой",
                    Field = nameof(updateDto.LastName)
                });
            }

            if (updateDto.GroupUuid != null && updateDto.GroupUuid.Value == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "GroupUuid не может быть пустым",
                    Field = nameof(updateDto.GroupUuid)
                });
            }

            // проверка ответа БД
            Groups? group = null;

            if (updateDto.StudentCode != null)
            {
                bool codeExists = await _context.Students.AnyAsync(s => s.StudentCode == updateDto.StudentCode.Value && s.Uuid != uuid);
                if (codeExists)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.1",
                        Title = "Неверный запрос",
                        Message = $"Студент с кодом {updateDto.StudentCode.Value} уже существует",
                        Field = nameof(updateDto.StudentCode)
                    });
                }
            }

            if (updateDto.GroupUuid != null)
            {
                group = await _context.Groups.FirstOrDefaultAsync(g => g.Uuid == updateDto.GroupUuid.Value);
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

            Students? student = await _context.Students
                .Include(s => s.StudentPerson)
                    .ThenInclude(sp => sp!.User)
                .Include(s => s.Group)
                .FirstOrDefaultAsync(s => s.Uuid == uuid);

            if (student == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Студент не найден",
                    Message = $"Студент с UUID \"{uuid}\" не найден",
                    Field = nameof(uuid)
                });
            }

            if (updateDto.StudentCode != null)
            {
                student.StudentCode = updateDto.StudentCode.Value;
            }

            if (updateDto.FirstName != null)
            {
                student.StudentPerson!.User!.FirstName = updateDto.FirstName.Trim();
            }

            if (updateDto.LastName != null)
            {
                student.StudentPerson!.User!.LastName = updateDto.LastName.Trim();
            }

            if (updateDto.Patronymic != null)
            {
                student.StudentPerson!.User!.Patronymic = updateDto.Patronymic.Trim() == string.Empty
                    ? null
                    : updateDto.Patronymic.Trim();
            }

            if (group != null)
            {
                student.GroupId = group.GroupId;
                student.Group = group;
            }

            await _context.SaveChangesAsync();

            return Ok(new StudentsResponseDto(student));
        }
    };
}
