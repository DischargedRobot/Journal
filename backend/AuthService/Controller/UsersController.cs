using System.Diagnostics;

using AuthService.Enums;
using AuthService.Errors;
using AuthService.Lib.Utils;
using AuthService.Model;
using AuthService.ResponseExample;

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
		private readonly ILogger<UsersController> _logger;
		private readonly ActivitySource _activitySource;

		public UsersController(AuthServiceContext context, ILogger<UsersController> logger, ActivitySource activitySource)
		{
			_context = context;
			_logger = logger;
			_activitySource = activitySource;
		}

		[HttpGet]
		[SwaggerResponse(StatusCodes.Status200OK, "Пользователи найдены", typeof(PagedResult<UsersResponseDto>))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "Параметр offset не может быть отрицательным", nameof(offset))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "Параметр size не может быть отрицательным", nameof(size))]
		[ApiErrorExample(StatusCodes.Status404NotFound, "1.0.3", "Пользователи не найдены", "В системе не найдено ни одного пользователя", "")]
		[ApiErrorExample(StatusCodes.Status404NotFound, "1.1.3", "Пользователи не найдены", "В системе не найдено ни одного пользователя для указанных параметров запроса", "BODY")]
		[SwaggerOperation(Summary = "Получить список пользователей")]
		[ResponseExample(StatusCodes.Status200OK, typeof(PagedResult<UsersResponseDto>))]
		public async Task<ActionResult<PagedResult<UsersResponseDto>>> GetUsers(
			[FromQuery, SwaggerParameter("Количество записей")]
			int size = 100,
			[FromQuery, SwaggerParameter("Смещение от начала списка")]
			int offset = 0,
			[FromQuery, SwaggerParameter("Фильтр по логину")]
			string? filterLogin = null,
			[FromQuery, SwaggerParameter("Порядок сортировки по логину")]
			SortOrder sortOrder = SortOrder.Ascending
		)
		{
			string functionName = ControllerContext.ActionDescriptor.ActionName;
			try
			{
				using Activity? activity = _activitySource.StartAndLog(_logger, this);
				_logger.LogInformation("{Function} вызвано: size={Size}, offset={Offset}, filterLogin={FilterLogin}, sortOrder={SortOrder}", functionName, size, offset, filterLogin, sortOrder);

				if (offset < 0)
				{
					_logger.LogWarning("{Function}: неверный offset {Offset}", functionName, offset);
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
					_logger.LogWarning("{Function}: неверный size {Size}", functionName, size);
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
					_logger.LogInformation("{Function}: пользователей не найдено (total=0)", functionName);
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
					_logger.LogInformation("{Function}: нет пользователей по фильтру (total={Total}, offset={Offset})", functionName, total, offset);
					return NotFound(
						new ApiError
						{
							StatusCode = "1.1.3",
							Title = "Пользователи не найдены",
							Message = "В системе не найдено ни одного пользователя для указанных параметров запроса",
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

				_logger.LogInformation("{Function}: возвращает {Count} элементов (offset={Offset}, total={Total})", functionName, items.Count, offset, total);

				return Ok(result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "{Function}: неожиданная ошибка при получении пользователей", functionName);
				return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
				{
					StatusCode = "1.0.0",
					Title = "Внутренняя ошибка сервера",
					Message = "Произошла ошибка на сервере",
				});
			}
		}


		[HttpPost]
		[SwaggerResponse(StatusCodes.Status201Created, "Пользователь создан", typeof(UsersResponseDto))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Некорректные данные", typeof(ApiError))]
		[SwaggerResponse(StatusCodes.Status409Conflict, "Конфликт: логин занят", typeof(ApiError))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.1.0", "Пустой запрос", "Тело запроса не может быть пустым", "BODY")]
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
		public async Task<ActionResult<UsersResponseDto>> CreateUser(
			[FromBody] UsersCreateDto createDto
			)
		{
			string functionName = ControllerContext.ActionDescriptor.ActionName;
			try
			{
				using Activity? activity = _activitySource.StartAndLog(_logger, this);
				_logger.LogInformation("{Function}: вызвано CreateUser", functionName);

				if (createDto == null)
				{
					_logger.LogWarning("{Function}: пустой createDto", functionName);
					return BadRequest(
						new ApiError
						{
							StatusCode = "0.1.0",
							Title = "Неверный запрос",
							Message = "Тело запроса не может быть пустым",
							Field = "BODY",
						}
					);
				}

				if (
					string.IsNullOrWhiteSpace(createDto.Login)
					|| string.IsNullOrWhiteSpace(createDto.Password)
				)
				{
					_logger.LogWarning("{Function}: пустой Login или Password", functionName);
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
				{
					_logger.LogWarning("{Function}: пользователь с логином уже существует: {Login}", functionName, createDto.Login);
					return Conflict(
						new ApiError
						{
							StatusCode = "1.1.1",
							Title = "Конфликт",
							Message = $"Пользователь с таким {nameof(createDto.Login)} уже существует",
							Field = nameof(createDto.Login),
						}
					);
				}

				Users user = new()
				{
					Uuid = Guid.NewGuid(),
					Login = createDto.Login.Trim(),
					PasswordHash = HashingPassword.ComputeHash(createDto.Password),
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
					TokenVersion = 0,
					Roles = new List<Roles>()
				};

				if (createDto.RolesUuid != null)
				{
					Guid[] roleUuids = createDto.RolesUuid.Distinct().ToArray();
					List<Roles> foundRoles = await _context.Roles
						.Where(r => roleUuids.Contains(r.Uuid))
						.ToListAsync();

					if (foundRoles.Count != roleUuids.Length)
					{
						Guid[] missing = roleUuids.Except(foundRoles.Select(r => r.Uuid)).ToArray();
						_logger.LogWarning("{Function}: некоторые роли не найдены: {Missing}", functionName, string.Join(", ", missing));
						return BadRequest(
							new ApiError
							{
								StatusCode = "0.2.3",
								Title = "Неверный запрос",
								Message = "Одна или несколько ролей не найдены",
								Field = nameof(createDto.RolesUuid),
								Details = string.Join(", ", missing),
							}
						);
					}

					user.Roles = foundRoles;
				}

				_context.Users.Add(user);
				await _context.SaveChangesAsync();

				_logger.LogInformation("{Function}: создан пользователь uuid={Uuid}", functionName, user.Uuid);
				return CreatedAtAction(
					nameof(GetUser),
					new { uuid = user.Uuid },
					new UsersResponseDto(user)
				);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "{Function}: неожиданная ошибка при создании пользователя", functionName);
				return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
				{
					StatusCode = "1.0.0",
					Title = "Внутренняя ошибка сервера",
					Message = "Произошла ошибка на сервере",
				});
			}
		}


		[HttpGet("{uuid}")]
		[SwaggerResponse(StatusCodes.Status200OK, "Пользователь найден", typeof(UsersResponseDto))]
		[ResponseExample(StatusCodes.Status200OK, typeof(UsersResponseDto))]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Пользователь не найден", typeof(ApiError))]
		[ApiErrorExample(
			StatusCodes.Status404NotFound,
			"1.2.3",
			"Пользователь не найден",
			"Пользователь с указанным UUID не найден",
			nameof(uuid)
		)]
		[SwaggerOperation(Summary = "Получить пользователя по UUID")]
		public async Task<ActionResult<UsersResponseDto>> GetUser(Guid uuid)
		{
			string functionName = ControllerContext.ActionDescriptor.ActionName;
			try
			{
				using Activity? activity = _activitySource.StartAndLog(_logger, this);
				_logger.LogInformation("{Function}: GetUser uuid={Uuid}", functionName, uuid);

				UsersResponseDto? user = await _context.Users
					.Include(u => u.Roles)
					.Where(u => u.Uuid == uuid)
					.Select(u => new UsersResponseDto
					{
						Uuid = u.Uuid,
						Login = u.Login,
						Email = u.Email,
						FirstName = u.FirstName,
						LastName = u.LastName,
						Patronymic = u.Patronymic,
						TokenVersion = u.TokenVersion,
						RolesUuid = u.Roles != null ? u.Roles.Select(r => r.Uuid).ToArray() : null,
					})
					.FirstOrDefaultAsync();

				if (user == null)
				{
					_logger.LogInformation("{Function}: пользователь не найден uuid={Uuid}", functionName, uuid);
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
			catch (Exception ex)
			{
				_logger.LogError(ex, "{Function}: неожиданная ошибка при получении пользователя uuid={Uuid}", functionName, uuid);
				return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
				{
					StatusCode = "1.0.0",
					Title = "Внутренняя ошибка сервера",
					Message = "Произошла ошибка на сервере",
				});
			}
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
						StatusCode = "0.1.0",
						Title = "Неверный запрос",
						Message = "Тело запроса не может быть пустым",
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
					user.PasswordHash = HashingPassword.ComputeHash(request.Password);
					user.TokenVersion++;
				}
			}

			if (request.FirstName != null && request.FirstName == string.Empty)
			{
				return BadRequest(
					new ApiError
					{
						StatusCode = "0.2.0",
						Title = "Неверный запрос",
						Message = $"{nameof(UsersUpdateDto.FirstName)} не может быть пустым",
						Field = nameof(UsersUpdateDto.FirstName),
					}
				);
			}

			if (request.LastName != null && request.LastName == string.Empty)
			{
				return BadRequest(
					new ApiError
					{
						StatusCode = "0.2.0",
						Title = "Неверный запрос",
						Message = $"{nameof(UsersUpdateDto.LastName)} не может быть пустым",
						Field = nameof(UsersUpdateDto.LastName),
					}
				);
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

			if (request.RolesUuid != null)
			{
				Guid[] roleUuids = request.RolesUuid.Distinct().ToArray();
				List<Roles> foundRoles = await _context.Roles
					.Where(r => roleUuids.Contains(r.Uuid))
					.ToListAsync();

				if (foundRoles.Count != roleUuids.Length)
				{
					Guid[] missing = roleUuids.Except(foundRoles.Select(r => r.Uuid)).ToArray();
					return BadRequest(
						new ApiError
						{
							StatusCode = "0.2.3",
							Title = "Неверный запрос",
							Message = "Одна или несколько ролей не найдены",
							Field = nameof(request.RolesUuid),
							Details = string.Join(", ", missing),
						}
					);
				}

				user.Roles = foundRoles;
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
