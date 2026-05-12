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
        [SwaggerResponse(200, "Записи из реестров дисциплин найдены", typeof(IEnumerable<DisciplinesRegistersDto>))]
        [SwaggerResponse(404, "Записи дисциплин не найдены", typeof(ApiError))]
        [SwaggerResponseExample(404, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить список записей реестра дисциплин",
            Description = "Возвращает все записи реестра дисциплин"
        )]
        public async Task<ActionResult<IEnumerable<DisciplinesRegistersDto>>> GetDisciplinesRegisters()
        {
            List<DisciplinesRegisters> registersList = await _context.DisciplinesRegisters.ToListAsync();
            if (registersList.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "0.0",
                    Title = "Записи дисциплин не найдены",
                    Message = "В системе не найдено ни одной записи реестра дисциплин"
                });
            }

            var registersDtoList = registersList.Select(r => new DisciplinesRegistersDto(r)).ToList();
            return Ok(registersDtoList);
        }

        [HttpGet("{uuid}")]
        [SwaggerResponse(200, "Запись дисциплины найдена", typeof(DisciplinesRegisters))]
        [SwaggerResponse(404, "Запись дисциплины не найдена", typeof(ApiError))]
        [SwaggerResponseExample(404, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить запись дисциплины по идентификатору",
            Description = "Возвращает одну запись дисциплины по её uuid"
        )]
        public async Task<ActionResult<DisciplinesRegistersDto>> GetDisciplineRegister(
            [SwaggerParameter("UUID записи дисциплины")]
            Guid uuid
            )
        {
            DisciplinesRegisters? disciplineRegister = await _context.DisciplinesRegisters.FindAsync(uuid);

            if (disciplineRegister == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "0.0",
                    Title = "Запись дисциплины не найдена",
                    Message = $"Запись дисциплины с UUID {uuid} не найдена"
                });
            }

            return Ok(new DisciplinesRegistersDto(disciplineRegister));
        }

        [HttpPost]
        [SwaggerResponse(201, "Запись дисциплины создана", typeof(DisciplinesRegisters))]
        [SwaggerResponse(400, "Некорректные данные", typeof(ApiError))]
        [SwaggerResponseExample(400, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Создать новую запись в реестре дисциплин",
            Description = "Создает новую запись в реестре дисциплин с предоставленными данными"
        )]
        public async Task<ActionResult<DisciplinesRegistersDto>> CreateDisciplineRegister(
            [SwaggerParameter("Данные новой записи в реестре дисциплин")]
            DisciplinesRegistersDto disciplineRegisterDto
            )
        {
            if (disciplineRegisterDto == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.0",
                    Title = "Неверный запрос",
                    Message = "Тело запроса не может быть пустым"
                });
            }

            DisciplinesRegisters disciplineRegister = new()
            {
                Uuid = Guid.NewGuid(),
                Name = disciplineRegisterDto.Name.Trim(),
                ShortName = string.IsNullOrWhiteSpace(disciplineRegisterDto.ShortName) ? "" : disciplineRegisterDto.ShortName.Trim()
            };
            _context.DisciplinesRegisters.Add(disciplineRegister);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetDisciplineRegister),
                new { uuid = disciplineRegister.Uuid },
                new DisciplinesRegistersDto(disciplineRegister)
                );
        }

        [HttpDelete("{uuid}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Запись дисциплины удалена")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Запись дисциплины не найдена", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Удалить запись дисциплины по идентификатору"
        )]
        public async Task<IActionResult> DeleteDisciplineRegister(
            [SwaggerParameter("UUID записи дисциплины")] Guid uuid
            )
        {
            var disciplineRegister = await _context.DisciplinesRegisters.FindAsync(uuid);
            if (disciplineRegister == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "0",
                    Title = "Запись дисциплины не найдена",
                    Message = $"Запись дисциплины с UUID {uuid} не найдена"
                });
            }

            _context.DisciplinesRegisters.Remove(disciplineRegister);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}