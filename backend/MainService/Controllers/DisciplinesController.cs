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
    public class DisciplinesController : ControllerBase
    {
        private readonly MainServiceContext _context;

        public DisciplinesController(MainServiceContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Дисциплины найдены", typeof(PagedResult<DisciplinesResponseDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Дисциплины не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить список дисциплин",
            Description = "Возвращает список дисциплин с пагинацией"
        )]
        public async Task<ActionResult<PagedResult<DisciplinesResponseDto>>> GetDisciplines(
            [FromQuery, SwaggerParameter("Количество записей")]
            int size = 100,
            [FromQuery, SwaggerParameter("Сдвиг от начала")]
            int offset = 0,
            [FromQuery, SwaggerParameter("Название")]
            string? name = null,
            [FromQuery, SwaggerParameter("В архиве")]
            bool? isArchived = null,
            [FromQuery, SwaggerParameter("Порядок сортировки по названию")]
            SortOrder sortOrder = SortOrder.Ascending
        )
        {
            IQueryable<DisciplinesResponseDto> query = _context.Disciplines
                .Where(d => string.IsNullOrEmpty(name) || d.Name.Contains(name))
                .Where(d => isArchived == null || d.IsArchived == isArchived.Value)
                .SortByKey(d => d.Name, sortOrder)
                .TakeWithOffset(offset, size)
                .Select(d => new DisciplinesResponseDto
                {
                    Uuid = d.Uuid,
                    Name = d.Name,
                    ShortName = d.ShortName,
                    IsArchived = d.IsArchived,
                    DisciplineRegisterUuid = d.DisciplineRegister != null ? d.DisciplineRegister.Uuid : (Guid?)null,
                    SemesterUuid = d.Semester!.Uuid,
                    AcademicYearUuid = d.AcademicYear!.Uuid,
                    GroupsUuids = d.Groups.Select(g => g.Uuid).ToArray(),
                    ProfessorsUuids = d.Professors!.Select(p => p.Uuid).ToArray(),
                    Version = d.Version
                });

            Task<int> totalTask = _context.Disciplines.CountAsync();
            Task<List<DisciplinesResponseDto>> listTask = query.ToListAsync();

            List<DisciplinesResponseDto> disciplinesList = await listTask;
            int totalCount = await totalTask;

            if (totalCount == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.1",
                    Title = "Дисциплины не найдены",
                    Message = "В системе не найдено ни одной дисциплины"
                });
            }

            if (disciplinesList.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2",
                    Title = "Дисциплины не найдены",
                    Message = "В системе не найдено ни одной дисциплины для указанных параметров запроса"
                });
            }

            return Ok(new PagedResult<DisciplinesResponseDto>(
                Total: totalCount,
                Offset: offset,
                Size: size,
                Items: disciplinesList
            ));
        }

        [HttpGet("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Дисциплина найдена", typeof(DisciplinesResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Дисциплина не найдена", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить дисциплину по идентификатору",
            Description = "Возвращает одну дисциплину по её UUID"
        )]
        public async Task<ActionResult<DisciplinesResponseDto>> GetDiscipline(
            [SwaggerParameter("UUID дисциплины")]
            Guid uuid
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.1",
                    Title = "Неверный запрос",
                    Message = "UUID дисциплины не может быть пустым"
                });
            }

            DisciplinesResponseDto? discipline = await _context.Disciplines
                .Where(d => d.Uuid == uuid)
                .Select(d => new DisciplinesResponseDto
                {
                    Uuid = d.Uuid,
                    Name = d.Name,
                    ShortName = d.ShortName,
                    IsArchived = d.IsArchived,
                    DisciplineRegisterUuid = d.DisciplineRegister != null ? d.DisciplineRegister.Uuid : (Guid?)null,
                    SemesterUuid = d.Semester!.Uuid,
                    AcademicYearUuid = d.AcademicYear!.Uuid,
                    GroupsUuids = d.Groups.Select(g => g.Uuid).ToArray(),
                    ProfessorsUuids = d.Professors!.Select(p => p.Uuid).ToArray(),
                    Version = d.Version
                })
                .FirstOrDefaultAsync();

            if (discipline == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.1",
                    Title = "Дисциплина не найдена",
                    Message = $"Дисциплина с UUID \"{uuid}\" не найдена"
                });
            }

            return Ok(discipline);
        }


        [HttpGet("group/{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Дисциплины найдены", typeof(PagedResult<DisciplinesResponseDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Дисциплины не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить дисциплины по группе",
            Description = "Возвращает список дисциплин, которые читаются для указанной группы"
        )]
        public async Task<ActionResult<PagedResult<DisciplinesResponseDto>>> GetDisciplinesByGroup(
            [SwaggerParameter("UUID группы")]
            Guid uuid,
            [FromQuery, SwaggerParameter("Количество записей")]
            int size = 100,
            [FromQuery, SwaggerParameter("Сдвиг от начала")]
            int offset = 0,
            [FromQuery, SwaggerParameter("Название")]
            string? name = null,
            [FromQuery, SwaggerParameter("В архиве")]
            bool? isArchived = null,
            [FromQuery, SwaggerParameter("Сортировка по названию")]
            SortOrder sortOrder = SortOrder.Ascending
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.1",
                    Title = "Неверный запрос",
                    Message = "UUID группы не может быть пустым"
                });
            }

            IQueryable<Disciplines> baseQuery = _context.Disciplines
                .Where(d => d.Groups.Any(g => g.Uuid == uuid))
                .Where(d => string.IsNullOrEmpty(name) || d.Name.Contains(name))
                .Where(d => isArchived == null || d.IsArchived == isArchived);

            Task<int> totalTask = baseQuery.CountAsync();
            Task<List<DisciplinesResponseDto>> listTask = baseQuery
                .SortByKey(d => d.Name, sortOrder)
                .TakeWithOffset(offset, size)
                .Select(d => new DisciplinesResponseDto
                {
                    Uuid = d.Uuid,
                    Name = d.Name,
                    ShortName = d.ShortName,
                    IsArchived = d.IsArchived,
                    DisciplineRegisterUuid = d.DisciplineRegister != null ? d.DisciplineRegister.Uuid : (Guid?)null,
                    SemesterUuid = d.Semester!.Uuid,
                    AcademicYearUuid = d.AcademicYear!.Uuid,
                    GroupsUuids = d.Groups.Select(g => g.Uuid).ToArray(),
                    ProfessorsUuids = d.Professors!.Select(p => p.Uuid).ToArray(),
                    Version = d.Version
                })
                .ToListAsync();

            List<DisciplinesResponseDto> disciplines = await listTask;
            int totalCount = await totalTask;

            if (totalCount == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.1",
                    Title = "Дисциплины не найдены",
                    Message = $"Для группы с UUID \"{uuid}\" не найдено ни одной дисциплины"
                });
            }

            if (disciplines.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2",
                    Title = "Дисциплины не найдены",
                    Message = $"Для группы с UUID \"{uuid}\" не найдено дисциплин для указанных параметров запроса"
                });
            }

            return Ok(new PagedResult<DisciplinesResponseDto>(
                Total: totalCount,
                Offset: offset,
                Size: size,
                Items: disciplines
            ));
        }

        [HttpGet("professor/{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Дисциплины найдены", typeof(PagedResult<DisciplinesResponseDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Дисциплины не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить дисциплины по преподавателю",
            Description = "Возвращает список дисциплин, которые ведёт указанный преподаватель"
        )]
        public async Task<ActionResult<PagedResult<DisciplinesResponseDto>>> GetDisciplinesByProfessor(
            [SwaggerParameter("UUID преподавателя")]
            Guid uuid,
            [FromQuery, SwaggerParameter("Количество записей")]
            int size = 100,
            [FromQuery, SwaggerParameter("Сдвиг от начала")]
            int offset = 0,
            [FromQuery, SwaggerParameter("Название")]
            string? name = null,
            [FromQuery, SwaggerParameter("В архиве")]
            bool? isArchived = null,
            [FromQuery, SwaggerParameter("Сортировка по названию")]
            SortOrder sortOrder = SortOrder.Ascending
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.1",
                    Title = "Неверный запрос",
                    Message = "UUID преподавателя не может быть пустым"
                });
            }

            IQueryable<Disciplines> baseQuery = _context.Disciplines
                .Where(d => d.Professors!.Any(p => p.Uuid == uuid))
                .Where(d => string.IsNullOrEmpty(name) || d.Name.Contains(name))
                .Where(d => isArchived == null || d.IsArchived == isArchived);

            Task<int> totalTask = baseQuery.CountAsync();
            Task<List<DisciplinesResponseDto>> listTask = baseQuery
                .SortByKey(d => d.Name, sortOrder)
                .TakeWithOffset(offset, size)
                .Select(d => new DisciplinesResponseDto
                {
                    Uuid = d.Uuid,
                    Name = d.Name,
                    ShortName = d.ShortName,
                    IsArchived = d.IsArchived,
                    DisciplineRegisterUuid = d.DisciplineRegister != null ? d.DisciplineRegister.Uuid : (Guid?)null,
                    SemesterUuid = d.Semester!.Uuid,
                    AcademicYearUuid = d.AcademicYear!.Uuid,
                    GroupsUuids = d.Groups.Select(g => g.Uuid).ToArray(),
                    ProfessorsUuids = d.Professors!.Select(p => p.Uuid).ToArray(),
                    Version = d.Version
                })
                .ToListAsync();

            List<DisciplinesResponseDto> disciplines = await listTask;
            int totalCount = await totalTask;

            if (totalCount == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.1",
                    Title = "Дисциплины не найдены",
                    Message = $"Для преподавателя с UUID \"{uuid}\" не найдено ни одной дисциплины"
                });
            }

            if (disciplines.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2",
                    Title = "Дисциплины не найдены",
                    Message = $"Для преподавателя с UUID \"{uuid}\" не найдено дисциплин для указанных параметров запроса"
                });
            }

            return Ok(new PagedResult<DisciplinesResponseDto>(
                Total: totalCount,
                Offset: offset,
                Size: size,
                Items: disciplines
            ));
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, "Дисциплина успешно создана", typeof(DisciplinesResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Некорректные данные", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Создать новую дисциплину"
        )]
        public async Task<ActionResult<DisciplinesResponseDto>> CreateDiscipline(
            [FromBody, SwaggerParameter("Данные новой дисциплины")]
            DisciplinesCreateDto createDto
        )
        {
            if (string.IsNullOrWhiteSpace(createDto.Name))
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.1",
                    Title = "Неверный запрос",
                    Message = "Название дисциплины не может быть пустым"
                });
            }

            if (createDto.SemesterUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2",
                    Title = "Неверный запрос",
                    Message = "SemesterUuid обязателен"
                });
            }

            Semesters? semester = await _context.Semesters.FirstOrDefaultAsync(s => s.Uuid == createDto.SemesterUuid);
            if (semester == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "1.1",
                    Title = "Некорректные данные",
                    Message = "Семестр с указанным UUID не найден"
                });
            }

            if (createDto.AcademicYearUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.3",
                    Title = "Неверный запрос",
                    Message = "AcademicYearUuid обязателен"
                });
            }

            AcademicYears? academicYear = await _context.AcademicYears.FirstOrDefaultAsync(a => a.Uuid == createDto.AcademicYearUuid);
            if (academicYear == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "1.2",
                    Title = "Некорректные данные",
                    Message = "Учебный год с указанным UUID не найден"
                });
            }

            if (createDto.GroupsUuids == null || createDto.GroupsUuids.Length == 0)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.4",
                    Title = "Неверный запрос",
                    Message = "Необходимо указать хотя бы одну группу"
                });
            }

            DisciplinesRegisters? disciplineRegister = null;
            if (createDto.DisciplineRegisterUuid.HasValue && createDto.DisciplineRegisterUuid.Value != Guid.Empty)
            {
                disciplineRegister = await _context.DisciplinesRegisters.FirstOrDefaultAsync(r => r.Uuid == createDto.DisciplineRegisterUuid.Value);
                if (disciplineRegister == null)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "1.3",
                        Title = "Некорректные данные",
                        Message = "Реестр дисциплин с указанным UUID не найден"
                    });
                }
            }

            List<Groups> groups = await _context.Groups
                .Where(g => createDto.GroupsUuids.Contains(g.Uuid))
                .ToListAsync();

            if (groups.Count != createDto.GroupsUuids.Length)
            {
                Guid[] notFoundGroups = createDto.GroupsUuids.Except(groups.Select(g => g.Uuid)).ToArray();
                return BadRequest(new ApiError
                {
                    StatusCode = "1.4",
                    Title = "Некорректные данные",
                    Message = "Одна или несколько групп с указанными UUID не найдены",
                    Details = string.Join(", ", notFoundGroups)
                });
            }

            List<Professors> professors = [];
            if (createDto.ProfessorsUuids != null && createDto.ProfessorsUuids.Length > 0)
            {
                professors = await _context.Professors
                    .Where(p => createDto.ProfessorsUuids.Contains(p.Uuid))
                    .ToListAsync();
                if (professors.Count != createDto.ProfessorsUuids.Length)
                {
                    Guid[] notFoundProfessors = createDto.ProfessorsUuids.Except(professors.Select(p => p.Uuid)).ToArray();
                    return BadRequest(new ApiError
                    {
                        StatusCode = "1.6",
                        Title = "Некорректные данные",
                        Message = "Один или несколько преподавателей с указанными UUID не найдены",
                        Details = string.Join(", ", notFoundProfessors)
                    });
                }
            }

            Disciplines newDiscipline = new()
            {
                Uuid = Guid.NewGuid(),
                Name = createDto.Name.Trim(),
                ShortName = string.IsNullOrWhiteSpace(createDto.ShortName)
                    ? string.Concat(createDto.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(w => w[0]))
                    : createDto.ShortName.Trim(),
                IsArchived = createDto.IsArchived,
                DisciplineRegisterId = disciplineRegister?.DisciplineRegisterId,
                SemesterId = semester.SemesterId,
                AcademicYearId = academicYear.AcademicYearId,
                Groups = groups,
                Professors = professors
            };

            _context.Disciplines.Add(newDiscipline);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetDiscipline),
                new { uuid = newDiscipline.Uuid },
                new DisciplinesResponseDto(newDiscipline)
            );
        }

        [HttpDelete("{uuid}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Дисциплина удалена")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Дисциплина не найдена", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Удалить дисциплину по идентификатору"
        )]
        public async Task<IActionResult> DeleteDiscipline(
            [SwaggerParameter("UUID дисциплины")]
            Guid uuid
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.1",
                    Title = "Неверный запрос",
                    Message = "UUID дисциплины не может быть пустым"
                });
            }

            Disciplines? discipline = await _context.Disciplines.FirstOrDefaultAsync(d => d.Uuid == uuid);
            if (discipline == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.1",
                    Title = "Дисциплина не найдена",
                    Message = $"Дисциплина с UUID \"{uuid}\" не найдена"
                });
            }

            _context.Disciplines.Remove(discipline);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Дисциплина обновлена", typeof(DisciplinesResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Дисциплина не найдена", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Обновить дисциплину",
            Description = "Обновляет данные дисциплины по её UUID. Все поля необязательны"
        )]
        public async Task<ActionResult<DisciplinesResponseDto>> UpdateDiscipline(
            [SwaggerParameter("UUID дисциплины")]
            Guid uuid,
            [FromBody, SwaggerParameter("Данные для обновления")]
            DisciplinesUpdateDto updateDto
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.1",
                    Title = "Неверный запрос",
                    Message = "UUID дисциплины не может быть пустым"
                });
            }

            Disciplines? discipline = await _context.Disciplines
                .Include(d => d.Groups)
                .Include(d => d.Professors)
                .Include(d => d.DisciplineRegister)
                .Include(d => d.Semester)
                .Include(d => d.AcademicYear)
                .FirstOrDefaultAsync(d => d.Uuid == uuid);

            if (discipline == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.1",
                    Title = "Дисциплина не найдена",
                    Message = $"Дисциплина с UUID \"{uuid}\" не найдена"
                });
            }

            if (updateDto.Name != null)
            {
                if (updateDto.Name.Trim() == string.Empty)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2",
                        Title = "Неверный запрос",
                        Message = "Название дисциплины не может быть пустым"
                    });
                }
                discipline.Name = updateDto.Name.Trim();
            }

            if (updateDto.ShortName != null)
            {
                discipline.ShortName = updateDto.ShortName.Trim() != string.Empty
                    ? updateDto.ShortName.Trim()
                    : string.Concat(discipline.Name
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(w => w[0]));
            }

            if (updateDto.IsArchived.HasValue)
            {
                discipline.IsArchived = updateDto.IsArchived.Value;
            }

            if (updateDto.SemesterUuid.HasValue)
            {
                if (updateDto.SemesterUuid.Value == Guid.Empty)
                {
                    return BadRequest(new ApiError { StatusCode = "0.3", Title = "Неверный запрос", Message = "SemesterUuid не может быть пустым" });
                }
                Semesters? semester = await _context.Semesters.FirstOrDefaultAsync(s => s.Uuid == updateDto.SemesterUuid.Value);
                if (semester == null)
                {
                    return BadRequest(new ApiError { StatusCode = "1.2", Title = "Некорректные данные", Message = "Семестр с указанным UUID не найден" });
                }
                discipline.SemesterId = semester.SemesterId;
            }

            if (updateDto.AcademicYearUuid.HasValue)
            {
                if (updateDto.AcademicYearUuid.Value == Guid.Empty)
                {
                    return BadRequest(new ApiError { StatusCode = "0.4", Title = "Неверный запрос", Message = "AcademicYearUuid не может быть пустым" });
                }
                AcademicYears? academicYear = await _context.AcademicYears.FirstOrDefaultAsync(a => a.Uuid == updateDto.AcademicYearUuid.Value);
                if (academicYear == null)
                {
                    return BadRequest(new ApiError { StatusCode = "1.3", Title = "Некорректные данные", Message = "Учебный год с указанным UUID не найден" });
                }
                discipline.AcademicYearId = academicYear.AcademicYearId;
            }

            if (updateDto.DisciplineRegisterUuid != null)
            {
                if (updateDto.DisciplineRegisterUuid.Value == Guid.Empty)
                {
                    discipline.DisciplineRegisterId = null;
                }
                else
                {
                    DisciplinesRegisters? register = await _context.DisciplinesRegisters.FirstOrDefaultAsync(r => r.Uuid == updateDto.DisciplineRegisterUuid.Value);
                    if (register == null)
                    {
                        return BadRequest(new ApiError { StatusCode = "1.4", Title = "Некорректные данные", Message = "Реестр дисциплин с указанным UUID не найден" });
                    }
                    discipline.DisciplineRegisterId = register.DisciplineRegisterId;
                }
            }

            if (updateDto.GroupsUuids != null)
            {
                if (updateDto.GroupsUuids.Length == 0)
                {
                    return BadRequest(new ApiError { StatusCode = "0.5", Title = "Неверный запрос", Message = "Необходимо указать хотя бы одну группу" });
                }
                List<Groups> groups = await _context.Groups
                    .Where(g => updateDto.GroupsUuids.Contains(g.Uuid))
                    .ToListAsync();
                if (groups.Count != updateDto.GroupsUuids.Length)
                {
                    Guid[] notFoundGroups = updateDto.GroupsUuids.Except(groups.Select(g => g.Uuid)).ToArray();
                    return BadRequest(new ApiError { StatusCode = "1.5", Title = "Некорректные данные", Message = "Одна или несколько групп с указанными UUID не найдены", Details = string.Join(", ", notFoundGroups) });
                }
                discipline.Groups = groups;
            }

            if (updateDto.ProfessorsUuids != null)
            {
                List<Professors> professors = await _context.Professors
                    .Where(p => updateDto.ProfessorsUuids.Contains(p.Uuid))
                    .ToListAsync();
                if (professors.Count != updateDto.ProfessorsUuids.Length)
                {
                    Guid[] notFoundProfessors = updateDto.ProfessorsUuids.Except(professors.Select(p => p.Uuid)).ToArray();
                    return BadRequest(new ApiError
                    {
                        StatusCode = "1.7",
                        Title = "Некорректные данные",
                        Message = "Один или несколько преподавателей с указанными UUID не найдены",
                        Details = string.Join(", ", notFoundProfessors)
                    });
                }
                discipline.Professors = professors;
            }

            await _context.SaveChangesAsync();

            return Ok(new DisciplinesResponseDto(discipline));
        }
    }
}

