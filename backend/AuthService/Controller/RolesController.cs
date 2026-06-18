using AuthService.Enums;
using AuthService.Errors;
using AuthService.Model;
using AuthService.ResponseExample;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Annotations;
using System.Diagnostics;
using AuthService.Lib.Utils;
using Microsoft.EntityFrameworkCore.Storage;

namespace AuthService.Controller
{
	[ApiController]
	[Route("auth-service/v1/[controller]")]
	[Produces("application/json")]
	public class RolesController : ControllerBase
	{
		private readonly ILogger<RolesController> _logger;
		private readonly AuthServiceContext _context;
		private readonly ActivitySource _activitySource;

		public RolesController(AuthServiceContext context, ILogger<RolesController> logger, ActivitySource activitySource)
		{
			_context = context;
			_logger = logger;
			_activitySource = activitySource;
		}

		[HttpGet]
		[SwaggerResponse(StatusCodes.Status200OK, "Роли найдены", typeof(PagedResult<RolesResponseDto>))]
		[ResponseExample(StatusCodes.Status200OK, typeof(PagedResult<RolesResponseDto>))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "Параметр offset не может быть отрицательным", nameof(offset))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "Параметр size не может быть отрицательным", nameof(size))]
		[ApiErrorExample(StatusCodes.Status404NotFound, "1.0.3", "Роли не найдены", "В системе не найдено ни одной роли", "")]
		[ApiErrorExample(StatusCodes.Status404NotFound, "1.1.3", "Роли не найдены", "В системе не найдено ни одной роли для указанных параметров запроса", "BODY")]
		[SwaggerOperation(Summary = "Получить список ролей")]
		public async Task<ActionResult<PagedResult<RolesResponseDto>>> GetRoles(
			[FromQuery, SwaggerParameter("Количество записей")]
			int size = 100,
			[FromQuery, SwaggerParameter("Сдвиг от начала")]
			int offset = 0,
			[FromQuery, SwaggerParameter("Фильтр по имени")]
			string? filterName = null,
			[FromQuery, SwaggerParameter("Базовая роль")]
			bool? isBase = null,
			[FromQuery, SwaggerParameter("Порядок сортировки по имени")]
			SortOrder sortOrder = SortOrder.Ascending
		)
		{
			string functionName = ControllerContext.ActionDescriptor.ActionName;
			try
			{
				using Activity? activity = _activitySource.StartAndLog(_logger, this);
				_logger.LogInformation("{Function} вызвано: size={Size}, offset={Offset}, filterName={FilterName}, isBase={IsBase}, sortOrder={SortOrder}", functionName, size, offset, filterName, isBase, sortOrder);
				if (offset < 0)
				{
					_logger.LogWarning("{Function}: неверный offset {Offset}", functionName, offset);
					return BadRequest(
						new ApiError(
							"0.2.1",
							"Неверный запрос",
							"Параметр offset не может быть отрицательным",
							nameof(offset)
						)
					);
				}
				if (size < 0)
				{
					_logger.LogWarning("{Function}: неверный size {Size}", functionName, size);
					return BadRequest(
						new ApiError(
							"0.2.1",
							"Неверный запрос",
							"Параметр size не может быть отрицательным",
							nameof(size)
						)
					);
				}

				IQueryable<Roles> baseQuery = _context
					.Roles.Where(r =>
						(filterName == null || r.Name.Contains(filterName)) &&
						(isBase == null || r.IsBase == isBase))
					.AsNoTracking();

				Task<int> totalRecord = baseQuery.CountAsync();

				List<RolesResponseDto> items = await baseQuery
					.SortByKey(r => r.Name, sortOrder)
					.TakeWithOffset(offset, size)
					.Select(r => new RolesResponseDto(r))
					.ToListAsync();

				int total = await totalRecord;

				_logger.LogInformation("{Function}: найдено записей = {Total}", functionName, total);

				if (total == 0)
				{
					_logger.LogInformation("{Function}: роли не найдены (total=0)", functionName);
					return NotFound(
						new ApiError(
							"1.0.3",
							"Роли не найдены",
							"В системе не найдено ни одной роли",
							string.Empty
						)
					);
				}

				if (items.Count == 0)
				{
					_logger.LogInformation("{Function}: нет ролей по фильтру (total={Total}, offset={Offset})", functionName, total, offset);
					return NotFound(
						new ApiError(
							"1.1.3",
							"Роли не найдены",
							"В системе не найдено ни одной роли для указанных параметров запроса",
							"BODY"
						)
					);
				}

				PagedResult<RolesResponseDto> result = new(
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
				_logger.LogError(ex, "{Function}: неожиданная ошибка при обработке запроса", functionName);
				return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
				{
					StatusCode = "1.0.0",
					Title = "Внутренняя ошибка сервера",
					Message = "Произошла ошибка на сервере",
				});
			}
		}

		[HttpGet("{uuid}")]
		[SwaggerResponse(StatusCodes.Status200OK, "Роль найдена", typeof(RolesResponseDto))]
		[ResponseExample(StatusCodes.Status200OK, typeof(RolesResponseDto))]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Роль не найдена", typeof(ApiError))]
		[ApiErrorExample(StatusCodes.Status404NotFound, "1.2.3", "Роль не найдена", "Роль с указанным UUID не найдена", nameof(uuid))]
		[SwaggerOperation(Summary = "Получить роль по UUID")]
		public async Task<IActionResult> GetRole(
			[SwaggerParameter("UUID роли")]
			Guid uuid)
		{
			string functionName = ControllerContext.ActionDescriptor.ActionName;
			try
			{
				using Activity? activity = _activitySource.StartAndLog(_logger, this);
				_logger.LogInformation("{Function}: вызвано для uuid={Uuid}", functionName, uuid);

				RolesResponseDto? role = await _context.Roles
					.Where(r => r.Uuid == uuid)
					.Select(r => new RolesResponseDto(r))
					.FirstOrDefaultAsync();

				if (role == null)
				{
					_logger.LogInformation("{Function}: роль с uuid={Uuid} не найдена", functionName, uuid);
					return NotFound(
						new ApiError(
							"1.2.3",
							"Роль не найдена",
							"Роль с указанным UUID не найдена",
							nameof(uuid)
						)
					);
				}

				_logger.LogInformation("{Function}: возвращена роль uuid={Uuid}", functionName, uuid);
				return Ok(role);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "{Function}: неожиданная ошибка при получении роли", functionName);
				return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
				{
					StatusCode = "1.0.0",
					Title = "Внутренняя ошибка сервера",
					Message = "Произошла ошибка на сервере",
				});
			}
		}

		[HttpPost]
		[SwaggerResponse(StatusCodes.Status201Created, "Роль создана", typeof(RolesResponseDto))]
		[ResponseExample(StatusCodes.Status201Created, typeof(RolesResponseDto))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Некорректные данные", typeof(ApiError))]
		[SwaggerResponse(StatusCodes.Status409Conflict, "Роль уже существует", typeof(ApiError))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.1.0", "Неверный запрос", "Тело запроса не может быть пустым", "BODY")]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.0", "Неверный запрос", "Поле Name обязательно для создания роли", "Name")]
		[ApiErrorExample(StatusCodes.Status409Conflict, "1.2.1", "Конфликт", "Роль с таким Name уже существует", nameof(RolesCreateDto.Name))]
		[ApiErrorExample(StatusCodes.Status409Conflict, "1.2.2", "Конфликт", "Базовая роль уже существует", nameof(RolesCreateDto.IsBase))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.3", "Неверный запрос", "Некоторые права не найдены", nameof(RolesCreateDto.RightsUuids))]
		[SwaggerOperation(Summary = "Создать новую роль")]
		public async Task<IActionResult> CreateRole(
			[FromBody, SwaggerParameter("Тело запроса: данные для создания роли")]
			RolesCreateDto? createDto)
		{
			string functionName = ControllerContext.ActionDescriptor.ActionName;
			try
			{
				using Activity? activity = _activitySource.StartAndLog(_logger, this);
				_logger.LogInformation("{Function}: вызвано", functionName);

				if (createDto == null)
				{
					_logger.LogWarning("{Function}: пустой createDto", functionName);
					return BadRequest(
						new ApiError(
							"0.1.0",
							"Неверный запрос",
							"Неверный формат данных",
							"BODY")
					);
				}

				if (string.IsNullOrWhiteSpace(createDto.Name))
				{
					_logger.LogWarning("{Function}: Name отсутствует", functionName);
					return BadRequest(
						new ApiError(
							"0.2.0",
							"Неверный запрос",
							"Поле Name обязательно для создания роли",
							nameof(createDto.Name))
					);
				}

				if (createDto.RightsUuids == null)
				{
					_logger.LogWarning("{Function}: RightsUuids отсутствует", functionName);
					return BadRequest(
						new ApiError(
							"0.2.0",
							"Неверный запрос",
							"Поле RightsUuids обязательно для создания роли (может быть пустым массивом)",
							nameof(createDto.RightsUuids))
					);
				}

				if (createDto.RoleTypesUuids == null || !createDto.RoleTypesUuids.Any())
				{
					_logger.LogWarning("{Function}: RoleTypesUuids отсутствует или пустой", functionName);
					return BadRequest(
						new ApiError(
							"0.2.0",
							"Неверный запрос",
							"Поле RoleTypesUuids обязательно и должно содержать хотя бы один UUID типа роли",
							nameof(createDto.RoleTypesUuids))
					);
				}

				bool exists = await _context.Roles.AnyAsync(r => r.Name == createDto.Name);
				if (exists)
				{
					_logger.LogWarning("{Function}: роль с Name={Name} уже существует", functionName, createDto.Name);
					return Conflict(
						new ApiError(
							"1.2.1",
							"Конфликт",
							"Роль с таким Name уже существует",
							nameof(RolesCreateDto.Name)
						)
					);
				}

				if (createDto.IsBase && await BaseRoleExistsAsync())
				{
					_logger.LogWarning("{Function}: попытка создать вторую базовую роль", functionName);
					return Conflict(
						new ApiError(
							"1.2.1",
							"Конфликт",
							"Базовая роль уже существует",
							nameof(RolesCreateDto.IsBase)
						)
					);
				}

				_logger.LogInformation("{Function}: добавление роли", functionName);
				Roles role = new()
				{
					Uuid = Guid.NewGuid(),
					Name = createDto.Name.Trim(),
					IsBase = createDto.IsBase,
				};

				// Операция должна быть атомарной: либо роль и привязки прав создаются вместе, либо ничего не сохраняется.
				await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
				try
				{
					List<Guid> requested = createDto.RightsUuids?.Distinct().ToList() ?? [];

					List<Guid>? requestedRoleTypes = createDto.RoleTypesUuids?.Distinct().ToList();

					List<RoleRights> rights = [];
					if (requested.Count > 0)
					{
						rights = await _context.RoleRights
							.Where(rr => requested.Contains(rr.Uuid))
							.ToListAsync();

						// Вычисляем UUID прав, которые были запрошены, но не найдены в базе данных
						List<Guid> missing = requested.Except(rights.Select(r => r.Uuid)).ToList();
						if (missing.Count > 0)
						{
							_logger.LogWarning("{Function}: некоторые права не найдены: {Missing}", functionName, string.Join(", ", missing));
							await transaction.RollbackAsync();
							return BadRequest(
								new ApiError
								{
									StatusCode = "0.2.3",
									Title = "Неверный запрос",
									Message = "Некоторые права не найдены",
									Field = nameof(createDto.RightsUuids),
									Details = string.Join(", ", missing),
								}
							);
						}
					}

					// Добавляем роль и сохраняем в рамках транзакции, чтобы получить RoleId
					_context.Roles.Add(role);

					if (rights.Count > 0)
					{
						role.RoleRights ??= [];
						foreach (RoleRights rr in rights)
						{
							if (!role.RoleRights.Any(r => r.Uuid == rr.Uuid))
								role.RoleRights.Add(rr);
						}

					}

					if (requestedRoleTypes == null)
					{
						_logger.LogWarning("{Function}: RoleTypesUuids отсутствует", functionName);
						await transaction.RollbackAsync();
						return BadRequest(
							new ApiError(
								"0.2.0",
								"Неверный запрос",
								"Поле RoleTypesUuids обязательно для создания роли (может быть пустым массивом)",
								nameof(createDto.RoleTypesUuids))
						);
					}
					// Обработка типов ролей
					if (requestedRoleTypes.Count > 0)
					{
						List<RolesTypes> roleTypes = await _context.RolesTypes
							.Where(rt => requestedRoleTypes.Contains(rt.Uuid))
							.ToListAsync();

						List<Guid> missingRoleTypes = requestedRoleTypes.Except(roleTypes.Select(r => r.Uuid)).ToList();
						if (missingRoleTypes.Count > 0)
						{
							_logger.LogWarning("{Function}: некоторые типы ролей не найдены: {Missing}", functionName, string.Join(", ", missingRoleTypes));
							await transaction.RollbackAsync();
							return BadRequest(
								new ApiError
								{
									StatusCode = "0.2.4",
									Title = "Неверный запрос",
									Message = "Некоторые типы ролей не найдены",
									Field = nameof(createDto.RoleTypesUuids),
									Details = string.Join(", ", missingRoleTypes),
								}
							);
						}

						role.RoleType ??= new List<RolesTypes>();
						foreach (RolesTypes rt in roleTypes)
						{
							if (!role.RoleType.Any(r => r.Uuid == rt.Uuid))
								role.RoleType.Add(rt);
						}
					}
					await _context.SaveChangesAsync();
					await transaction.CommitAsync();
				}
				catch (Exception ex)
				{
					await transaction.RollbackAsync();
					_logger.LogError(ex, "{Function}: ошибка при создании роли в транзакции", functionName);
					return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
					{
						StatusCode = "1.0.0",
						Title = "Внутренняя ошибка сервера",
						Message = "Произошла ошибка при создании роли",
					});
				}

				_logger.LogInformation("{Function}: создана роль uuid={Uuid}", functionName, role.Uuid);
				return CreatedAtAction(
					nameof(GetRole),
					new { uuid = role.Uuid },
					new RolesResponseDto
					{
						Uuid = role.Uuid,
						Name = role.Name,
						IsBase = role.IsBase,
					}
				);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "{Function}: неожиданная ошибка при создании роли", functionName);
				return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
				{
					StatusCode = "1.0.0",
					Title = "Внутренняя ошибка сервера",
					Message = "Произошла ошибка на сервере",
				});
			}
		}

		[HttpPatch("{uuid}")]
		[SwaggerResponse(StatusCodes.Status200OK, "Роль обновлена", typeof(RolesResponseDto))]
		[ResponseExample(StatusCodes.Status200OK, typeof(RolesResponseDto))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Роль не найдена", typeof(ApiError))]
		[SwaggerResponse(StatusCodes.Status409Conflict, "Имя роли уже используется", typeof(ApiError))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.1.0", "Неверный запрос", "Тело запроса не может быть пустым", "BODY")]
		[ApiErrorExample(StatusCodes.Status404NotFound, "1.2.3", "Роль не найдена", "Роль с указанным UUID не найдена", nameof(uuid))]
		[ApiErrorExample(StatusCodes.Status409Conflict, "1.2.1", "Конфликт", "Name уже используется", nameof(RolesUpdateDto.Name))]
		[ApiErrorExample(StatusCodes.Status409Conflict, "1.2.1", "Конфликт", "Базовая роль уже существует", nameof(RolesUpdateDto.IsBase))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "Некоторые права не найдены", nameof(RolesUpdateDto.RightsUuids))]
		[SwaggerOperation(Summary = "Частично обновить роль по UUID")]
		public async Task<IActionResult> UpdateRole(
			[SwaggerParameter("UUID роли")]
			Guid uuid,
			[FromBody, SwaggerParameter("Тело запроса: частичные данные для обновления роли")]
			 RolesUpdateDto? request)
		{
			string functionName = ControllerContext.ActionDescriptor.ActionName;
			try
			{
				using Activity? activity = _activitySource.StartAndLog(_logger, this);
				_logger.LogInformation("{Function}: вызвано для uuid={Uuid}", functionName, uuid);

				if (request == null)
				{
					_logger.LogWarning("{Function}: пустой request", functionName);
					return BadRequest(
						new ApiError(
							"0.1.0",
							"Неверный запрос",
							"Тело запроса не может быть пустым",
							"BODY"
						)
					);
				}

				Roles? role = await _context.Roles.Include(r => r.RoleRights).Include(r => r.RoleType).FirstOrDefaultAsync(r => r.Uuid == uuid);
				if (role == null)
				{
					_logger.LogInformation("{Function}: роль uuid={Uuid} не найдена", functionName, uuid);
					return NotFound(
						new ApiError(
							"1.2.3",
							"Роль не найдена",
							"Роль с указанным UUID не найдена",
							nameof(uuid)
						)
					);
				}

				if (!string.IsNullOrWhiteSpace(request.Name) && request.Name != role.Name)
				{
					bool exists = await _context.Roles.AnyAsync(r =>
						r.Name == request.Name && r.Uuid != uuid
					);
					if (exists)
					{
						_logger.LogWarning("{Function}: попытка установить Name={Name}, уже занято", functionName, request.Name);
						return Conflict(
							new ApiError(
								"1.2.1",
								"Конфликт",
								"Name уже используется",
								nameof(RolesUpdateDto.Name)
							)
						);
					}
					role.Name = request.Name.Trim();
				}

				if (request.IsBase == true && !role.IsBase && await BaseRoleExistsAsync(uuid))
				{
					_logger.LogWarning("{Function}: попытка назначить вторую базовую роль", functionName);
					return Conflict(
						new ApiError(
							"1.2.1",
							"Конфликт",
							"Базовая роль уже существует",
							nameof(RolesUpdateDto.IsBase)
						)
					);
				}

				if (request.IsBase.HasValue)
				{
					role.IsBase = request.IsBase.Value;
				}

				if (request.Name != null)
				{
					_logger.LogInformation("{Function}: попытка установить Name={Name}", functionName, request.Name);

					role.Name = request.Name;
				}

				if (request.RightsUuids != null)
				{
					List<Guid> requestedRights = request.RightsUuids.Distinct().ToList();

					List<RoleRights> rights = await _context.RoleRights
						.Where(rr => requestedRights.Contains(rr.Uuid))
						.ToListAsync();

					List<Guid> missing = requestedRights.Except(rights.Select(r => r.Uuid)).ToList();
					if (missing.Count > 0)
					{
						_logger.LogWarning("{Function}: некоторые права не найдены: {Missing}", functionName, string.Join(", ", missing));
						return BadRequest(
							new ApiError
							{
								StatusCode = "1.2.3",
								Title = "Неверный запрос",
								Message = "Некоторые права не найдены",
								Field = nameof(request.RightsUuids),
								Details = string.Join(", ", missing),
							}
						);
					}

					_logger.LogInformation("{Function}: изменяем права роли uuid={Uuid}", functionName, uuid);
					// Убираем привязку прав у роли (удаляем из коллекции role.RoleRights)
					role.RoleRights ??= [];
					List<RoleRights> current = role.RoleRights.ToList();

					List<RoleRights> toRemove = current
						.Where(cr => !requestedRights.Contains(cr.Uuid))
						.ToList();
					foreach (RoleRights r in toRemove)
					{
						role.RoleRights.Remove(r);
					}

					// Привязываем запрошенные права к роли (добавляем в коллекцию)
					foreach (RoleRights rr in rights)
					{
						if (!role.RoleRights.Any(x => x.Uuid == rr.Uuid))
							role.RoleRights.Add(rr);
					}

				}

				if (request.RoleTypesUuids != null)
				{
					List<Guid> requestedRoleTypes = request.RoleTypesUuids.Distinct().ToList();

					List<RolesTypes> roleTypes = await _context.RolesTypes
						.Where(rt => requestedRoleTypes.Contains(rt.Uuid))
						.ToListAsync();

					List<Guid> missingRoleTypes = requestedRoleTypes.Except(roleTypes.Select(r => r.Uuid)).ToList();
					if (missingRoleTypes.Count > 0)
					{
						_logger.LogWarning("{Function}: некоторые типы ролей не найдены: {Missing}", functionName, string.Join(", ", missingRoleTypes));
						return BadRequest(
							new ApiError
							{
								StatusCode = "0.2.4",
								Title = "Неверный запрос",
								Message = "Некоторые типы ролей не найдены",
								Field = nameof(request.RoleTypesUuids),
								Details = string.Join(", ", missingRoleTypes),
							}
						);
					}

					_logger.LogInformation("{Function}: изменяем типы роли uuid={Uuid}", functionName, uuid);

					List<RolesTypes> currentTypes = role.RoleType.ToList();

					List<RolesTypes> toRemoveTypes = currentTypes
						.Where(cr => !requestedRoleTypes.Contains(cr.Uuid))
						.ToList();
					foreach (RolesTypes r in toRemoveTypes)
					{
						role.RoleType.Remove(r);
					}

					foreach (RolesTypes rt in roleTypes)
					{
						if (!role.RoleType.Any(x => x.Uuid == rt.Uuid))
							role.RoleType.Add(rt);
					}
				}

				await _context.SaveChangesAsync();

				_logger.LogInformation("{Function}: роль uuid={Uuid} обновлена", functionName, uuid);
				return Ok(new RolesResponseDto(role));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "{Function}: неожиданная ошибка при обновлении роли", functionName);
				return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
				{
					StatusCode = "1.0.0",
					Title = "Внутренняя ошибка сервера",
					Message = "Произошла ошибка на сервере",
				});
			}
		}

		[HttpPut("{uuid}")]
		[SwaggerResponse(StatusCodes.Status200OK, "Роль заменена", typeof(RolesResponseDto))]
		[ResponseExample(StatusCodes.Status200OK, typeof(RolesResponseDto))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Роль не найдена", typeof(ApiError))]
		[SwaggerResponse(StatusCodes.Status409Conflict, "Конфликт: имя роли уже используется", typeof(ApiError))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.1.0", "Неверный запрос", "Неверный формат данных", "BODY")]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.0", "Неверный запрос", "Name обязательна", "Name")]
		[ApiErrorExample(StatusCodes.Status404NotFound, "1.2.3", "Роль не найдена", "Роль с указанным UUID не найдена", nameof(uuid))]
		[ApiErrorExample(StatusCodes.Status409Conflict, "1.2.1", "Конфликт", "Name уже используется", nameof(RolesCreateDto.Name))]
		[ApiErrorExample(StatusCodes.Status409Conflict, "1.2.1", "Конфликт", "Базовая роль уже существует", nameof(RolesCreateDto.IsBase))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "Некоторые права не найдены", nameof(RolesCreateDto.RightsUuids))]
		[ApiErrorExample(StatusCodes.Status409Conflict, "1.2.1", "Конфликт", "Одна или несколько прав уже привязаны к другой роли", nameof(RolesCreateDto.RightsUuids))]
		[SwaggerOperation(Summary = "Полная замена роли по UUID")]
		public async Task<IActionResult> ReplaceRole(
			[SwaggerParameter("UUID роли")]
			Guid uuid,
			[FromBody, SwaggerParameter("Тело запроса: данные для полной замены роли")]
			RolesCreateDto replaceDto
		)
		{
			string functionName = ControllerContext.ActionDescriptor.ActionName;
			try
			{
				using Activity? activity = _activitySource.StartAndLog(_logger, this);
				_logger.LogInformation("{Function}: вызвано для uuid={Uuid}", functionName, uuid);

				if (replaceDto == null)
				{
					_logger.LogWarning("{Function}: пустой replaceDto", functionName);
					return BadRequest(new ApiError(
						"0.1.0",
						"Неверный запрос",
						"Тело запроса не может быть пустым",
						"BODY"
						));
				}
				if (string.IsNullOrWhiteSpace(replaceDto.Name))
				{
					_logger.LogWarning("{Function}: Name отсутствует", functionName);
					return BadRequest(
						new ApiError(
							"0.2.0",
							"Неверный запрос",
							"Name обязательна",
							nameof(replaceDto.Name)
						)
					);
				}

				if (replaceDto.RoleTypesUuids == null || !replaceDto.RoleTypesUuids.Any())
				{
					_logger.LogWarning("{Function}: RoleTypesUuids отсутствует или пустой", functionName);
					return BadRequest(
						new ApiError(
							"0.2.0",
							"Неверный запрос",
							"Поле RoleTypesUuids обязательно и должно содержать хотя бы один UUID типа роли",
							nameof(replaceDto.RoleTypesUuids)
						)
					);
				}

				Roles? role = await _context.Roles.Include(r => r.RoleRights).Include(r => r.RoleType).FirstOrDefaultAsync(r => r.Uuid == uuid);
				if (role == null)
				{
					_logger.LogInformation("{Function}: роль uuid={Uuid} не найдена", functionName, uuid);
					return NotFound(
						new ApiError(
							"1.2.3",
							"Роль не найдена",
							"Роль с указанным UUID не найдена",
							nameof(uuid)
						)
					);
				}

				bool exists = await _context.Roles.AnyAsync(r =>
					r.Name == replaceDto.Name && r.Uuid != uuid
				);
				if (exists)
				{
					_logger.LogWarning("{Function}: попытка установить Name={Name}, уже занято", functionName, replaceDto.Name);
					return Conflict(
						new ApiError(
							"1.2.1",
							"Конфликт",
							"Name уже используется",
							nameof(RolesCreateDto.Name)
						)
					);
				}
				role.Name = replaceDto.Name.Trim();


				if (replaceDto.IsBase && !role.IsBase && await BaseRoleExistsAsync(uuid))
				{
					_logger.LogWarning("{Function}: попытка назначить вторую базовую роль", functionName);
					return Conflict(
						new ApiError(
							"1.2.1",
							"Конфликт",
							"Базовая роль уже существует",
							nameof(RolesCreateDto.IsBase)
						)
					);
				}
				role.IsBase = replaceDto.IsBase;

				List<Guid> requestedRights = replaceDto.RightsUuids?.Distinct().ToList() ?? new List<Guid>();
				List<Guid> requestedRoleTypes = replaceDto.RoleTypesUuids?.Distinct().ToList() ?? [];

				if (requestedRights.Count > 0)
				{
					List<RoleRights> rights = await _context.RoleRights
						.Where(rr => requestedRights.Contains(rr.Uuid))
						.ToListAsync();

					List<Guid> missing = requestedRights.Except(rights.Select(r => r.Uuid)).ToList();
					if (missing.Count > 0)
					{
						_logger.LogWarning(
							"{Function}: некоторые права не найдены: {Missing}",
							 functionName,
							 string.Join(", ", missing)
							 );
						return BadRequest(
							new ApiError
							{
								StatusCode = "0.2.1",
								Title = "Неверный запрос",
								Message = "Некоторые права не найдены",
								Field = nameof(replaceDto.RightsUuids),
								Details = string.Join(", ", missing),
							}
						);
					}

					// Убираем привязку прав у роли (удаляем из коллекции role.RoleRights)
					List<RoleRights> currentRoleRights = role.RoleRights?.ToList() ?? new List<RoleRights>();
					foreach (RoleRights r in currentRoleRights)
					{
						role.RoleRights?.Remove(r);
					}

					// Привязываем запрошенные права к роли (добавляем в коллекцию)
					foreach (RoleRights r in rights)
					{
						if (role.RoleRights?.Any(x => x.Uuid == r.Uuid) == false)
						{
							role.RoleRights?.Add(r);
						}
					}
				}
				else
				{
					role.RoleRights?.Clear();
				}

				// Обработка типов ролей для Replace
				List<RolesTypes> roleTypes = await _context.RolesTypes
					.Where(rt => requestedRoleTypes.Contains(rt.Uuid))
					.ToListAsync();

				List<Guid> missingTypes = requestedRoleTypes.Except(roleTypes.Select(r => r.Uuid)).ToList();
				if (missingTypes.Count > 0)
				{
					_logger.LogWarning(
						"{Function}: некоторые типы ролей не найдены: {Missing}",
						 functionName,
						 string.Join(", ", missingTypes)
						 );
					return BadRequest(
						new ApiError
						{
							StatusCode = "0.2.4",
							Title = "Неверный запрос",
							Message = "Некоторые типы ролей не найдены",
							Field = nameof(replaceDto.RoleTypesUuids),
							Details = string.Join(", ", missingTypes),
						}
					);
				}

				// Убираем привязку типов ролей
				List<RolesTypes> current = role.RoleType?.ToList() ?? new List<RolesTypes>();
				foreach (RolesTypes r in current)
				{
					role.RoleType?.Remove(r);
				}

				foreach (RolesTypes r in roleTypes)
				{
					if (role.RoleType?.Any(x => x.Uuid == r.Uuid) == false)
					{
						role.RoleType?.Add(r);
					}
				}

				await _context.SaveChangesAsync();

				_logger.LogInformation("{Function}: роль uuid={Uuid} заменена", functionName, uuid);
				return Ok(
					new RolesResponseDto(role)
				);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "{Function}: неожиданная ошибка при замене роли", functionName);
				return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
				{
					StatusCode = "1.0.0",
					Title = "Внутренняя ошибка сервера",
					Message = "Произошла ошибка на сервере",
				});
			}
		}

		[HttpDelete("{uuid}")]
		[SwaggerResponse(StatusCodes.Status204NoContent, "Роль удалена")]
		[SwaggerOperation(Summary = "Удалить роль по UUID")]
		[ApiErrorExample(StatusCodes.Status404NotFound, "1.2.3", "Роль не найдена", "Роль с указанным UUID не найдена", nameof(uuid))]
		public async Task<IActionResult> DeleteRole(
			[SwaggerParameter("UUID роли")]
			Guid uuid)
		{
			string functionName = ControllerContext.ActionDescriptor.ActionName;
			try
			{
				using Activity? activity = _activitySource.StartAndLog(_logger, this);
				_logger.LogInformation("{Function}: вызвано для uuid={Uuid}", functionName, uuid);

				Roles? role = await _context.Roles.Include(r => r.RoleRights).FirstOrDefaultAsync(r => r.Uuid == uuid);
				if (role == null)
				{
					_logger.LogInformation("{Function}: роль uuid={Uuid} не найдена", functionName, uuid);
					return NotFound(
						new ApiError(
							"1.3.3",
							"Роль не найдена",
							"Роль с указанным UUID не найдена",
							nameof(uuid)
						)
					);
				}

				_context.Roles.Remove(role);
				await _context.SaveChangesAsync();

				_logger.LogInformation("{Function}: роль uuid={Uuid} удалена", functionName, uuid);
				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "{Function}: неожиданная ошибка при удалении роли", functionName);
				return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
				{
					StatusCode = "1.0.0",
					Title = "Внутренняя ошибка сервера",
					Message = "Произошла ошибка на сервере",
				});
			}
		}

		private Task<bool> BaseRoleExistsAsync(Guid? excludeUuid = null)
		{
			IQueryable<Roles> query = _context.Roles.Where(r => r.IsBase);
			if (excludeUuid.HasValue)
			{
				query = query.Where(r => r.Uuid != excludeUuid.Value);
			}

			return query.AnyAsync();
		}
	}
}
