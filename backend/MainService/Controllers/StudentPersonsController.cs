using MainService.Errors;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace MainService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class StudentPersonsController : ControllerBase
    {
        private readonly MainServiceContext _context;

        public StudentPersonsController(MainServiceContext context)
        {
            _context = context;
        }

        [HttpGet("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "StudentPerson найден", typeof(StudentPersonsResponseDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "StudentPerson не найден", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Получить StudentPerson по UUID",
            Description = "Возвращает StudentPerson по указанному UUID"
        )]
        public async Task<IActionResult> GetStudentPerson(Guid uuid)
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID StudentPerson не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            StudentPersons? studentPerson = await _context.StudentPersons
                .Include(sp => sp.User)
                .FirstOrDefaultAsync(sp => sp.Uuid == uuid);

            if (studentPerson == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "StudentPerson не найден",
                    Message = $"StudentPerson с UUID \"{uuid}\" не найден",
                    Field = nameof(uuid)
                });
            }

            return Ok(new StudentPersonsResponseDto(studentPerson));
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, "StudentPerson успешно создан", typeof(StudentPersonsResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Некорректные данные", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Создать нового StudentPerson"
        )]
        public async Task<ActionResult<StudentPersonsResponseDto>> CreateStudentPerson(
            [FromBody, SwaggerParameter("Данные нового StudentPerson")]
            StudentPersonsCreateDto createDto
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

            StudentPersons newStudentPerson = new()
            {
                Uuid = Guid.NewGuid(),
                FirstName = createDto.FirstName.Trim(),
                LastName = createDto.LastName.Trim(),
                Patronymic = string.IsNullOrWhiteSpace(createDto.Patronymic) ? null : createDto.Patronymic.Trim(),
                UserId = newUser.UserId,
                User = newUser
            };

            _context.StudentPersons.Add(newStudentPerson);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetStudentPerson),
                new { uuid = newStudentPerson.Uuid },
                new StudentPersonsResponseDto(newStudentPerson)
            );
        }

        [HttpDelete("{uuid}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "StudentPerson удалён")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "StudentPerson не найден", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Удалить StudentPerson по UUID"
        )]
        public async Task<IActionResult> DeleteStudentPerson(
            [SwaggerParameter("UUID StudentPerson")]
            Guid uuid
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID StudentPerson не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            StudentPersons? studentPerson = await _context.StudentPersons
                .FirstOrDefaultAsync(sp => sp.Uuid == uuid);

            if (studentPerson == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "StudentPerson не найден",
                    Message = $"StudentPerson с UUID \"{uuid}\" не найден",
                    Field = nameof(uuid)
                });
            }

            _context.StudentPersons.Remove(studentPerson);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "StudentPerson обновлён", typeof(StudentPersonsResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "StudentPerson не найден", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Обновить StudentPerson",
            Description = "Обновляет данные StudentPerson по его UUID. Все поля необязательны"
        )]
        public async Task<ActionResult<StudentPersonsResponseDto>> UpdateStudentPerson(
            [SwaggerParameter("UUID StudentPerson")]
            Guid uuid,
            [FromBody, SwaggerParameter("Данные для обновления")]
            StudentPersonsUpdateDto updateDto
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID StudentPerson не может быть пустым",
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

            StudentPersons? studentPerson = await _context.StudentPersons
                .Include(sp => sp.User)
                .FirstOrDefaultAsync(sp => sp.Uuid == uuid);

            if (studentPerson == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "StudentPerson не найден",
                    Message = $"StudentPerson с UUID \"{uuid}\" не найден",
                    Field = nameof(uuid)
                });
            }

            if (updateDto.FirstName != null)
            {
                studentPerson.FirstName = updateDto.FirstName.Trim();
            }

            if (updateDto.LastName != null)
            {
                studentPerson.LastName = updateDto.LastName.Trim();
            }

            if (updateDto.Patronymic != null)
            {
                studentPerson.Patronymic = updateDto.Patronymic.Trim() == string.Empty
                    ? null
                    : updateDto.Patronymic.Trim();
            }

            await _context.SaveChangesAsync();

            return Ok(new StudentPersonsResponseDto(studentPerson));
        }
    }
}
