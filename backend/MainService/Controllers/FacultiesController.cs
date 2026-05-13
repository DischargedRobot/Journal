using MainService.EntityDtoExamples;
using MainService.Errors;

using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using MainService.Enums;

namespace MainService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class FacultiesController : ControllerBase
    {

        private readonly MainServiceContext _context;
        public FacultiesController(MainServiceContext context)
        {
            _context = context;
        }


        [HttpGet]
        [SwaggerResponse(200, "Факультеты найдены", typeof(IEnumerable<FacultiesResponseDto>))]
        [SwaggerResponse(404, "Факультеты не найдены", typeof(ApiError))]
        [SwaggerResponseExample(404, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить список всех факультетов",
            Description = "Возвращает список всех факультетов в системе"
        )]
        public async Task<ActionResult<PagedResult<FacultiesResponseDto>>> GetFaculties(
            [FromQuery, SwaggerParameter("Количество записей")]
            int size = 100,
            [FromQuery, SwaggerParameter("Смещение от начала списка")]
            int offset = 0,
            [FromQuery, SwaggerParameter("Название факультета")]
            string? name = null,
            [FromQuery, SwaggerParameter("Порядок сортировки по названию факультета")]
            SortOrder sortOrder = SortOrder.Ascending
        )
        {
            IQueryable<Faculties> query = _context.Faculties
                .AsQueryable()
                .SortByKey(f => string.IsNullOrWhiteSpace(name) || f.Name.Contains(name), sortOrder)
                .TakeWithOffset(offset, size)
                .AsNoTracking();

            Task<int> totalTask = _context.Faculties.CountAsync();
            Task<List<Faculties>> facultiesTask = query.ToListAsync();

            List<Faculties> faculties = await facultiesTask;
            int totalCount = await totalTask;

            if (totalCount == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.1",
                    Title = "Факультеты не найдены",
                    Message = "В системе не найдено ни одного факультета"
                });
            }

            if (faculties.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2",
                    Title = "Факультеты не найдены",
                    Message = "В системе не найдено ни одного факультета для указанных параметров запроса"
                });
            }

            return Ok(new PagedResult<FacultiesResponseDto>(
                Total: totalCount,
                Offset: offset,
                Size: size,
                Items: faculties.Select(f => new FacultiesResponseDto(f))
            ));
        }


        [HttpGet("{uuid}")]
        [SwaggerResponse(200, "Факультет найден", typeof(FacultiesResponseDto))]
        [SwaggerResponseExample(200, typeof(FacultiesDtoExample))]
        [SwaggerResponse(400, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(400, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(404, "Факультет не найден", typeof(ApiError))]
        [SwaggerResponseExample(404, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить факультет по идентификатору",
            Description = "Возвращает один факультет по его UUID"
        )]
        public async Task<ActionResult<FacultiesResponseDto>> GetFaculty(
            [SwaggerParameter("UUID факультета")]
            Guid uuid
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.1",
                    Title = "Неверный запрос",
                    Message = "UUID факультета не может быть пустым"
                });
            }

            Faculties? faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.Uuid == uuid);
            if (faculty == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.1",
                    Title = "Факультет не найден",
                    Message = "В системе не найден факультет с указанным UUID"
                });
            }

            return Ok(new FacultiesResponseDto(faculty));
        }


        [HttpPost]
        [SwaggerResponse(201, "Факультет создан", typeof(FacultiesResponseDto))]
        [SwaggerResponse(400, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(400, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Создать новый факультет",
            Description = "Создает новый факультет в системе"
        )]
        public async Task<ActionResult<FacultiesResponseDto>> CreateFaculty(
            [FromBody] FacultiesCreateDto createDto
        )
        {
            if (createDto == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.1",
                    Title = "Неверный запрос",
                    Message = "Тело запроса не может быть пустым"
                });
            }

            if (createDto.Name == null || createDto.Name.Trim() == string.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2",
                    Title = "Неверное название факультета",
                    Message = "Название факультета не может быть пустым"
                });
            }

            if (_context.Faculties.Any(f => f.Name == createDto.Name))
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "1.1",
                    Title = "Неверное название факультета",
                    Message = $"Факультет с названием \"{createDto.Name}\" уже существует"
                });
            }

            Faculties faculty = new()
            {
                Name = createDto.Name.Trim(),
                // создаём авбревиатуру из первых букв слов названия, если название не указано
                ShortName = string.IsNullOrWhiteSpace(createDto.ShortName)
                    ? string.Concat(createDto.Name
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(w => w[0]))
                    : createDto.ShortName.Trim()
            };

            _context.Faculties.Add(faculty);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFaculty), new { uuid = faculty.Uuid }, new FacultiesResponseDto(faculty));
        }


        [HttpDelete("{uuid}")]
        [SwaggerResponse(204, "Факультет удален")]
        [SwaggerResponse(400, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(400, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(404, "Факультет не найден", typeof(ApiError))]
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
                    StatusCode = "0.1",
                    Title = "Неверный запрос",
                    Message = "UUID факультета не может быть пустым"
                });
            }

            Faculties? faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.Uuid == uuid);
            if (faculty == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.1",
                    Title = "Факультет не найден",
                    Message = "В системе не найден факультет с указанным UUID"
                });
            }

            _context.Faculties.Remove(faculty);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPatch("{uuid}")]
        [SwaggerResponse(200, "Факультет обновлен", typeof(FacultiesResponseDto))]
        [SwaggerResponse(400, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(400, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(404, "Факультет не найден", typeof(ApiError))]
        [SwaggerResponseExample(404, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Обновить факультет",
            Description = "Обновляет факультет по его UUID"
        )]
        public async Task<ActionResult<FacultiesResponseDto>> UpdateFaculty(
            [SwaggerParameter("UUID факультета")] Guid uuid,
            [FromBody] FacultiesUpdateDto updateDto
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.1",
                    Title = "Неверный запрос",
                    Message = "UUID факультета не может быть пустым"
                });
            }

            Faculties? faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.Uuid == uuid);
            if (faculty == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.1",
                    Title = "Факультет не найден",
                    Message = "В системе не найден факультет с указанным UUID"
                });
            }

            if (updateDto.Name != null)
            {
                if (updateDto.Name.Trim() == string.Empty)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2",
                        Title = "Неверное название факультета",
                        Message = "Название факультета не может быть пустым"
                    });
                }

                if (_context.Faculties.Any(f => f.Name == updateDto.Name && f.Uuid != uuid))
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "1.2",
                        Title = "Неверное название факультета",
                        Message = $"Факультет с названием \"{updateDto.Name}\" уже существует"
                    });
                }

                faculty.Name = updateDto.Name.Trim();
            }

            if (updateDto.ShortName != null && updateDto.ShortName.Trim() != string.Empty)
            {
                faculty.ShortName = updateDto.ShortName.Trim();
            }
            else if (updateDto.ShortName != null) // если указано, но пустое, то генерируем аббревиатуру из названия
            {
                faculty.ShortName = string.Concat(faculty.Name
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(w => w[0]));
            }

            await _context.SaveChangesAsync();

            return Ok(new FacultiesResponseDto(faculty));
        }
    }
}
