using MainService.Errors;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Annotations;


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
        public async Task<ActionResult<IEnumerable<Disciplines>>> GetDisciplines()
        {
            return await _context.Disciplines.ToListAsync();
        }

        [HttpGet("{id}")]
        [SwaggerResponse(200, "Дисциплина найдена", typeof(Disciplines))]
        [SwaggerResponse(404, "Дисциплина не найдена", typeof(ApiError))]
        [SwaggerOperation(
            Summary = "Получить дисциплину по идентификатору",
            Description = "Возвращает одну дисциплину по её ID"
        )]
        public async Task<ActionResult<Disciplines>> GetDiscipline([SwaggerParameter("ID дисциплины")] int id)
        {
            var discipline = await _context.Disciplines.FindAsync(id);

            if (discipline == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "0.0",
                    Title = "Дисциплина не найдена",
                    Message = $"Дисциплина с ID \"{id}\" не найдена"
                });
            }

            return Ok(discipline);
        }

        [HttpPost]
        [SwaggerResponse(201, "Дисциплина успешно создана", typeof(Disciplines))]
        [SwaggerResponse(400, "Некорректные данные для создания дисциплины", typeof(ApiError))]
        [SwaggerOperation(
            Summary = "Создать новую дисциплину"
        )]
        public async Task<ActionResult<Disciplines>> CreateDiscipline([FromBody, SwaggerParameter("Данные новой дисциплины")] Disciplines discipline)
        {
            if (discipline == null || string.IsNullOrEmpty(discipline.Name))
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.0",
                    Title = "Некорректные данные",
                    Message = "Дисциплина должна иметь имя"
                });
            }

            _context.Disciplines.Add(discipline);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDiscipline), new { id = discipline.DisciplineId }, discipline);
        }
    }

}