using AuthService.Enums;
using AuthService.Errors;
using AuthService.Lib.Utils;
using AuthService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace AuthService.Controller
{
	[ApiController]
	[Route("api/auth-service/v1/[controller]")]
	[Produces("application/json")]
	public class UsersController : ControllerBase
	{
		private readonly AuthServiceContext _context;

		public UsersController(AuthServiceContext context)
		{
			_context = context;
		}

		[HttpGet]
		[SwaggerResponse(
			StatusCodes.Status200OK,
			"Пользователи найдены",
			typeof(PagedResult<UsersResponseDto>)
		)]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
		[SwaggerOperation(Summary = "Получить список пользователей")]
		public async Task<ActionResult<PagedResult<UsersResponseDto>>> GetUsers(
			[FromQuery, SwaggerParameter("Количество записей")] int size = 100,
			[FromQuery, SwaggerParameter("Смещение от начала списка")] int offset = 0,
			[FromQuery, SwaggerParameter("Фильтр по логину")] string? filterLogin = null,
			[FromQuery, SwaggerParameter("Порядок сортировки по логину")]
				SortOrder sortOrder = SortOrder.Ascending
		)
		{
			if (offset < 0)
			{
				return BadRequest(
					new ApiError
					{
						StatusCode = "0.2.1",
						Title = "Неверный запрос",
						Message = "Параметр offset не может быть отрицательным",
						Field = nameof(offset),
					}
				);
			}

			if (size < 0)
			{
				return BadRequest(
					new ApiError
					{
						StatusCode = "0.2.1",
						Title = "Неверный запрос",
						Message = "Параметр size не может быть отрицательным",
						Field = nameof(size),
					}
				);
			}

			IQueryable<Users> baseQuery = _context
				.Users.Where(u => filterLogin == null || u.Login.Contains(filterLogin))
				.AsNoTracking();

			Task<int> totalRecord = baseQuery.CountAsync();

			List<UsersResponseDto> items = await baseQuery
				.SortByKey(u => u.Login, sortOrder)
				.TakeWithOffset(offset, size)
				.Select(u => new UsersResponseDto(u))
				.ToListAsync();

			int total = await totalRecord;

			if (total == 0)
			{
				return NotFound(
					new ApiError
					{
						StatusCode = "1.0.3",
						Title = "Пользователи не найдены",
						Message = "В системе не найдено ни одного пользователя",
						Field = string.Empty,
					}
				);
			}

			if (items.Count == 0)
			{
				return NotFound(
					new ApiError
					{
						StatusCode = "1.1.3",
						Title = "Пользователи не найдены",
						Message =
							"В системе не найдено ни одного пользователя для указанных параметров запроса",
						Field = "BODY",
					}
				);
			}

			PagedResult<UsersResponseDto> result = new(
				Total: total,
				Offset: offset,
				Size: items.Count,
				Items: items
			);

			return Ok(result);
		}

		[HttpPost]
		[SwaggerResponse(StatusCodes.Status201Created, "Пользователь создан", typeof(object))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Некорректные данные", typeof(ApiError))]
		[SwaggerResponse(StatusCodes.Status409Conflict, "Конфликт: логин занят", typeof(ApiError))]
		[ApiErrorExample(
			StatusCodes.Status400BadRequest,
			"0.2.0",
			"Неверный запрос",
			"Логин или пароль не могут быть пустыми",
			"Login, Password"
		)]
		[ApiErrorExample(
			StatusCodes.Status400BadRequest,
			"0.2.1",
			"Неверный запрос",
			"FirstName или LastName обязательны",
			"FirstName, LastName"
		)]
		[ApiErrorExample(
			StatusCodes.Status409Conflict,
			"1.1.1",
			"Конфликт",
			"Пользователь с таким логином уже существует",
			"Login"
		)]
		[SwaggerOperation(Summary = "Создать нового пользователя")]
		public async Task<IActionResult> CreateUser([FromBody] UsersCreateDto createDto)
		{
			if (createDto == null)
			{
				return BadRequest(
					new ApiError
					{
						StatusCode = "0.2.1",
						Title = "Неверный запрос",
						Message = "Неверный формат данных",
						Field = "BODY",
					}
				);
			}

			if (
				string.IsNullOrWhiteSpace(createDto.Login)
				|| string.IsNullOrWhiteSpace(createDto.Password)
			)
			{
				return BadRequest(
					new ApiError
					{
						StatusCode = "0.2.1",
						Title = "Неверный запрос",
						Message = "Логин или пароль не могут быть пустыми",
						Field = $"{nameof(createDto.Login)}, {nameof(createDto.Password)}",
					}
				);
			}

			if (string.IsNullOrWhiteSpace(createDto.FirstName))
			{
				return BadRequest(
					new ApiError
					{
						StatusCode = "0.2.1",
						Title = "Неверный запрос",
						Message = $"{nameof(createDto.FirstName)} обязательно",
						Field = nameof(createDto.FirstName),
					}
				);
			}

			if (string.IsNullOrWhiteSpace(createDto.LastName))
			{
				return BadRequest(
					new ApiError
					{
						StatusCode = "0.2.1",
						Title = "Неверный запрос",
						Message = $"{nameof(createDto.LastName)} обязательно",
						Field = nameof(createDto.LastName),
					}
				);
			}

			bool exists = await _context.Users.AnyAsync(u => u.Login == createDto.Login);
			if (exists)
				return Conflict(
					new ApiError
					{
						StatusCode = "1.1.1",
						Title = "Конфликт",
						Message = $"Пользователь с таким {nameof(createDto.Login)} уже существует",
						Field = nameof(createDto.Login),
					}
				);

			Users user = new()
			{
				Uuid = Guid.NewGuid(),
				Login = createDto.Login.Trim(),
				PasswordHash = HashingPassword.ComputeHash(createDto.Password, string.Empty),
				Email = string.IsNullOrWhiteSpace(createDto.Email) ? null : createDto.Email.Trim(),
				FirstName = string.IsNullOrWhiteSpace(createDto.FirstName)
					? null
					: createDto.FirstName.Trim(),
				LastName = string.IsNullOrWhiteSpace(createDto.LastName)
					? null
					: createDto.LastName.Trim(),
				Patronymic = string.IsNullOrWhiteSpace(createDto.Patronymic)
					? null
					: createDto.Patronymic.Trim(),
			};

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			return CreatedAtAction(
				nameof(GetUser),
				new { uuid = user.Uuid },
				new
				{
					user.Uuid,
					user.Login,
					user.Email,
					user.FirstName,
					user.LastName,
					user.Patronymic,
					user.TokenVersion,
				}
			);
		}

		[HttpGet("{uuid}")]
		[SwaggerResponse(StatusCodes.Status200OK, "Пользователь найден", typeof(object))]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Пользователь не найден", typeof(ApiError))]
		[ApiErrorExample(
			StatusCodes.Status404NotFound,
			"1.2.3",
			"Пользователь не найден",
			"Пользователь с указанным UUID не найден",
			nameof(uuid)
		)]
		[SwaggerOperation(Summary = "Получить пользователя по UUID")]
		public async Task<IActionResult> GetUser(Guid uuid)
		{
			var user = await _context
				.Users.Include(u => u.Roles)
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
					Roles = u.Roles != null ? u.Roles.Select(r => new { r.Uuid, r.Name }) : null,
				})
				.FirstOrDefaultAsync();

			if (user == null)
			{
				return NotFound(
					new ApiError
					{
						StatusCode = "1.2.3",
						Title = "Пользователь не найден",
						Message = "Пользователь с указанным UUID не найден",
						Field = nameof(uuid),
					}
				);
			}

			return Ok(user);
		}

		[HttpDelete("{uuid}")]
		[SwaggerResponse(StatusCodes.Status204NoContent, "Пользователь удалён")]
		[SwaggerOperation(Summary = "Удалить пользователя по UUID")]
		public async Task<IActionResult> DeleteUser(Guid uuid)
		{
			Users? user = await _context.Users.FirstOrDefaultAsync(u => u.Uuid == uuid);
			if (user == null)
			{
				return NotFound(
					new ApiError
					{
						StatusCode = "1.2.3",
						Title = "Пользователь не найден",
						Message = "Пользователь с указанным UUID не найден",
						Field = nameof(uuid),
					}
				);
			}

			_context.Users.Remove(user);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		[HttpPatch("{uuid}")]
		[SwaggerResponse(StatusCodes.Status200OK, "Пользователь обновлён", typeof(object))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Пользователь не найден", typeof(ApiError))]
		[SwaggerResponse(
			StatusCodes.Status409Conflict,
			"Конфликт: логин уже используется",
			typeof(ApiError)
		)]
		[ApiErrorExample(
			StatusCodes.Status400BadRequest,
			"0.2.1",
			"Неверный запрос",
			"Неверный формат данных"
		)]
		[ApiErrorExample(
			StatusCodes.Status409Conflict,
			"1.1.1",
			"Конфликт",
			"Логин уже используется",
			nameof(UsersUpdateDto.Login)
		)]
		[SwaggerOperation(Summary = "Обновить данные пользователя по UUID")]
		public async Task<IActionResult> UpdateUser(Guid uuid, [FromBody] UsersUpdateDto? request)
		{
			if (request == null)
			{
				return BadRequest(
					new ApiError
					{
						StatusCode = "0.2.1",
						Title = "Неверный запрос",
						Message = "Неверный формат данных",
						Field = "BODY",
					}
				);
			}

			Users? user = await _context.Users.FirstOrDefaultAsync(u => u.Uuid == uuid);
			if (user == null)
			{
				return NotFound(
					new ApiError
					{
						StatusCode = "1.2.3",
						Title = "Пользователь не найден",
						Message = "Пользователь с указанным UUID не найден",
						Field = nameof(uuid),
					}
				);
			}

			if (!string.IsNullOrWhiteSpace(request.Login) && request.Login != user.Login)
			{
				bool exists = await _context.Users.AnyAsync(u =>
					u.Login == request.Login && u.Uuid != uuid
				);
				if (exists)
					return Conflict(
						new ApiError
						{
							StatusCode = "1.2.1",
							Title = "Конфликт",
							Message = "Логин уже используется",
							Field = nameof(UsersUpdateDto.Login),
						}
					);
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

			return Ok(
				new
				{
					user.Uuid,
					user.Login,
					user.Email,
					user.FirstName,
					user.LastName,
					user.Patronymic,
					user.TokenVersion,
				}
			);
		}
	}
}
