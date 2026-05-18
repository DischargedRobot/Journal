using AuthService.Model;
using AuthService.Lib.Utils;

using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using AuthService.Errors;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Controller
{
    [ApiController]
    [Route("api/auth-service/v1/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AuthServiceContext _context;

        public UsersController(AuthServiceContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Пользователи найдены", typeof(IEnumerable<object>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int? offset,
            [FromQuery] int? limit)
        {
            int l = limit.HasValue && limit.Value > 0 ? Math.Min(limit.Value, 100) : 100;
            int o = offset.GetValueOrDefault(0);

            var users = await _context.Users
                .OrderBy(u => u.UserId)
                .Skip(o)
                .Take(l)
                .Select(u => new
                {
                    u.Uuid,
                    u.Login,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.Patronymic,
                    u.TokenVersion
                })
                .ToListAsync();

            return Ok(users);
        }


        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, "Пользователь создан", typeof(object))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Некорректные данные", typeof(ApiError))]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Конфликт: логин занят", typeof(ApiError))]
        [ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.0", "Неверный запрос", "Логин или пароль не могут быть пустыми")]
        [ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "FirstName или LastName обязательны")]
        [ApiErrorExample(StatusCodes.Status409Conflict, "1.1.1", "Конфликт", "Пользователь с таким логином уже существует", "Login")]
        public async Task<IActionResult> CreateUser(
            [FromBody]
            UsersCreateDto createDto
        )
        {
            if (createDto == null) return BadRequest();
            if (string.IsNullOrWhiteSpace(createDto.Login) || string.IsNullOrWhiteSpace(createDto.Password))
            {
                return BadRequest(new
                {
                    error = "Login and Password are required"
                });
            }

            if (string.IsNullOrWhiteSpace(createDto.FirstName))
            {
                return BadRequest(new { error = "FirstName is required" });
            }

            if (string.IsNullOrWhiteSpace(createDto.LastName))
            {
                return BadRequest(new { error = "LastName is required" });
            }

            bool exists = await _context.Users.AnyAsync(u => u.Login == createDto.Login);
            if (exists) return Conflict(new { error = "User with this login already exists" });

            Users user = new()
            {
                Uuid = Guid.NewGuid(),
                Login = createDto.Login.Trim(),
                PasswordHash = HashingPassword.ComputeHash(createDto.Password, string.Empty),
                Email = string.IsNullOrWhiteSpace(createDto.Email) ? null : createDto.Email.Trim(),
                FirstName = string.IsNullOrWhiteSpace(createDto.FirstName) ? null : createDto.FirstName.Trim(),
                LastName = string.IsNullOrWhiteSpace(createDto.LastName) ? null : createDto.LastName.Trim(),
                Patronymic = string.IsNullOrWhiteSpace(createDto.Patronymic) ? null : createDto.Patronymic.Trim()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { uuid = user.Uuid }, new
            {
                user.Uuid,
                user.Login,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Patronymic,
                user.TokenVersion
            });
        }


        [HttpGet("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Пользователь найден", typeof(object))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Пользователь не найден", typeof(ApiError))]
        [ApiErrorExample(StatusCodes.Status404NotFound, "1.2.3", "Пользователь не найден", "Пользователь с указанным UUID не найден", "uuid")]
        public async Task<IActionResult> GetUser(Guid uuid)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .Where(u => u.Uuid == uuid)
                .Select(u => new
                {
                    u.Uuid,
                    u.Login,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.Patronymic,
                    u.TokenVersion,
                    Roles = u.Roles != null ? u.Roles.Select(r => new { r.Uuid, r.Name }) : null
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpDelete("{uuid}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Пользователь удалён")]
        public async Task<IActionResult> DeleteUser(Guid uuid)
        {
            Users? user = await _context.Users.FirstOrDefaultAsync(u => u.Uuid == uuid);
            if (user == null)
            {
                return NoContent();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Пользователь обновлён", typeof(object))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Пользователь не найден", typeof(ApiError))]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Конфликт: логин уже используется", typeof(ApiError))]
        [ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "Неверный формат данных")]
        [ApiErrorExample(StatusCodes.Status409Conflict, "1.1.1", "Конфликт", "Логин уже используется", "Login")]
        public async Task<IActionResult> UpdateUser(Guid uuid, [FromBody] UsersUpdateDto? request)
        {
            if (request == null)
            {
                return BadRequest();
            }
            Users? user = await _context.Users.FirstOrDefaultAsync(u => u.Uuid == uuid);
            if (user == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(request.Login) && request.Login != user.Login)
            {
                bool exists = await _context.Users.AnyAsync(u => u.Login == request.Login && u.Uuid != uuid);
                if (exists) return Conflict(new { error = "Login already in use" });
                user.Login = request.Login;
            }

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                // Если пароль совпадает с текущим — не обновляем и не инкрементируем версию токена
                bool same = HashingPassword.VerifyPassword(request.Password, user.PasswordHash);
                if (!same)
                {
                    user.PasswordHash = HashingPassword.ComputeHash(request.Password, string.Empty);
                    user.TokenVersion++;
                }
            }

            if (request.Email != null)
            {
                user.Email = request.Email;
            }
            if (request.FirstName != null)
            {
                user.FirstName = request.FirstName;
            }
            if (request.LastName != null)
            {
                user.LastName = request.LastName;
            }
            if (request.Patronymic != null)
            {
                user.Patronymic = request.Patronymic;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                user.Uuid,
                user.Login,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Patronymic,
                user.TokenVersion
            });
        }
    }
}
