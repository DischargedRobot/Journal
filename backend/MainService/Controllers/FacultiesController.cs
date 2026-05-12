using MainService.EntityDtoExamples;
using MainService.Errors;

using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace MainService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacultiesController : ControllerBase
    {

        private readonly MainServiceContext _context;
        public FacultiesController(MainServiceContext context)
        {
            _context = context;
        }


        [HttpGet]
        [SwaggerResponse(200, "Факультеты найдены", typeof(IEnumerable<FacultiesDto>))]
        [SwaggerResponse(404, "Факультеты не найдены", typeof(ApiError))]
        [SwaggerResponseExample(404, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить список всех факультетов",
            Description = "Возвращает список всех факультетов в системе"
        )]
        public async Task<ActionResult<IEnumerable<FacultiesDto>>> GetFaculties()
        {
            List<Faculties> faculties = await _context.Faculties.ToListAsync();
            if (faculties.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "0.0",
                    Title = "Факультеты не найдены",
                    Message = "В системе не найдено ни одного факультета"
                });
            }

            List<FacultiesDto> facultiesDtos = faculties.Select(f => new FacultiesDto(f)).ToList();
            return Ok(facultiesDtos);
        }


        [HttpGet("{uuid}")]
        [SwaggerResponse(200, "Факультет найден", typeof(FacultiesDto))]
        [SwaggerResponseExample(200, typeof(FacultiesDtoExample))]

        [SwaggerResponse(404, "Факультет не найден", typeof(ApiError))]
        [SwaggerResponseExample(404, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить факультет по идентификатору",
            Description = "Возвращает один факультет по его UUID"
        )]
        public async Task<ActionResult<FacultiesDto>> GetFaculty(
            [SwaggerParameter("UUID факультета")] Guid uuid)
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.0",
                    Title = "Неверный запрос",
                    Message = "UUID факультета не может быть пустым"
                });
            }
            Faculties? faculty = await _context.Faculties.FindAsync(uuid);
            if (faculty == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "0.0",
                    Title = "Факультет не найден",
                    Message = "В системе не найден факультет с указанным UUID"
                });
            }
            return Ok(new FacultiesDto(faculty));
        }


        [HttpPost]
        [SwaggerResponse(201, "Факультет создан", typeof(FacultiesDto))]
        [SwaggerResponse(400, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(400, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Создать новый факультет",
            Description = "Создает новый факультет в системе"
        )]
        public async Task<ActionResult<FacultiesDto>> CreateFaculty(
            [FromBody, SwaggerRequestBody("Данные для создания факультета", Required = true)]
            FacultiesCreateDto FacultiesDto
        )
        {
            if (FacultiesDto == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.0",
                    Title = "Неверный запрос",
                    Message = "Тело запроса не может быть пустым"
                });
            }

            if (FacultiesDto.Name == null || FacultiesDto.Name.Trim() == string.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.1",
                    Title = "Неверный запрос",
                    Message = "Название факультета не может быть пустым"
                });
            }

            Faculties faculty = new()
            {
                Name = FacultiesDto.Name.Trim(),
                ShortName = string.IsNullOrWhiteSpace(FacultiesDto.ShortName)
                    ? string.Concat(FacultiesDto.Name
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(w => w[0]))
                    : FacultiesDto.ShortName.Trim()
            };

            _context.Faculties.Add(faculty);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFaculty), new { uuid = faculty.Uuid }, new FacultiesDto(faculty));
        }


        [HttpDelete("{uuid}")]
        [SwaggerResponse(204, "Факультет удален")]
        [SwaggerResponse(400, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(400, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(404, "Факультет не найден", typeof(ApiError404NotFoundExample))]
        [SwaggerResponseExample(404, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Удалить факультет",
            Description = "Удаляет факультет по его UUID"
        )]
        public async Task<IActionResult> DeleteFaculty(
            [SwaggerParameter("UUID факультета")] Guid uuid
            )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.0",
                    Title = "Неверный запрос",
                    Message = "UUID факультета не может быть пустым"
                });
            }

            Faculties? faculty = await _context.Faculties.FindAsync(uuid);
            if (faculty == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "0.0",
                    Title = "Факультет не найден",
                    Message = "В системе не найден факультет с указанным UUID"
                });
            }

            _context.Faculties.Remove(faculty);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}