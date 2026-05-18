using AuthService.Enums;
using AuthService.Errors;
using AuthService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace AuthService.Controller
{
	[ApiController]
	[Route("api/auth-service/v1/[controller]")]
	[Produces("application/json")]
	public class RolesController : ControllerBase
	{
		private readonly AuthServiceContext _context;

		public RolesController(AuthServiceContext context)
		{
			_context = context;
		}

		[HttpGet]
		[SwaggerResponse(StatusCodes.Status200OK, "Роли найдены", typeof(PagedResult<RolesResponseDto>))]
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
			if (offset < 0)
			{
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

			if (total == 0)
			{
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

			return Ok(result);
		}

		[HttpGet("{uuid}")]
		[SwaggerResponse(StatusCodes.Status200OK, "Роль найдена", typeof(object))]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Роль не найдена", typeof(ApiError))]
		[ApiErrorExample(StatusCodes.Status404NotFound, "1.2.3", "Роль не найдена", "Роль с указанным UUID не найдена", nameof(uuid))]
		[SwaggerOperation(Summary = "Получить роль по UUID")]
		public async Task<IActionResult> GetRole(Guid uuid)
		{
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
				return NotFound(
					new ApiError(
						"1.2.3",
						"Роль не найдена",
						"Роль с указанным UUID не найдена",
						nameof(uuid)
					)
				);
			}

			return Ok(role);
		}

		[HttpPost]
		[SwaggerResponse(StatusCodes.Status201Created, "Роль создана", typeof(object))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Некорректные данные", typeof(ApiError))]
		[SwaggerResponse(StatusCodes.Status409Conflict,"Роль уже существует",typeof(ApiError))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "Неверный формат данных", "BODY")]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.0", "Неверный запрос", "Name и RoleName обязательны", "Name, RoleName")]
		[ApiErrorExample(StatusCodes.Status409Conflict, "1.1.1", "Конфликт", "Роль с таким RoleName уже существует", nameof(RolesCreateDto.RoleName))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "Некоторые права не найдены", nameof(RolesCreateDto.RightsUuids))]
		[SwaggerOperation(Summary = "Создать новую роль")]
		public async Task<IActionResult> CreateRole([FromBody] RolesCreateDto createDto)
		{
			if (createDto == null)
			{
				return BadRequest(
					new ApiError(
					"0.2.1", 
					"Неверный запрос",
					"Неверный формат данных", 
					"BODY")
				);
			}

			if (string.IsNullOrWhiteSpace(createDto.Name)
				|| string.IsNullOrWhiteSpace(createDto.RoleName))
			{
				return BadRequest(
					new ApiError(
						"0.2.0",
						"Неверный запрос",
						"Name и RoleName обязательны",
						"Name, RoleName")
				);
			}

			bool exists = await _context.Roles.AnyAsync(r => r.RoleName == createDto.RoleName);
			if (exists)
			{
				return Conflict(
					new ApiError(
						"1.1.1",
						"Конфликт",
						"Роль с таким RoleName уже существует",
						nameof(RolesCreateDto.RoleName)
					)
				);
			}

			Roles role = new()
			{
				Uuid = Guid.NewGuid(),
				Name = createDto.Name.Trim(),
				RoleName = createDto.RoleName.Trim(),
			};

			_context.Roles.Add(role);
			await _context.SaveChangesAsync();

			if (createDto.RightsUuids != null)
			{
				// Список запрошенных UUID прав
				List<Guid> requested = createDto.RightsUuids
					.Distinct()
					.ToList();

				// Загружаем объекты прав по UUID
				List<RoleRights> rights = await _context.RoleRights
					.Where(rr => requested.Contains(rr.Uuid))
					.ToListAsync();

				// Вычисляем UUID прав, которых нету в БД
				List<Guid> missing = requested.Except(rights.Select(r => r.Uuid)).ToList();
				if (missing.Count > 0)
				{
					return BadRequest(
						new ApiError
						{
							StatusCode = "0.2.1",
							Title = "Неверный запрос",
							Message = "Некоторые права не найдены",
							Field = nameof(createDto.RightsUuids),
							Details = string.Join(", ", missing),
						}
					);
				}

				// Привязываем права к созданной роли
				foreach (RoleRights rr in rights)
				{
					rr.RoleId = role.RoleId;
				}
				await _context.SaveChangesAsync();
			}

			return CreatedAtAction(
				nameof(GetRole),
				new { uuid = role.Uuid },
				new
				{
					role.Uuid,
					role.Name,
					role.RoleName,
				}
			);
		}

		[HttpPatch("{uuid}")]
		[SwaggerResponse(StatusCodes.Status200OK, "Роль обновлена", typeof(object))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Роль не найдена", typeof(ApiError))]
		[SwaggerResponse(StatusCodes.Status409Conflict, "Имя роли уже используется", typeof(ApiError))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "Неверный формат данных", "BODY")]
		[ApiErrorExample(StatusCodes.Status404NotFound, "1.2.3", "Роль не найдена", "Роль с указанным UUID не найдена", nameof(uuid))]
		[ApiErrorExample(StatusCodes.Status409Conflict, "1.2.1", "Конфликт", "RoleName уже используется", nameof(RolesUpdateDto.RoleName))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "Некоторые права не найдены", nameof(RolesUpdateDto.RightsUuids))]
		[ApiErrorExample(StatusCodes.Status409Conflict, "1.2.1", "Конфликт", "Одна или несколько прав уже привязаны к другой роли", nameof(RolesUpdateDto.RightsUuids))]
		[SwaggerOperation(Summary = "Частично обновить роль по UUID")]
		public async Task<IActionResult> UpdateRole(Guid uuid, [FromBody] RolesUpdateDto? request)
		{
			if (request == null)
				return BadRequest(
					new ApiError(
						"0.2.1",
						"Неверный запрос",
						"Неверный формат данных",
						"BODY"
					)
				);

			Roles? role = await _context.Roles.FirstOrDefaultAsync(r => r.Uuid == uuid);
			if (role == null)
				return NotFound(
					new ApiError(
						"1.2.3",
						"Роль не найдена",
						"Роль с указанным UUID не найдена",
						nameof(uuid)
					)
				);

			if (!string.IsNullOrWhiteSpace(request.RoleName) && request.RoleName != role.RoleName)
			{
				bool exists = await _context.Roles.AnyAsync(r =>
					r.RoleName == request.RoleName && r.Uuid != uuid
				);
				if (exists)
					return Conflict(
						new ApiError(
							"1.2.1",
							"Конфликт",
							"RoleName уже используется",
							nameof(RolesUpdateDto.RoleName)
						)
					);
				role.RoleName = request.RoleName.Trim();
			}

			if (request.Name != null)
			{
				role.Name = request.Name;
			}

			if (request.RightsUuids != null)
			{
				// Явный список запрошенных UUID прав
				List<Guid> requested = request.RightsUuids.Distinct().ToList();

				// Загружаем объекты прав по UUID
				List<RoleRights> rights = await _context
					.RoleRights.Where(rr => requested.Contains(rr.Uuid))
					.ToListAsync();

				// Вычисляем недостающие UUID прав
				List<Guid> missing = requested.Except(rights.Select(r => r.Uuid)).ToList();
				if (missing.Count > 0)
				{
					return BadRequest(
						new ApiError
						{
							StatusCode = "0.2.1",
							Title = "Неверный запрос",
							Message = "Некоторые права не найдены",
							Field = nameof(request.RightsUuids),
							Details = string.Join(", ", missing),
						}
					);
				}

				// Права, которые сейчас принадлежат этой роли
				List<RoleRights> current = await _context
					.RoleRights.Where(rr => rr.RoleId == role.RoleId)
					.ToListAsync();

				// Убираем права, которые были удалены из запроса
				List<RoleRights> toRemove = current
					.Where(cr => !requested.Contains(cr.Uuid))
					.ToList();
				foreach (RoleRights r in toRemove)
					r.RoleId = null;

				// Проверяем, что нет попытки захватить права, принадлежащие другим ролям
				if (rights.Any(rr => rr.RoleId != null && rr.RoleId != role.RoleId))
				{
					return Conflict(
						new ApiError(
							"1.2.1",
							"Конфликт",
							"Одна или несколько прав уже привязаны к другой роли",
							nameof(request.RightsUuids)
						)
					);
				}

				// Добавляем права, которые нужно привязать
				List<RoleRights> toAdd = rights
					.Where(rr => rr.RoleId == null || rr.RoleId == role.RoleId)
					.ToList();
				foreach (RoleRights r in toAdd)
					r.RoleId = role.RoleId;
			}

			await _context.SaveChangesAsync();

			return Ok(
				new
				{
					role.Uuid,
					role.Name,
					role.RoleName,
				}
			);
		}

		[HttpPut("{uuid}")]
		[SwaggerResponse(StatusCodes.Status200OK, "Роль заменена", typeof(object))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Роль не найдена", typeof(ApiError))]
		[SwaggerResponse(StatusCodes.Status409Conflict,"Конфликт: имя роли уже используется",typeof(ApiError))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "Неверный формат данных", "BODY")]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "Name и RoleName обязательны", "Name, RoleName")]
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
			if (replaceDto == null)
				return BadRequest(
					new ApiError("0.2.1", "Неверный запрос", "Неверный формат данных", "BODY")
				);
			if (
				string.IsNullOrWhiteSpace(replaceDto.Name)
				|| string.IsNullOrWhiteSpace(replaceDto.RoleName)
			)
			{
				return BadRequest(
					new ApiError(
						"0.2.1",
						"Неверный запрос",
						"Name и RoleName обязательны",
						"Name, RoleName"
					)
				);
			}

			Roles? role = await _context.Roles.FirstOrDefaultAsync(r => r.Uuid == uuid);
			if (role == null)
				return NotFound(
					new ApiError(
						"1.2.3",
						"Роль не найдена",
						"Роль с указанным UUID не найдена",
						nameof(uuid)
					)
				);

			bool exists = await _context.Roles.AnyAsync(r =>
				r.RoleName == replaceDto.RoleName && r.Uuid != uuid
			);
			if (exists)
				return Conflict(
					new ApiError(
						"1.2.1",
						"Конфликт",
						"RoleName уже используется",
						nameof(RolesCreateDto.RoleName)
					)
				);

			role.Name = replaceDto.Name.Trim();
			role.RoleName = replaceDto.RoleName.Trim();

			// handle rights: PUT semantics - replace set. Null or empty clears.
			var requested = replaceDto.RightsUuids?.Distinct().ToList() ?? new List<Guid>();

			if (requested.Count > 0)
			{
				// Загружаем объекты прав по UUID
				List<RoleRights> rights = await _context
					.RoleRights.Where(rr => requested.Contains(rr.Uuid))
					.ToListAsync();

				// Вычисляем недостающие UUID прав
				List<Guid> missing = requested.Except(rights.Select(r => r.Uuid)).ToList();
				if (missing.Count > 0)
				{
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

				if (rights.Any(rr => rr.RoleId != null && rr.RoleId != role.RoleId))
				{
					return Conflict(
						new ApiError(
							"1.2.1",
							"Конфликт",
							"Одна или несколько прав уже привязаны к другой роли",
							nameof(replaceDto.RightsUuids)
						)
					);
				}

				// Отвязываем существующие права, которые не присутствуют в новом наборе
				List<RoleRights> current = await _context
					.RoleRights.Where(rr => rr.RoleId == role.RoleId)
					.ToListAsync();
				foreach (RoleRights r in current.Where(cr => !requested.Contains(cr.Uuid)))
					r.RoleId = null;

				// Привязываем запрошенные права
				foreach (RoleRights r in rights)
					r.RoleId = role.RoleId;
			}
			else
			{
				// clear all rights
				var currentAll = await _context
					.RoleRights.Where(rr => rr.RoleId == role.RoleId)
					.ToListAsync();
				foreach (var r in currentAll)
					r.RoleId = null;
			}

			await _context.SaveChangesAsync();

			return Ok(
				new
				{
					role.Uuid,
					role.Name,
					role.RoleName,
				}
			);
		}

		[HttpDelete("{uuid}")]
		[SwaggerResponse(StatusCodes.Status204NoContent, "Роль удалена")]
		[SwaggerOperation(Summary = "Удалить роль по UUID")]
		[ApiErrorExample(StatusCodes.Status404NotFound, "1.2.3", "Роль не найдена", "Роль с указанным UUID не найдена", nameof(uuid))]
		public async Task<IActionResult> DeleteRole(Guid uuid)
		{
			Roles? role = await _context.Roles.FirstOrDefaultAsync(r => r.Uuid == uuid);
			if (role == null)
				return NotFound(
					new ApiError(
						"1.2.3",
						"Роль не найдена",
						"Роль с указанным UUID не найдена",
						nameof(uuid)
					)
				);

			_context.Roles.Remove(role);
			await _context.SaveChangesAsync();

			return NoContent();
		}
	}
}
