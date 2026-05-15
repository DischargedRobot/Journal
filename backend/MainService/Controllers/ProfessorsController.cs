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
    public class ProfessorsController : ControllerBase
    {
        private readonly MainServiceContext _context;

        public ProfessorsController(MainServiceContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Преподаватели найдены", typeof(PagedResult<ProfessorsResponseDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Преподаватели не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Получить список преподавателей",
            Description = "Возвращает список преподавателей с пагинацией"
        )]
        public async Task<ActionResult<PagedResult<ProfessorsResponseDto>>> GetProfessors(
            [FromQuery, SwaggerParameter("Количество записей")]
            int size = 100,
            [FromQuery, SwaggerParameter("Сдвиг от начала")]
            int offset = 0,
            [FromQuery, SwaggerParameter("ФИО")]
            string? filterFullName = null,
            [FromQuery, SwaggerParameter("Порядок сортировки по фамилии")]
            SortOrder sortOrder = SortOrder.Ascending
        )
        {
            IQueryable<Professors> baseQuery = _context.Professors
                .Where(p => filterFullName == null
                    || p.UniversityEmployer!.FirstName.Contains(filterFullName)
                    || p.UniversityEmployer.LastName.Contains(filterFullName));

            Task<int> totalTask = baseQuery.CountAsync();
            Task<List<ProfessorsResponseDto>> listTask = baseQuery
                .SortByKey(p => p.UniversityEmployer!.LastName, sortOrder)
                .TakeWithOffset(offset, size)
                .Select(p => new ProfessorsResponseDto
                {
                    Uuid = p.Uuid,
                    DepartmentUuid = p.Department!.Uuid,
                    PostUuid = p.Post!.Uuid,
                    PostName = p.Post.Name,
                    AcademicYearUuid = p.AcademicYear!.Uuid,
                    FirstName = p.UniversityEmployer!.FirstName,
                    LastName = p.UniversityEmployer.LastName,
                    Patronymic = p.UniversityEmployer.Patronymic ?? string.Empty,
                    GroupCuratorUuids = p.GroupCurator!.Select(g => g.Uuid).ToArray(),
                    DisciplinesUuids = p.Disciplines!.Select(d => d.Uuid).ToArray(),
                    Version = p.Version
                })
                .ToListAsync();

            List<ProfessorsResponseDto> items = await listTask;
            int total = await totalTask;

            if (total == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Преподаватели не найдены",
                    Message = "В системе не найдено ни одного преподавателя",
                    Field = string.Empty
                });
            }

            if (items.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Преподаватели не найдены",
                    Message = "Для указанных параметров запроса преподаватели не найдены",
                    Field = string.Empty
                });
            }

            return Ok(new PagedResult<ProfessorsResponseDto>(
                Total: total,
                Offset: offset,
                Size: size,
                Items: items
            ));
        }

        [HttpGet("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Преподаватель найден", typeof(ProfessorsResponseDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Преподаватель не найден", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Получить преподавателя по UUID",
            Description = "Возвращает преподавателя по указанному UUID"
        )]
        public async Task<ActionResult<ProfessorsResponseDto>> GetProfessor(
            [SwaggerParameter("UUID преподавателя")]
            Guid uuid
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID преподавателя не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            ProfessorsResponseDto? professor = await _context.Professors
                .Where(p => p.Uuid == uuid)
                .Select(p => new ProfessorsResponseDto
                {
                    Uuid = p.Uuid,
                    DepartmentUuid = p.Department!.Uuid,
                    PostUuid = p.Post!.Uuid,
                    PostName = p.Post.Name,
                    AcademicYearUuid = p.AcademicYear!.Uuid,
                    FirstName = p.UniversityEmployer!.FirstName,
                    LastName = p.UniversityEmployer.LastName,
                    Patronymic = p.UniversityEmployer.Patronymic ?? string.Empty,
                    GroupCuratorUuids = p.GroupCurator!.Select(g => g.Uuid).ToArray(),
                    DisciplinesUuids = p.Disciplines!.Select(d => d.Uuid).ToArray(),
                    Version = p.Version
                })
                .FirstOrDefaultAsync();

            if (professor == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Преподаватель не найден",
                    Message = $"Преподаватель с UUID \"{uuid}\" не найден",
                    Field = nameof(uuid)
                });
            }

            return Ok(professor);
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, "Преподаватель успешно создан", typeof(ProfessorsResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Некорректные данные", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Создать нового преподавателя"
        )]
        public async Task<ActionResult<ProfessorsResponseDto>> CreateProfessor(
            [FromBody, SwaggerParameter("Данные нового преподавателя")]
            ProfessorsCreateDto createDto
        )
        {
            if (string.IsNullOrWhiteSpace(createDto.FirstName))
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "Имя не может быть пустым",
                    Field = nameof(createDto.FirstName)
                });
            }

            if (string.IsNullOrWhiteSpace(createDto.LastName))
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "Фамилия не может быть пустой",
                    Field = nameof(createDto.LastName)
                });
            }

            if (createDto.AcademicYearUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "AcademicYearUuid не может быть пустым",
                    Field = nameof(createDto.AcademicYearUuid)
                });
            }

            AcademicYears? academicYear = await _context.AcademicYears
                .FirstOrDefaultAsync(a => a.Uuid == createDto.AcademicYearUuid);
            if (academicYear == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Некорректные данные",
                    Message = "Учебный год с указанным UUID не найден",
                    Field = nameof(createDto.AcademicYearUuid)
                });
            }

            Departments? department = null;
            if (createDto.DepartmentUuid != null && createDto.DepartmentUuid != Guid.Empty)
            {
                department = await _context.Departments
                    .FirstOrDefaultAsync(d => d.Uuid == createDto.DepartmentUuid.Value);
                if (department == null)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Некорректные данные",
                        Message = "Кафедра с указанным UUID не найдена",
                        Field = nameof(createDto.DepartmentUuid)
                    });
                }
            }

            EmployeePosts? post = null;
            if (createDto.PostUuid != null && createDto.PostUuid != Guid.Empty)
            {
                post = await _context.EmployeePosts
                    .FirstOrDefaultAsync(p => p.Uuid == createDto.PostUuid.Value);
                if (post == null)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Некорректные данные",
                        Message = "Должность с указанным UUID не найдена",
                        Field = nameof(createDto.PostUuid)
                    });
                }
            }


            Users newUser = new()
            {
                Uuid = Guid.NewGuid(),
                UserUuid = Guid.NewGuid().ToString(),
                Role = createDto.Role
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            UniversityEmployers newEmployee = new()
            {
                Uuid = Guid.NewGuid(),
                UserId = newUser.UserId,
                FirstName = createDto.FirstName.Trim(),
                LastName = createDto.LastName.Trim(),
                Patronymic = string.IsNullOrWhiteSpace(createDto.Patronymic) ? null : createDto.Patronymic.Trim(),
                User = newUser
            };

            _context.UniversityEmployers.Add(newEmployee);
            await _context.SaveChangesAsync();

            Professors newProfessor = new()
            {
                Uuid = Guid.NewGuid(),
                UniversityEmployerId = newEmployee.UniversityEmployerId,
                AcademicYearId = academicYear.AcademicYearId,
                DepartmentId = department?.DepartmentId,
                PostId = post?.PostId
            };

            _context.Professors.Add(newProfessor);
            await _context.SaveChangesAsync();

            newProfessor.UniversityEmployer = newEmployee;
            newProfessor.AcademicYear = academicYear;
            newProfessor.Department = department;
            newProfessor.Post = post;

            return CreatedAtAction(
                nameof(GetProfessor),
                new { uuid = newProfessor.Uuid },
                new ProfessorsResponseDto(newProfessor)
            );
        }

        [HttpDelete("{uuid}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Преподаватель удалён")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Преподаватель не найден", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Удалить преподавателя по UUID"
        )]
        public async Task<IActionResult> DeleteProfessor(
            [SwaggerParameter("UUID преподавателя")]
            Guid uuid
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID преподавателя не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            Professors? professor = await _context.Professors
                .FirstOrDefaultAsync(p => p.Uuid == uuid);

            if (professor == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Преподаватель не найден",
                    Message = $"Преподаватель с UUID \"{uuid}\" не найден",
                    Field = nameof(uuid)
                });
            }

            _context.Professors.Remove(professor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Преподаватель обновлён", typeof(ProfessorsResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Преподаватель не найден", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Обновить преподавателя",
            Description = "Обновляет данные преподавателя по его UUID. Все поля необязательны"
        )]
        public async Task<ActionResult<ProfessorsResponseDto>> UpdateProfessor(
            [SwaggerParameter("UUID преподавателя")]
            Guid uuid,
            [FromBody, SwaggerParameter("Данные для обновления")]
            ProfessorsUpdateDto updateDto
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID преподавателя не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            if (updateDto.FirstName != null && updateDto.FirstName.Trim() == string.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "Имя не может быть пустым",
                    Field = nameof(updateDto.FirstName)
                });
            }

            if (updateDto.LastName != null && updateDto.LastName.Trim() == string.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "Фамилия не может быть пустой",
                    Field = nameof(updateDto.LastName)
                });
            }

            if (updateDto.AcademicYearUuid != null && updateDto.AcademicYearUuid.Value == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "AcademicYearUuid не может быть пустым",
                    Field = nameof(updateDto.AcademicYearUuid)
                });
            }

            Professors? professor = await _context.Professors
                .Include(p => p.UniversityEmployer)
                    .ThenInclude(e => e!.User)
                .Include(p => p.AcademicYear)
                .Include(p => p.Department)
                .Include(p => p.Post)
                .FirstOrDefaultAsync(p => p.Uuid == uuid);

            if (professor == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Преподаватель не найден",
                    Message = $"Преподаватель с UUID \"{uuid}\" не найден",
                    Field = nameof(uuid)
                });
            }

            if (updateDto.FirstName != null)
            {
                professor.UniversityEmployer!.FirstName = updateDto.FirstName.Trim();
            }

            if (updateDto.LastName != null)
            {
                professor.UniversityEmployer!.LastName = updateDto.LastName.Trim();
            }

            if (updateDto.Patronymic != null)
            {
                professor.UniversityEmployer!.Patronymic = updateDto.Patronymic.Trim() == string.Empty
                    ? null
                    : updateDto.Patronymic.Trim();
            }

            if (updateDto.AcademicYearUuid != null)
            {
                AcademicYears? academicYear = await _context.AcademicYears
                    .FirstOrDefaultAsync(a => a.Uuid == updateDto.AcademicYearUuid.Value);
                if (academicYear == null)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Некорректные данные",
                        Message = "Учебный год с указанным UUID не найден",
                        Field = nameof(updateDto.AcademicYearUuid)
                    });
                }
                professor.AcademicYearId = academicYear.AcademicYearId;
                professor.AcademicYear = academicYear;
            }

            if (updateDto.DepartmentUuid != null)
            {
                if (updateDto.DepartmentUuid.Value == Guid.Empty)
                {
                    professor.DepartmentId = null;
                    professor.Department = null;
                }
                else
                {
                    Departments? department = await _context.Departments
                        .FirstOrDefaultAsync(d => d.Uuid == updateDto.DepartmentUuid.Value);
                    if (department == null)
                    {
                        return BadRequest(new ApiError
                        {
                            StatusCode = "1.2.3",
                            Title = "Некорректные данные",
                            Message = "Кафедра с указанным UUID не найдена",
                            Field = nameof(updateDto.DepartmentUuid)
                        });
                    }
                    professor.DepartmentId = department.DepartmentId;
                    professor.Department = department;
                }
            }

            if (updateDto.PostUuid != null)
            {
                if (updateDto.PostUuid.Value == Guid.Empty)
                {
                    professor.PostId = null;
                    professor.Post = null;
                }
                else
                {
                    EmployeePosts? post = await _context.EmployeePosts
                        .FirstOrDefaultAsync(p => p.Uuid == updateDto.PostUuid.Value);
                    if (post == null)
                    {
                        return BadRequest(new ApiError
                        {
                            StatusCode = "1.2.3",
                            Title = "Некорректные данные",
                            Message = "Должность с указанным UUID не найдена",
                            Field = nameof(updateDto.PostUuid)
                        });
                    }
                    professor.PostId = post.PostId;
                    professor.Post = post;
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new ProfessorsResponseDto(professor));
        }
    }
}