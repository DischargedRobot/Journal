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
	[Route("api/auth-service/v1/[controller]")]
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
			[FromQuery, SwaggerParameter("Порядок сортировки по имени")]
			SortOrder sortOrder = SortOrder.Ascending
		)
		{
			string functionName = ControllerContext.ActionDescriptor.ActionName;
			try
			{
				using Activity? activity = _activitySource.StartAndLog(_logger, this);
				_logger.LogInformation("{Function} вызвано: size={Size}, offset={Offset}, filterName={FilterName}, sortOrder={SortOrder}", functionName, size, offset, filterName, sortOrder);
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
					.Roles.Where(r => filterName == null || r.Name.Contains(filterName))
					.AsNoTracking();

				Task<int> totalRecord = baseQuery.CountAsync();

				List<RolesResponseDto> items = await baseQuery
					.SortByKey(r => r.RoleName, sortOrder)
					.TakeWithOffset(offset, size)
					.Select(r => new RolesResponseDto
					{
						Uuid = r.Uuid,
						Name = r.Name,
						RoleName = r.RoleName,
						Rights =
							r.RoleRights != null
								? r.RoleRights.Select(rr => new RoleRightsResponseDto
								{
									Uuid = rr.Uuid,
									Name = rr.Name,
								})
								: null,
					})
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
		public async Task<IActionResult> GetRole(Guid uuid)
		{
			string functionName = ControllerContext.ActionDescriptor.ActionName;
			try
			{
				using Activity? activity = _activitySource.StartAndLog(_logger, this);
				_logger.LogInformation("{Function}: вызвано для uuid={Uuid}", functionName, uuid);

				var role = await _context
						.Roles.Where(r => r.Uuid == uuid)
						.Select(r => new
						{
							r.Uuid,
							r.Name,
							r.RoleName,
							Rights = r.RoleRights != null
								? r.RoleRights.Select(rr => new { rr.Uuid, rr.Name })
								: null,
						})
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
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "Неверный формат данных", "BODY")]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.0", "Неверный запрос", "Name и RoleName обязательны", "Name, RoleName")]
		[ApiErrorExample(StatusCodes.Status409Conflict, "1.2.1", "Конфликт", "Роль с таким RoleName уже существует", nameof(RolesCreateDto.RoleName))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.3", "Неверный запрос", "Некоторые права не найдены", nameof(RolesCreateDto.RightsUuids))]
		[SwaggerOperation(Summary = "Создать новую роль")]
		public async Task<IActionResult> CreateRole([FromBody] RolesCreateDto createDto)
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
							"0.2.1",
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
							"Name обязательна",
							nameof(createDto.Name))
					);
				}

				if (string.IsNullOrWhiteSpace(createDto.RoleName))
				{
					_logger.LogWarning("{Function}: RoleName отсутствует", functionName);
					return BadRequest(
						new ApiError(
							"0.2.0",
							"Неверный запрос",
							"RoleName обязательна",
							nameof(createDto.RoleName))
					);
				}

				bool exists = await _context.Roles.AnyAsync(r => r.RoleName == createDto.RoleName);
				if (exists)
				{
					_logger.LogWarning("{Function}: роль с RoleName={RoleName} уже существует", functionName, createDto.RoleName);
					return Conflict(
						new ApiError(
							"1.2.1",
							"Конфликт",
							"Роль с таким RoleName уже существует",
							nameof(RolesCreateDto.RoleName)
						)
					);
				}

				_logger.LogInformation("{Function}: добавление роли", functionName);
				Roles role = new()
				{
					Uuid = Guid.NewGuid(),
					Name = createDto.Name.Trim(),
					RoleName = createDto.RoleName.Trim(),
				};

				// Операция должна быть атомарной: либо роль и привязки прав создаются вместе, либо ничего не сохраняется.
				await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
				try
				{
					List<Guid> requested = createDto.RightsUuids?.Distinct().ToList() ?? new List<Guid>();

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
						foreach (RoleRights rr in rights)
						{
							rr.Role = role;
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
						RoleName = role.RoleName,
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
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.0", "Неверный запрос", "Неверный формат данных", "BODY")]
		[ApiErrorExample(StatusCodes.Status404NotFound, "1.2.3", "Роль не найдена", "Роль с указанным UUID не найдена", nameof(uuid))]
		[ApiErrorExample(StatusCodes.Status409Conflict, "1.2.1", "Конфликт", "RoleName уже используется", nameof(RolesUpdateDto.RoleName))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "1.2.3", "Неверный запрос", "Некоторые права не найдены", nameof(RolesUpdateDto.RightsUuids))]
		[ApiErrorExample(StatusCodes.Status409Conflict, "1.2.1", "Конфликт", "Одна или несколько прав уже привязаны к другой роли", nameof(RolesUpdateDto.RightsUuids))]
		[SwaggerOperation(Summary = "Частично обновить роль по UUID")]
		public async Task<IActionResult> UpdateRole(Guid uuid, [FromBody] RolesUpdateDto? request)
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
							"0.2.1",
							"Неверный запрос",
							"Неверный формат данных",
							"BODY"
						)
					);
				}

				Roles? role = await _context.Roles.FirstOrDefaultAsync(r => r.Uuid == uuid);
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

				if (!string.IsNullOrWhiteSpace(request.RoleName) && request.RoleName != role.RoleName)
				{
					bool exists = await _context.Roles.AnyAsync(r =>
						r.RoleName == request.RoleName && r.Uuid != uuid
					);
					if (exists)
					{
						_logger.LogWarning("{Function}: попытка установить RoleName={RoleName}, уже занято", functionName, request.RoleName);
						return Conflict(
							new ApiError(
								"1.2.1",
								"Конфликт",
								"RoleName уже используется",
								nameof(RolesUpdateDto.RoleName)
							)
						);
					}
					role.RoleName = request.RoleName.Trim();
				}

				if (request.Name != null)
				{
					_logger.LogInformation("{Function}: попытка установить Name={Name}", functionName, request.Name);

					role.Name = request.Name;
				}

				if (request.RightsUuids != null)
				{
					List<Guid> requested = request.RightsUuids.Distinct().ToList();

					List<RoleRights> rights = await _context
						.RoleRights.Where(rr => requested.Contains(rr.Uuid))
						.ToListAsync();

					List<Guid> missing = requested.Except(rights.Select(r => r.Uuid)).ToList();
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
					// Убираем привязку прав, которые не были запрошены, но сейчас привязаны к роли
					List<RoleRights> current = await _context
						.RoleRights.Where(rr => rr.RoleId == role.RoleId)
						.ToListAsync();

					List<RoleRights> toRemove = current
						.Where(cr => !requested.Contains(cr.Uuid))
						.ToList();
					foreach (RoleRights r in toRemove)
					{
						r.RoleId = null;

					}

					if (rights.Any(rr => rr.RoleId != null && rr.RoleId != role.RoleId))
					{
						_logger.LogWarning("{Function}: попытка привязать права, принадлежащие другим ролям", functionName);
						return Conflict(
							new ApiError(
								"1.2.1",
								"Конфликт",
								"Одна или несколько прав уже привязаны к другой роли",
								nameof(request.RightsUuids)
							)
						);
					}

					List<RoleRights> toAdd = rights
						.Where(rr => rr.RoleId == null || rr.RoleId == role.RoleId)
						.ToList();
					foreach (RoleRights r in toAdd)
					{
						r.RoleId = role.RoleId;
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
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.0", "Неверный запрос", "Неверный формат данных", "BODY")]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.0", "Неверный запрос", "Name обязательна", "Name")]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.0", "Неверный запрос", "RoleName обязательна", "RoleName")]
		[ApiErrorExample(StatusCodes.Status404NotFound, "1.2.3", "Роль не найдена", "Роль с указанным UUID не найдена", nameof(uuid))]
		[ApiErrorExample(StatusCodes.Status409Conflict, "1.2.1", "Конфликт", "RoleName уже используется", nameof(RolesCreateDto.RoleName))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "Некоторые права не найдены", nameof(RolesCreateDto.RightsUuids))]
		[ApiErrorExample(StatusCodes.Status409Conflict, "1.2.1", "Конфликт", "Одна или несколько прав уже привязаны к другой роли", nameof(RolesCreateDto.RightsUuids))]
		[SwaggerOperation(Summary = "Полная замена роли по UUID")]
		public async Task<IActionResult> ReplaceRole(
			Guid uuid,
			[FromBody] RolesCreateDto replaceDto
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
						"0.2.0",
						"Неверный запрос",
						"Неверный формат данных",
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

				if (string.IsNullOrWhiteSpace(replaceDto.RoleName))
				{
					_logger.LogWarning("{Function}: RoleName отсутствует", functionName);
					return BadRequest(
						new ApiError(
							"0.2.0",
							"Неверный запрос",
							"RoleName обязательна",
							nameof(replaceDto.RoleName)
						)
					);
				}

				Roles? role = await _context.Roles.FirstOrDefaultAsync(r => r.Uuid == uuid);
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
					r.RoleName == replaceDto.RoleName && r.Uuid != uuid
				);
				if (exists)
				{
					_logger.LogWarning("{Function}: попытка установить RoleName={RoleName}, уже занято", functionName, replaceDto.RoleName);
					return Conflict(
						new ApiError(
							"1.2.1",
							"Конфликт",
							"RoleName уже используется",
							nameof(RolesCreateDto.RoleName)
						)
					);
				}

				role.Name = replaceDto.Name.Trim();
				role.RoleName = replaceDto.RoleName.Trim();

				List<Guid> requested = replaceDto.RightsUuids?.Distinct().ToList() ?? new List<Guid>();

				if (requested.Count > 0)
				{
					List<RoleRights> rights = await _context
						.RoleRights.Where(rr => requested.Contains(rr.Uuid))
						.ToListAsync();

					List<Guid> missing = requested.Except(rights.Select(r => r.Uuid)).ToList();
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

					// Убираем привязку прав, которые не были запрошены, но сейчас привязаны к роли
					List<RoleRights> current = await _context.RoleRights
						.Where(rr => rr.RoleId == role.RoleId)
						.ToListAsync();
					foreach (RoleRights r in current.Where(cr => !requested.Contains(cr.Uuid)))
					{
						r.RoleId = null;
					}

					// Привязываем запрошенные права к роли
					foreach (RoleRights r in rights)
					{
						r.RoleId = role.RoleId;
					}
				}
				else
				{
					List<RoleRights> currentAll = await _context
						.RoleRights.Where(rr => rr.RoleId == role.RoleId)
						.ToListAsync();
					foreach (RoleRights r in currentAll)
					{
						r.RoleId = null;
					}
				}
				await _context.SaveChangesAsync();

				_logger.LogInformation("{Function}: роль uuid={Uuid} заменена", functionName, uuid);
				return Ok(
					new RolesResponseDto
					{
						Uuid = role.Uuid,
						Name = role.Name,
						RoleName = role.RoleName,
						Rights = role.RoleRights?.Select(rr => new RoleRightsResponseDto
						{
							Uuid = rr.Uuid,
							Name = rr.Name
						}).ToList()
					}
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
		public async Task<IActionResult> DeleteRole(Guid uuid)
		{
			string functionName = ControllerContext.ActionDescriptor.ActionName;
			try
			{
				using Activity? activity = _activitySource.StartAndLog(_logger, this);
				_logger.LogInformation("{Function}: вызвано для uuid={Uuid}", functionName, uuid);

				Roles? role = await _context.Roles.FirstOrDefaultAsync(r => r.Uuid == uuid);
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
	}
}
