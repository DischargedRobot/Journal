using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace MainService.Controllers
{
[ApiController]
[Route("api/v1/[controller]")]
public class TestGenerationController : ControllerBase
{
    private readonly MainServiceContext _context;

    public TestGenerationController(MainServiceContext context)
    {
        _context = context;
    }

    [HttpPost("groups")]
    [SwaggerResponse(StatusCodes.Status200OK, "Данные для теста сгенерированы успешно")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Внутренняя ошибка сервера")]
    public async Task<IActionResult> GenerateGroups()
    {
        try
        {
            Groups[] newGroup = Enumerable.Range(1, 3).Select(i => new Groups()
            {
                Uuid = Guid.NewGuid(),
                Code = $"{1000 + i}",
                AdmissionDate = DateOnly.FromDateTime(DateTime.UtcNow),
                TrainingDirectionId = 1,
                FacultyId = 1,
                Faculty = _context.Faculties.First(f => f.FacultyId == 1)
            }).ToArray();
            _context.Groups.AddRange(newGroup);
            await _context.SaveChangesAsync();

            return Ok(new {
                message = "Данные для теста сгенерированы успешно",
                newGroup = newGroup.Select(g => new GroupsResponseDto(g))
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("faculties")]
    [SwaggerResponse(StatusCodes.Status200OK, "Тестовые данные факультета сгенерированы успешно")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Внутренняя ошибка сервера")]
    public async Task<IActionResult> GenerateFaculties()
    {
        try
        {
            Faculties[] newFaculty = Enumerable.Range(1, 3).Select(i => new Faculties()
            {
                Uuid = Guid.NewGuid(),
                Name = $"Факультет {i}",
                ShortName = $"Ф {i}"
            }).ToArray();
            _context.Faculties.AddRange(newFaculty);
            await _context.SaveChangesAsync();

            return Ok(new {
                message = "Тестовые данные факультета сгенерированы успешно",
                newFaculty = newFaculty.Select(f => new FacultiesResponseDto(f))
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

// TODO: исправить ошибку при сортировке
    [HttpPost("departments")]
    [SwaggerResponse(StatusCodes.Status200OK, "Тестовые данные отделения сгенерированы успешно")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Внутренняя ошибка сервера")]
    public async Task<IActionResult> GenerateDepartments()
    {
        try
        {
            Faculties Faculty = _context.Faculties.First(f => f.FacultyId == 1);
            Departments[] newDepartment = Enumerable.Range(1, 3).Select(i => new Departments()
            {
                Uuid = Guid.NewGuid(),
                Name = $"Кафедра {i}",
                ShortName = $"К {i}",
                Code = $"{100 + i}",
                Faculty = Faculty,
                FacultyId = Faculty.FacultyId
            }).ToArray();
            _context.Departments.AddRange(newDepartment);
            await _context.SaveChangesAsync();

            return Ok(new {
                message = "Тестовые данные отделения сгенерированы успешно",
                newDepartment = newDepartment.Select(d => new DepartmentsResponseDto(d))
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
}
}