using MainService.Enums;
using MainService.Errors;

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
        [SwaggerResponse(200, "Кафедры найдены", typeof(IEnumerable<DepartmentsResponseDto>))]
        [SwaggerResponseExample(200, typeof(PagedResult<DepartmentsResponseDto>))]
        [SwaggerResponse(404, "Кафедры не найдены", typeof(ApiError))]
        [SwaggerResponseExample(404, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить список всех кафедр",
            Description = "Возвращает список всех кафедр в системе"
        )]
        public async Task<ActionResult<PagedResult<DepartmentsResponseDto>>> GetDepartments(
            [FromQuery, SwaggerParameter("Количество записей")]
            int size = 100,
            [FromQuery, SwaggerParameter("Сдвиг от начала")]
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
                })
                .AsNoTracking();

            Task<int> totalTask = _context.Departments.CountAsync();
            Task<List<DepartmentsResponseDto>> departmentsTask = query.ToListAsync();

            await Task.WhenAll(totalTask, departmentsTask);

            List<DepartmentsResponseDto> departments = await departmentsTask;
            int totalCount = await totalTask;

            if (totalCount == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.1",
                    Title = "Кафедры не найдены",
                    Message = "В системе не найдено ни одной кафедры"
                });
            }

            if (departments.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "0.1",
                    Title = "Кафедры не найдены",
                    Message = "В системе не найдено ни одной кафедры для указанных параметров запроса"
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
        [SwaggerResponse(200, "Кафедра найдена", typeof(DepartmentsResponseDto))]
        [SwaggerResponseExample(200, typeof(DepartmentsResponseDto))]
        [SwaggerResponse(400, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(400, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(404, "Кафедра не найдена", typeof(ApiError))]
        [SwaggerResponseExample(404, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить кафедру по идентификатору",
            Description = "Возвращает одну кафедру по её uuid"
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
                    StatusCode = "0.1",
                    Title = "Неверный запрос",
                    Message = "UUID кафедры не может быть пустым"
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
                    StatusCode = "1.1",
                    Title = "Кафедра не найдена",
                    Message = $"Кафедра с UUID \"{uuid}\" не найдена"
                });
            }

            return Ok(department);
        }



        [HttpGet("faculty/{facultyUuid}")]
        [SwaggerResponse(200, "Кафедры найдены", typeof(PagedResult<DepartmentsResponseDto>))]
        [SwaggerResponseExample(200, typeof(PagedResult<DepartmentsResponseDto>))]
        [SwaggerResponse(400, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(400, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(404, "Кафедры не найдены", typeof(ApiError))]
        [SwaggerResponseExample(404, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить кафедры по факультету",
            Description = "Возвращает список кафедр для указанного факультета"
        )]
        public async Task<ActionResult<PagedResult<DepartmentsResponseDto>>> GetDepartmentsByFaculty(
            [SwaggerParameter("UUID факультета")]
            Guid facultyUuid,
            [FromQuery, SwaggerParameter("Количество записей")]
            int size = 100,
            [FromQuery, SwaggerParameter("Сдвиг от начала")]
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
                    StatusCode = "0.1",
                    Title = "Неверный запрос",
                    Message = "UUID факультета не может быть пустым"
                });
            }

            Faculties? faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.Uuid == facultyUuid);
            if (faculty == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.1",
                    Title = "Факультет не найден",
                    Message = $"Факультет с UUID \"{facultyUuid}\" не найден"
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
                })
                .AsNoTracking();

            Task<int> totalTask = _context.Departments.CountAsync(d => d.FacultyId == faculty.FacultyId);
            Task<List<DepartmentsResponseDto>> departmentsTask = query.ToListAsync();

            List<DepartmentsResponseDto> departments = await departmentsTask;
            int totalCount = await totalTask;

            if (totalCount == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.1",
                    Title = "Кафедры не найдены",
                    Message = $"В системе не найдено ни одной кафедры для факультета с UUID \"{facultyUuid}\""
                });
            }

            if (departments.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2",
                    Title = "Кафедры не найдены",
                    Message = $"В системе не найдено ни одной кафедры для указанных параметров запроса"
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
        [SwaggerResponse(201, "Кафедра успешно создана", typeof(DepartmentsResponseDto))]
        [SwaggerResponseExample(201, typeof(DepartmentsResponseDto))]
        [SwaggerResponse(400, "Некорректные данные для создания кафедры", typeof(ApiError))]
        [SwaggerResponseExample(400, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Создать новую кафедру",
            Description = "Создает новую кафедру в системе"
        )]
        public async Task<ActionResult<DepartmentsResponseDto>> CreateDepartment(
            [FromBody, SwaggerParameter("Данные новой кафедры")]
            DepartmentsCreateDto departmentDto
        )
        {

            if (string.IsNullOrWhiteSpace(departmentDto.Name))
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.1",
                    Title = "Не все поля заполнены",
                    Message = "Название кафедры не может быть пустым"
                });
            }

            Faculties? faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.Uuid == departmentDto.FacultyUuid);
            if (faculty == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "1.1",
                    Title = "Неверные данные",
                    Message = $"Факультет с UUID \"{departmentDto.FacultyUuid}\" не найден"
                });
            }

            if (departmentDto.Code != null
                && _context.Departments.Any(d => d.Code == departmentDto.Code))
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "1.2",
                    Title = "Кафедра с таким кодом уже существует",
                    Message = $"Кафедра с кодом \"{departmentDto.Code}\" уже существует"
                });
            }

            // Если код не отравили, то будет равен последнему айди в базе + 1
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
        [SwaggerResponse(204, "Кафедра успешно удалена")]
        [SwaggerResponse(400, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(400, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(404, "Кафедра не найдена", typeof(ApiError))]
        [SwaggerResponseExample(404, typeof(ApiError404NotFoundExample))]
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
                    StatusCode = "0.1",
                    Title = "Неверный запрос",
                    Message = "UUID кафедры не может быть пустым"
                });
            }

            Departments? department = await _context.Departments.FirstOrDefaultAsync(d => d.Uuid == uuid);
            if (department == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.1",
                    Title = "Кафедра не найдена",
                    Message = $"Кафедра с UUID \"{uuid}\" не найдена"
                });
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{uuid}")]
        [SwaggerResponse(200, "Кафедра успешно обновлена", typeof(DepartmentsResponseDto))]
        [SwaggerResponse(400, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(400, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(404, "Кафедра не найдена", typeof(ApiError))]
        [SwaggerResponseExample(404, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Обновить кафедру",
            Description = "Обновляет данные кафедры по её UUID. Все поля являются необязательными, но хотя бы одно должно быть указано"
        )]
        public async Task<ActionResult<DepartmentsResponseDto>> UpdateDepartment(
            [SwaggerParameter("UUID кафедры")]
            Guid uuid,
            [FromBody, SwaggerParameter("Данные для обновления кафедры")]
            DepartmentsUpdateDto updateDto
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.1",
                    Title = "Неверный запрос",
                    Message = "UUID кафедры не может быть пустым"
                });
            }

            Departments? department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Uuid == uuid);
            if (department == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.1",
                    Title = "Кафедра не найдена",
                    Message = $"Кафедра с UUID \"{uuid}\" не найдена"
                });
            }

            if (updateDto.Name != null)
            {
                department.Name = updateDto.Name;
            }

            if (updateDto.ShortName != null)
            {
                department.ShortName = updateDto.ShortName;
            }

            if (updateDto.Code != null)
            {
                if (_context.Departments.Any(d => d.Code == updateDto.Code && d.DepartmentId != department.DepartmentId))
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "1.2",
                        Title = "Кафедра с таким кодом уже существует",
                        Message = $"Кафедра с кодом \"{updateDto.Code}\" уже существует"
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
                        StatusCode = "1.3",
                        Title = "Неверные данные",
                        Message = $"Факультет с UUID \"{updateDto.FacultyUuid}\" не найден"
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