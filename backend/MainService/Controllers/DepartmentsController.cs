using MainService.Enums;
using MainService.Errors;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace MainService
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly MainServiceContext _context;
        public DepartmentsController(MainServiceContext context)
        {
            _context = context;
        }


        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Кафедры найдены", typeof(IEnumerable<DepartmentsResponseDto>))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(PagedResult<DepartmentsResponseDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Кафедры не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить список всех кафедр",
            Description = "Возвращает список всех кафедр в системе"
        )]
        public async Task<ActionResult<PagedResult<DepartmentsResponseDto>>> GetDepartments(
            [FromQuery, SwaggerParameter("Количество записей")]
            int size = 100,
            [FromQuery, SwaggerParameter("Смещение от начала")]
            int offset = 0,
            [FromQuery, SwaggerParameter("Название")]
            string? name = null,
            [FromQuery, SwaggerParameter("Порядок сортировки по названию")]
            SortOrder sortOrder = SortOrder.Ascending
        )
        {

            IQueryable<DepartmentsResponseDto> query = _context.Departments
                .Where(d => string.IsNullOrEmpty(name) || d.Name.Contains(name))
                .SortByKey(d => d.Name, sortOrder)
                .TakeWithOffset(offset, size)
                .Select(d => new DepartmentsResponseDto
                {
                    Uuid = d.Uuid,
                    Name = d.Name,
                    ShortName = d.ShortName,
                    Code = d.Code,
                    FacultyUuid = d.Faculty!.Uuid,
                    Version = d.Version
                });

            Task<int> totalTask = _context.Departments.CountAsync();
            Task<List<DepartmentsResponseDto>> departmentsTask = query.ToListAsync();

            List<DepartmentsResponseDto> departments = await departmentsTask;
            int totalCount = await totalTask;

            if (totalCount == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Кафедры не найдены",
                    Message = "В системе не найдено ни одной кафедры",
                    Field = string.Empty
                });
            }

            if (departments.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Кафедры не найдены",
                    Message = "В системе не найдено ни одной кафедры для указанных параметров запроса",
                    Field = string.Empty
                });
            }

            return Ok(new PagedResult<DepartmentsResponseDto>(
                Total: totalCount,
                Offset: offset,
                Size: size,
                Items: departments
            ));
        }


        [HttpGet("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Кафедра найдена", typeof(DepartmentsResponseDto))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(DepartmentsResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Кафедра не найдена", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить кафедру по идентификатору",
            Description = "Возвращает одну кафедру по её UUID"
        )]
        public async Task<ActionResult<DepartmentsResponseDto>> GetDepartment(
            [SwaggerParameter("UUID кафедры")]
            Guid uuid
        )
        {

            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID кафедры не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            DepartmentsResponseDto? department = await _context.Departments
                .Where(d => d.Uuid == uuid)
                .Select(d => new DepartmentsResponseDto
                {
                    Uuid = d.Uuid,
                    Name = d.Name,
                    ShortName = d.ShortName,
                    Code = d.Code,
                    FacultyUuid = d.Faculty!.Uuid,
                    Version = d.Version
                })
                .FirstOrDefaultAsync();

            if (department == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Кафедра не найдена",
                    Message = $"Кафедра с UUID \"{uuid}\" не найдена",
                    Field = nameof(uuid)
                });
            }

            return Ok(department);
        }



        [HttpGet("faculty/{facultyUuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Кафедры найдены", typeof(PagedResult<DepartmentsResponseDto>))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(PagedResult<DepartmentsResponseDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Кафедры не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить кафедры по факультету",
            Description = "Возвращает список кафедр, принадлежащих указанному факультету"
        )]
        public async Task<ActionResult<PagedResult<DepartmentsResponseDto>>> GetDepartmentsByFaculty(
            [SwaggerParameter("UUID факультета")]
            Guid facultyUuid,
            [FromQuery, SwaggerParameter("Количество записей")]
            int size = 100,
            [FromQuery, SwaggerParameter("Смещение от начала списка")]
            int offset = 0,
            [FromQuery, SwaggerParameter("Название")]
            string? name = null,
            [FromQuery, SwaggerParameter("Порядок сортировки по названию")]
            SortOrder sortOrder = SortOrder.Ascending
        )
        {
            if (facultyUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID факультета не может быть пустым",
                    Field = nameof(facultyUuid)
                });
            }

            Faculties? faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.Uuid == facultyUuid);
            if (faculty == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Факультет не найден",
                    Message = $"Факультет с UUID \"{facultyUuid}\" не найден",
                    Field = nameof(facultyUuid)
                });
            }

            IQueryable<DepartmentsResponseDto> query = _context.Departments
                .Where(d => d.FacultyId == faculty.FacultyId
                    && (string.IsNullOrEmpty(name) || d.Name.Contains(name)))
                .SortByKey(d => d.Name, sortOrder)
                .TakeWithOffset(offset, size)
                .Select(d => new DepartmentsResponseDto
                {
                    Uuid = d.Uuid,
                    Name = d.Name,
                    ShortName = d.ShortName,
                    Code = d.Code,
                    FacultyUuid = d.Faculty!.Uuid,
                    Version = d.Version
                });

            Task<int> totalTask = _context.Departments.CountAsync(d => d.FacultyId == faculty.FacultyId);
            Task<List<DepartmentsResponseDto>> departmentsTask = query.ToListAsync();

            List<DepartmentsResponseDto> departments = await departmentsTask;
            int totalCount = await totalTask;

            if (totalCount == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Кафедры не найдены",
                    Message = $"В системе не найдено ни одной кафедры для факультета с UUID \"{facultyUuid}\"",
                    Field = nameof(facultyUuid)
                });
            }

            if (departments.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Кафедры не найдены",
                    Message = $"В системе не найдено ни одной кафедры для указанного факультета и параметров запроса",
                    Field = nameof(facultyUuid)
                });
            }

            return Ok(new PagedResult<DepartmentsResponseDto>(
                Total: totalCount,
                Offset: offset,
                Size: size,
                Items: departments
            ));
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, "Кафедра создана", typeof(DepartmentsResponseDto))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(DepartmentsResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Создать новую кафедру",
            Description = "Создает новую кафедру в системе"
        )]
        public async Task<ActionResult<DepartmentsResponseDto>> CreateDepartment(
            [FromBody, SwaggerParameter("Данные для создания кафедры")]
            DepartmentsCreateDto departmentDto
        )
        {
            // проверка перед запросом к бд
            if (string.IsNullOrWhiteSpace(departmentDto.Name))
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "Название записи не может быть пустым",
                    Field = nameof(departmentDto.Name)
                });
            }

            Faculties? faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.Uuid == departmentDto.FacultyUuid);
            if (faculty == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "1.2.3",
                    Title = "Неверный запрос",
                    Message = $"Факультет с UUID \"{departmentDto.FacultyUuid}\" не найден",
                    Field = nameof(departmentDto.FacultyUuid)
                });
            }

            if (departmentDto.Code != null
                && _context.Departments.Any(d => d.Code == departmentDto.Code))
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.1",
                    Title = "Код уже используется",
                    Message = $"Кафедра с кодом \"{departmentDto.Code}\" уже существует",
                    Field = nameof(departmentDto.Code)
                });
            }

            // если код не указан, вычисляем следующий код как max(DepartmentId) + 1
            string departmentCode = departmentDto.Code != null
                ? departmentDto.Code
                : _context.Departments.Any()
                    ? (_context.Departments.Max(d => d.DepartmentId) + 1).ToString()
                    : "1";

            Departments newDepartment = new()
            {
                Uuid = Guid.NewGuid(),
                Name = departmentDto.Name,
                ShortName = departmentDto.ShortName,
                Code = departmentCode,
                FacultyId = faculty.FacultyId
            };

            _context.Departments.Add(newDepartment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDepartment), new { uuid = newDepartment.Uuid }, new DepartmentsResponseDto
            {
                Uuid = newDepartment.Uuid,
                Name = newDepartment.Name,
                ShortName = newDepartment.ShortName,
                Code = newDepartment.Code,
                FacultyUuid = faculty.Uuid,
                Version = newDepartment.Version
            });
        }

        [HttpDelete("{uuid}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Кафедра удалена")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Кафедра не найдена", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Удалить кафедру",
            Description = "Удаляет кафедру по её UUID"
        )]
        public async Task<IActionResult> DeleteDepartment(
            [SwaggerParameter("UUID кафедры")]
            Guid uuid
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID кафедры не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            Departments? department = await _context.Departments.FirstOrDefaultAsync(d => d.Uuid == uuid);
            if (department == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Кафедра не найдена",
                    Message = $"Кафедра с UUID \"{uuid}\" не найдена",
                    Field = nameof(uuid)
                });
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Кафедра обновлена", typeof(DepartmentsResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Кафедра не найдена", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Обновить кафедру",
            Description = "Обновляет кафедру по её UUID. Все поля необязательны. " +
                          "Если передать shortName как пустую строку — аббревиатура будет сгенерирована автоматически из названия. " +
                          "Если не передавать shortName вовсе — текущее значение останется без изменений."
        )]
        public async Task<ActionResult<DepartmentsResponseDto>> UpdateDepartment(
            [SwaggerParameter("UUID кафедры")]
            Guid uuid,
            [FromBody, SwaggerParameter("Данные для обновления кафедры")]
            DepartmentsUpdateDto updateDto
        )
        {
            // Предварительная валидация без обращения к БД
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID записи не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            if (updateDto.Name != null && updateDto.Name.Trim() == string.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "Название записи не может быть пустым",
                    Field = nameof(updateDto.Name)
                });
            }

            if (updateDto.ShortName != null)
            {
                // нет обращений к БД, простая логика обработки строки
            }

            // Загрузка сущности из БД и дальнейшие проверки
            Departments? department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Uuid == uuid);
            if (department == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Запись не найдена",
                    Message = $"Запись с UUID \"{uuid}\" не найдена",
                    Field = nameof(uuid)
                });
            }

            if (updateDto.Name != null)
            {
                department.Name = updateDto.Name;
            }

            if (updateDto.ShortName != null)
            {
                if (updateDto.ShortName.Trim() != string.Empty)
                {
                    department.ShortName = updateDto.ShortName.Trim();
                }
                else
                {
                    department.ShortName = string.Concat(department.Name
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(w => w[0]));
                }
            }

            if (updateDto.Code != null)
            {
                if (_context.Departments.Any(d => d.Code == updateDto.Code && d.DepartmentId != department.DepartmentId))
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.1",
                        Title = "Код уже используется",
                        Message = $"Кафедра с кодом \"{updateDto.Code}\" уже существует",
                        Field = nameof(updateDto.Code)
                    });
                }
                department.Code = updateDto.Code;
            }

            if (updateDto.FacultyUuid != null)
            {
                Faculties? faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.Uuid == updateDto.FacultyUuid);
                if (faculty == null)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Факультет не найден",
                        Message = $"Факультет с UUID \"{updateDto.FacultyUuid}\" не найден",
                        Field = nameof(updateDto.FacultyUuid)
                    });
                }
                department.FacultyId = faculty.FacultyId;
            }

            await _context.SaveChangesAsync();

            return Ok(await _context.Departments
                .Where(d => d.Uuid == uuid)
                .Select(d => new DepartmentsResponseDto
                {
                    Uuid = d.Uuid,
                    Name = d.Name,
                    ShortName = d.ShortName,
                    Code = d.Code,
                    FacultyUuid = d.Faculty!.Uuid,
                    Version = d.Version
                })
                .FirstAsync());
        }

    }


}