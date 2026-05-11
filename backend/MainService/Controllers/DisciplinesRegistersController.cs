using MainService.Errors;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace MainService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DisciplinesRegistersController : ControllerBase
    {
        private readonly MainServiceContext _context;

        public DisciplinesRegistersController(MainServiceContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerResponse(200, "Записи из реестров дисциплин найдены", typeof(IEnumerable<DisciplinesRegisters>))]
        [SwaggerResponse(404, "Записи дисциплин не найдены")]
        public async Task<ActionResult<IEnumerable<DisciplinesRegisters>>> GetDisciplinesRegisters()
        {
            return await _context.DisciplinesRegisters.ToListAsync();
        }

        [HttpGet("{id}")]
        [SwaggerResponse(200, "Запись дисциплины найдена", typeof(DisciplinesRegisters))]
        [SwaggerResponse(404, "Запись дисциплины не найдена")]
        [SwaggerOperation(
            Summary = "Получить запись дисциплины по идентификатору",
            Description = "Возвращает одну запись дисциплины по её ID"
        )]
        public async Task<ActionResult<DisciplinesRegisters>> GetDisciplineRegister([SwaggerParameter("ID записи дисциплины")] int id)
        {
            var disciplineRegister = await _context.DisciplinesRegisters.FindAsync(id);

            if (disciplineRegister == null)
            {
                return NotFound();
            }

            return Ok(disciplineRegister);
        }

        [HttpPost]
        [SwaggerResponse(201, "Запись дисциплины создана", typeof(DisciplinesRegisters))]
        [SwaggerResponse(400, "Некорректные данные")]
        [SwaggerOperation(
            Summary = "Создать новую запись в реестре дисциплин",
            Description = "Создает новую запись в реестре дисциплин с предоставленными данными"
        )]
        public async Task<ActionResult<DisciplinesRegisters>> CreateDisciplineRegister(
            [SwaggerParameter("Данные новой записи в реестре дисциплин")] DisciplinesRegisters disciplineRegister
            )
        {
            _context.DisciplinesRegisters.Add(disciplineRegister);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDisciplineRegister), new { id = disciplineRegister.DisciplineRegisterId }, disciplineRegister);
        }

        [HttpDelete("{id}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Запись дисциплины удалена")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Запись дисциплины не найдена", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Удалить запись дисциплины по идентификатору"
        )]
        public async Task<IActionResult> DeleteDisciplineRegister(
            [SwaggerParameter("ID записи дисциплины")] int id
            )
        {
            var disciplineRegister = await _context.DisciplinesRegisters.FindAsync(id);
            if (disciplineRegister == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "0",
                    Title = "Запись дисциплины не найдена",
                    Message = $"Запись дисциплины с ID {id} не найдена"
                });
            }

            _context.DisciplinesRegisters.Remove(disciplineRegister);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}