using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace MainService.Controllers{
[ApiController]
[Route("api/v1/[controller]")]
public class TestGenerationController : ControllerBase
{
    private readonly MainServiceContext _context;

    public TestGenerationController(MainServiceContext context)
    {
        _context = context;
    }

    [HttpPost]
    [SwaggerResponse(StatusCodes.Status200OK, "Данные для теста сгенерированы успешно")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Внутренняя ошибка сервера")]
    public async Task<IActionResult> GenerateGroup()
    {
        try
        {
            Groups newGroup = new()
            {
                Uuid = Guid.NewGuid(),
                Code = "124",
                AdmissionDate = DateOnly.FromDateTime(DateTime.UtcNow),
                TrainingDirectionId = 1,
                FacultyId = 1
            };
            _context.Groups.Add(newGroup);
            await _context.SaveChangesAsync();

            return Ok(new {
                message = "Данные для теста сгенерированы успешно"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
}