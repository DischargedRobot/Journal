using MainService.EntityDtoExamples;
using MainService.Errors;

using Microsoft.AspNetCore.Mvc;
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
        [SwaggerResponse(StatusCodes.Status200OK, "Факультеты найдены", typeof(IEnumerable<FacultiesResponseDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Факультеты не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
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
            IQueryable<FacultiesResponseDto> query = _context.Faculties
                .SortByKey(f => string.IsNullOrWhiteSpace(name) || f.Name.Contains(name), sortOrder)
                .TakeWithOffset(offset, size)
                .Select(f => new FacultiesResponseDto
                {
                    Uuid = f.Uuid,
                    Name = f.Name,
                    ShortName = f.ShortName,
                    Version = f.Version
                })
                .AsNoTracking();

            Task<int> totalTask = _context.Faculties.CountAsync();
            Task<List<FacultiesResponseDto>> listTask = query.ToListAsync();

            List<FacultiesResponseDto> faculties = await listTask;
            int totalCount = await totalTask;

            if (totalCount == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "0.0.3",
                    Title = "Факультеты не найдены",
                    Message = "В системе не найдено ни одного факультета",
                    Field = string.Empty
                });
            }

            if (faculties.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "0.0.3",
                    Title = "Факультеты не найдены",
                    Message = "В системе не найдено ни одного факультета для указанных параметров запроса",
                    Field = string.Empty
                });
            }

            return Ok(new PagedResult<FacultiesResponseDto>(
                Total: totalCount,
                Offset: offset,
                Size: size,
                Items: faculties
            ));
        }


        [HttpGet("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Факультет найден", typeof(FacultiesResponseDto))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(FacultiesDtoExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Факультет не найден", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
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
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID факультета не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            Faculties? faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.Uuid == uuid);
            if (faculty == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "0.0.3",
                    Title = "Факультет не найден",
                    Message = "В системе не найден факультет с указанным UUID",
                    Field = nameof(uuid)
                });
            }

            return Ok(new FacultiesResponseDto(faculty));
        }


        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, "Факультет создан", typeof(FacultiesResponseDto))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(FacultiesDtoExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Создать новый факультет",
            Description = "Создает новый факультет в системе"
        )]
        public async Task<ActionResult<FacultiesResponseDto>> CreateFaculty(
            [FromBody] FacultiesCreateDto createDto
        )
        {
            // проверка перед запросом к бд

            if (createDto == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.1.0",
                    Title = "Неверный запрос",
                    Message = "Тело запроса не может быть пустым",
                    Field = "BODY"
                });
            }

            if (createDto.Name == null || createDto.Name.Trim() == string.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверное название факультета",
                    Message = "Название факультета не может быть пустым",
                    Field = nameof(createDto.Name)
                });
            }

            if (_context.Faculties.Any(f => f.Name == createDto.Name))
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.1",
                    Title = "Неверное название факультета",
                    Message = $"Факультет с названием \"{createDto.Name}\" уже существует",
                    Field = nameof(createDto.Name)
                });
            }

            Faculties faculty = new()
            {
                Name = createDto.Name.Trim(),
                // создаём авбревиатуру из первых букв слов названия, 
                // если сокращение не указано
                ShortName = string.IsNullOrWhiteSpace(createDto.ShortName)
                    ? string.Concat(createDto.Name
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(w => w[0]))
                    : createDto.ShortName.Trim()
            };

            _context.Faculties.Add(faculty);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetFaculty),
                new { uuid = faculty.Uuid },
                new FacultiesResponseDto(faculty)
                );
        }


        [HttpDelete("{uuid}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Факультет удален")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Факультет не найден", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
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
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID факультета не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            Faculties? faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.Uuid == uuid);
            if (faculty == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "0.0.3",
                    Title = "Факультет не найден",
                    Message = "В системе не найден факультет с указанным UUID",
                    Field = nameof(uuid)
                });
            }

            _context.Faculties.Remove(faculty);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPatch("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Факультет обновлен", typeof(FacultiesResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Факультет не найден", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Обновить факультет",
            Description = "Обновляет факультет по его UUID. Все поля необязательны. " +
                          "Если передать shortName как пустую строку — аббревиатура будет сгенерирована автоматически из названия. " +
                          "Если не передавать shortName вовсе — текущее значение останется без изменений."
        )]
        public async Task<ActionResult<FacultiesResponseDto>> UpdateFaculty(
            [SwaggerParameter("UUID факультета")]
            Guid uuid,
            [FromBody, SwaggerParameter("Данные для обновления факультета")]
            FacultiesUpdateDto updateDto
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID факультета не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            // Предварительная валидация DTO (без обращения к БД)
            if (updateDto.Name != null && updateDto.Name.Trim() == string.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверное название факультета",
                    Message = "Название факультета не может быть пустым",
                    Field = nameof(updateDto.Name)
                });
            }

            // Загрузка сущности и дальнейшие проверки
            Faculties? faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.Uuid == uuid);
            if (faculty == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "0.0.3",
                    Title = "Факультет не найден",
                    Message = "В системе не найден факультет с указанным UUID",
                    Field = nameof(uuid)
                });
            }

            if (updateDto.Name != null)
            {
                if (_context.Faculties.Any(f => f.Name == updateDto.Name
                    && f.Uuid != uuid))
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.1",
                        Title = "Неверное название факультета",
                        Message = $"Факультет с названием \"{updateDto.Name}\" уже существует",
                        Field = nameof(updateDto.Name)
                    });
                }

                faculty.Name = updateDto.Name.Trim();
            }

            // Если не передаётся — текущее значение остаётся без изменений
            if (updateDto.ShortName != null)
            {
                // если указано, но пустое, то генерируем аббревиатуру из названия
                string shortNameDto = updateDto.ShortName.Trim();
                faculty.ShortName = shortNameDto != string.Empty
                    ? shortNameDto
                    : string.Concat(faculty.Name
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(w => w[0]));
            }

            await _context.SaveChangesAsync();

            return Ok(new FacultiesResponseDto(faculty));
        }
    }
}
