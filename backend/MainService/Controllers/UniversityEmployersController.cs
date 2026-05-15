using MainService.Errors;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace MainService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UniversityEmployersController : ControllerBase
    {
        private readonly MainServiceContext _context;

        public UniversityEmployersController(MainServiceContext context)
        {
            _context = context;
        }

        [HttpGet("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Сотрудник найден", typeof(UniversityEmployersResponseDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Сотрудник не найден", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Получить сотрудника по UUID",
            Description = "Возвращает сотрудника университета по указанному UUID"
        )]
        public async Task<ActionResult<UniversityEmployersResponseDto>> GetUniversityEmployer(
            [SwaggerParameter("UUID сотрудника")]
            Guid uuid
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID сотрудника не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            UniversityEmployers? employee = await _context.UniversityEmployers
                .Include(e => e.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Uuid == uuid);

            if (employee == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Сотрудник не найден",
                    Message = $"Сотрудник с UUID \"{uuid}\" не найден",
                    Field = nameof(uuid)
                });
            }

            return Ok(new UniversityEmployersResponseDto(employee));
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, "Сотрудник успешно создан", typeof(UniversityEmployersResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Некорректные данные", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Создать нового сотрудника"
        )]
        public async Task<ActionResult<UniversityEmployersResponseDto>> CreateUniversityEmployer(
            [FromBody, SwaggerParameter("Данные нового сотрудника")]
            UniversityEmployersCreateDto createDto
        )
        {
            if (string.IsNullOrWhiteSpace(createDto.FirstName))
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "Имя не может быть пустым",
                    Field = nameof(createDto.FirstName)
                });
            }

            if (string.IsNullOrWhiteSpace(createDto.LastName))
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "Фамилия не может быть пустой",
                    Field = nameof(createDto.LastName)
                });
            }

            Users newUser = new()
            {
                Uuid = Guid.NewGuid(),
                UserUuid = Guid.NewGuid().ToString(),
                Role = createDto.Role
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            UniversityEmployers newEmployee = new()
            {
                Uuid = Guid.NewGuid(),
                FirstName = createDto.FirstName.Trim(),
                LastName = createDto.LastName.Trim(),
                Patronymic = string.IsNullOrWhiteSpace(createDto.Patronymic) 
                    ? null 
                    : createDto.Patronymic.Trim(),
                UserId = newUser.UserId,
                User = newUser
            };

            _context.UniversityEmployers.Add(newEmployee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetUniversityEmployer),
                new { uuid = newEmployee.Uuid },
                new UniversityEmployersResponseDto(newEmployee)
            );
        }

        [HttpDelete("{uuid}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Сотрудник удалён")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Сотрудник не найден", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Удалить сотрудника по UUID"
        )]
        public async Task<IActionResult> DeleteUniversityEmployer(
            [SwaggerParameter("UUID сотрудника")]
            Guid uuid
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID сотрудника не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            UniversityEmployers? employee = await _context.UniversityEmployers
                .FirstOrDefaultAsync(e => e.Uuid == uuid);

            if (employee == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Сотрудник не найден",
                    Message = $"Сотрудник с UUID \"{uuid}\" не найден",
                    Field = nameof(uuid)
                });
            }

            _context.UniversityEmployers.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Сотрудник обновлён", typeof(UniversityEmployersResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Сотрудник не найден", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Обновить сотрудника",
            Description = "Обновляет данные сотрудника по его UUID. Все поля необязательны"
        )]
        public async Task<ActionResult<UniversityEmployersResponseDto>> UpdateUniversityEmployer(
            [SwaggerParameter("UUID сотрудника")]
            Guid uuid,
            [FromBody, SwaggerParameter("Данные для обновления")]
            UniversityEmployersUpdateDto updateDto
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID сотрудника не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            if (updateDto.FirstName != null && updateDto.FirstName.Trim() == string.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "Имя не может быть пустым",
                    Field = nameof(updateDto.FirstName)
                });
            }

            if (updateDto.LastName != null && updateDto.LastName.Trim() == string.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "Фамилия не может быть пустой",
                    Field = nameof(updateDto.LastName)
                });
            }

            UniversityEmployers? employee = await _context.UniversityEmployers
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Uuid == uuid);

            if (employee == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Сотрудник не найден",
                    Message = $"Сотрудник с UUID \"{uuid}\" не найден",
                    Field = nameof(uuid)
                });
            }

            if (updateDto.FirstName != null)
            {
                employee.FirstName = updateDto.FirstName.Trim();
            }

            if (updateDto.LastName != null)
            {
                employee.LastName = updateDto.LastName.Trim();
            }

            if (updateDto.Patronymic != null)
            {
                employee.Patronymic = updateDto.Patronymic.Trim() == string.Empty
                    ? null
                    : updateDto.Patronymic.Trim();
            }

            await _context.SaveChangesAsync();

            return Ok(new UniversityEmployersResponseDto(employee));
        }
    }
}