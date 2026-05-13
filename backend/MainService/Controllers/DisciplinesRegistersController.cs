using MainService.Enums;
using MainService.Errors;

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
        [SwaggerResponse(200, "Записи реестра дисциплин найдены", typeof(IEnumerable<DisciplinesRegistersResponseDto>))]
        [SwaggerResponse(404, "Записи реестра дисциплин не найдены", typeof(ApiError))]
        [SwaggerResponseExample(404, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить список записей реестра дисциплин",
            Description = "Возвращает все записи реестра дисциплин"
        )]
        public async Task<ActionResult<PagedResult<DisciplinesRegistersResponseDto>>> GetDisciplinesRegisters(
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
            IQueryable<DisciplinesRegistersResponseDto> query = _context.DisciplinesRegisters
                .Where(r => string.IsNullOrEmpty(name) || r.Name.Contains(name))
                .SortByKey(r => r.Name, sortOrder)
                .TakeWithOffset(offset, size)
                .Select(r => new DisciplinesRegistersResponseDto
                {
                    Uuid = r.Uuid,
                    Name = r.Name,
                    ShortName = r.ShortName,
                    Version = r.Version
                });

            Task<int> totalTask = _context.DisciplinesRegisters.CountAsync();
            Task<List<DisciplinesRegistersResponseDto>> listTask = query.ToListAsync();

            List<DisciplinesRegistersResponseDto> registersList = await listTask;
            int totalCount = await totalTask;

            if (totalCount == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.1",
                    Title = "Записи реестра дисциплин не найдены",
                    Message = "В системе не найдено ни одной записи реестра дисциплин"
                });
            }

            if (registersList.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.2",
                    Title = "Записи реестра дисциплин не найдены",
                    Message = "В системе не найдено ни одной записи реестра дисциплин для указанных параметров запроса"
                });
            }

            return Ok(new PagedResult<DisciplinesRegistersResponseDto>(
                Total: totalCount,
                Offset: offset,
                Size: size,
                Items: registersList
            ));
        }

        [HttpGet("{uuid}")]
        [SwaggerResponse(200, "Запись реестра дисциплин найдена", typeof(DisciplinesRegistersResponseDto))]
        [SwaggerResponse(400, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(400, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(404, "Запись реестра дисциплин не найдена", typeof(ApiError))]
        [SwaggerResponseExample(404, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить запись реестра дисциплин по идентификатору",
            Description = "Возвращает одну запись реестра дисциплин по её UUID"
        )]
        public async Task<ActionResult<DisciplinesRegistersResponseDto>> GetDisciplineRegister(
            [SwaggerParameter("UUID записи реестра дисциплин")]
            Guid uuid
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.1",
                    Title = "Неверный запрос",
                    Message = "UUID записи реестра дисциплин не может быть пустым"
                });
            }

            DisciplinesRegistersResponseDto? register = await _context.DisciplinesRegisters
                .Where(r => r.Uuid == uuid)
                .Select(r => new DisciplinesRegistersResponseDto
                {
                    Uuid = r.Uuid,
                    Name = r.Name,
                    ShortName = r.ShortName,
                    Version = r.Version
                })
                .FirstOrDefaultAsync()
                ;

            if (register == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.1",
                    Title = "Запись реестра дисциплин не найдена",
                    Message = $"Запись реестра дисциплин с UUID \"{uuid}\" не найдена"
                });
            }

            return Ok(register);
        }

        [HttpPost]
        [SwaggerResponse(201, "Запись реестра дисциплин создана", typeof(DisciplinesRegistersResponseDto))]
        [SwaggerResponse(400, "Некорректные данные", typeof(ApiError))]
        [SwaggerResponseExample(400, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Создать новую запись в реестре дисциплин",
            Description = "Создает новую запись в реестре дисциплин"
        )]
        public async Task<ActionResult<DisciplinesRegistersResponseDto>> CreateDisciplineRegister(
            [FromBody, SwaggerParameter("Данные новой записи реестра дисциплин")]
            DisciplinesRegistersCreateDto createDto
        )
        {
            if (string.IsNullOrWhiteSpace(createDto.Name))
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.1",
                    Title = "Неверный запрос",
                    Message = "Название записи реестра дисциплин не может быть пустым"
                });
            }

            DisciplinesRegisters register = new()
            {
                Name = createDto.Name.Trim(),
                ShortName = string.IsNullOrWhiteSpace(createDto.ShortName)
                    ? string.Concat(createDto.Name
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(w => w[0]))
                    : createDto.ShortName.Trim()
            };

            _context.DisciplinesRegisters.Add(register);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDisciplineRegister), new { uuid = register.Uuid }, new DisciplinesRegistersResponseDto
            {
                Uuid = register.Uuid,
                Name = register.Name,
                ShortName = register.ShortName,
                Version = register.Version
            });
        }

        [HttpDelete("{uuid}")]
        [SwaggerResponse(204, "Запись реестра дисциплин удалена")]
        [SwaggerResponse(400, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(400, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(404, "Запись реестра дисциплин не найдена", typeof(ApiError))]
        [SwaggerResponseExample(404, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Удалить запись реестра дисциплин по идентификатору"
        )]
        public async Task<IActionResult> DeleteDisciplineRegister(
            [SwaggerParameter("UUID записи реестра дисциплин")]
            Guid uuid
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.1",
                    Title = "Неверный запрос",
                    Message = "UUID записи реестра дисциплин не может быть пустым"
                });
            }

            DisciplinesRegisters? register = await _context.DisciplinesRegisters.FirstOrDefaultAsync(r => r.Uuid == uuid);
            if (register == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.1",
                    Title = "Запись реестра дисциплин не найдена",
                    Message = $"Запись реестра дисциплин с UUID \"{uuid}\" не найдена"
                });
            }

            _context.DisciplinesRegisters.Remove(register);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPatch("{uuid}")]
        [SwaggerResponse(200, "Запись реестра дисциплин обновлена", typeof(DisciplinesRegistersResponseDto))]
        [SwaggerResponse(400, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(400, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(404, "Запись реестра дисциплин не найдена", typeof(ApiError))]
        [SwaggerResponseExample(404, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Обновить запись реестра дисциплин",
            Description = "Обновляет данные записи реестра дисциплин по её UUID. Все поля необязательны"
        )]
        public async Task<ActionResult<DisciplinesRegistersResponseDto>> UpdateDisciplineRegister(
            [SwaggerParameter("UUID записи реестра дисциплин")]
            Guid uuid,
            [FromBody, SwaggerParameter("Данные для обновления")]
            DisciplinesRegistersUpdateDto updateDto
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.1",
                    Title = "Неверный запрос",
                    Message = "UUID записи реестра дисциплин не может быть пустым"
                });
            }

            DisciplinesRegisters? register = await _context.DisciplinesRegisters.FirstOrDefaultAsync(r => r.Uuid == uuid);
            if (register == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.1",
                    Title = "Запись реестра дисциплин не найдена",
                    Message = $"Запись реестра дисциплин с UUID \"{uuid}\" не найдена"
                });
            }

            if (updateDto.Name != null)
            {
                if (string.IsNullOrWhiteSpace(updateDto.Name))
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2",
                        Title = "Неверный запрос",
                        Message = "Название записи реестра дисциплин не может быть пустым"
                    });
                }
                register.Name = updateDto.Name.Trim();
            }

            if (updateDto.ShortName != null)
            {
                if (updateDto.ShortName.Trim() != string.Empty)
                {
                    register.ShortName = updateDto.ShortName.Trim();
                }
                else
                {
                    register.ShortName = string.Concat(register.Name
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(w => w[0]));
                }
            }

            await _context.SaveChangesAsync();

            return Ok(await _context.DisciplinesRegisters
                .Where(r => r.Uuid == uuid)
                .Select(r => new DisciplinesRegistersResponseDto
                {
                    Uuid = r.Uuid,
                    Name = r.Name,
                    ShortName = r.ShortName,
                    Version = r.Version
                })
                .FirstAsync());
        }
    }
}
