using AuthService.Enums;
using AuthService.Errors;
using AuthService.Lib.Utils;
using AuthService.Model;
using AuthService.ResponseExample;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Annotations;
using System.Diagnostics;

namespace AuthService.Controller
{
	[ApiController]
	[Route("api/auth-service/v1/[controller]")]
	[Produces("application/json")]
	public class RolesTypesController : ControllerBase
	{
		private readonly ILogger<RolesTypesController> _logger;
		private readonly AuthServiceContext _context;
		private readonly ActivitySource _activitySource;

		public RolesTypesController(
			AuthServiceContext context,
			ILogger<RolesTypesController> logger,
			ActivitySource activitySource)
		{
			_context = context;
			_logger = logger;
			_activitySource = activitySource;
		}

		[HttpGet]
		[SwaggerResponse(StatusCodes.Status200OK, "Типы ролей найдены", typeof(PagedResult<RolesTypesResponseDto>))]
		[ResponseExample(StatusCodes.Status200OK, typeof(PagedResult<RolesTypesResponseDto>))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "Параметр offset не может быть отрицательным", nameof(offset))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "Параметр size не может быть отрицательным", nameof(size))]
		[ApiErrorExample(StatusCodes.Status404NotFound, "1.0.3", "Типы ролей не найдены", "В системе не найдено ни одного типа роли", "")]
		[ApiErrorExample(StatusCodes.Status404NotFound, "1.1.3", "Типы ролей не найдены", "В системе не найдено ни одного типа роли для указанных параметров запроса", "BODY")]
		[SwaggerOperation(Summary = "Получить список типов ролей")]
		public async Task<ActionResult<PagedResult<RolesTypesResponseDto>>> GetRolesTypes(
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
				_logger.LogInformation(
					"{Function} вызвано: size={Size}, offset={Offset}, filterName={FilterName}, sortOrder={SortOrder}",
					functionName, size, offset, filterName, sortOrder);

				if (offset < 0)
				{
					_logger.LogWarning("{Function}: неверный offset {Offset}", functionName, offset);
					return BadRequest(new ApiError(
						"0.2.1",
						"Неверный запрос",
						"Параметр offset не может быть отрицательным",
						nameof(offset)));
				}

				if (size < 0)
				{
					_logger.LogWarning("{Function}: неверный size {Size}", functionName, size);
					return BadRequest(new ApiError(
						"0.2.1",
						"Неверный запрос",
						"Параметр size не может быть отрицательным",
						nameof(size)));
				}

				IQueryable<RolesTypes> baseQuery = _context
					.RolesTypes.Where(rt => filterName == null || rt.Name.Contains(filterName))
					.AsNoTracking();

				Task<int> totalRecord = baseQuery.CountAsync();

				List<RolesTypesResponseDto> items = await baseQuery
					.SortByKey(rt => rt.Name, sortOrder)
					.TakeWithOffset(offset, size)
					.Select(rt => new RolesTypesResponseDto(rt))
					.ToListAsync();

				int total = await totalRecord;

				_logger.LogInformation("{Function}: найдено записей = {Total}", functionName, total);

				if (total == 0)
				{
					_logger.LogInformation("{Function}: типы ролей не найдены (total=0)", functionName);
					return NotFound(new ApiError(
						"1.0.3",
						"Типы ролей не найдены",
						"В системе не найдено ни одного типа роли",
						string.Empty));
				}

				if (items.Count == 0)
				{
					_logger.LogInformation(
						"{Function}: нет типов ролей по фильтру (total={Total}, offset={Offset})",
						functionName, total, offset);
					return NotFound(new ApiError(
						"1.1.3",
						"Типы ролей не найдены",
						"В системе не найдено ни одного типа роли для указанных параметров запроса",
						"BODY"));
				}

				PagedResult<RolesTypesResponseDto> result = new(
					Total: total,
					Offset: offset,
					Size: items.Count,
					Items: items);

				_logger.LogInformation(
					"{Function}: возвращает {Count} элементов (offset={Offset}, total={Total})",
					functionName, items.Count, offset, total);

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
		[SwaggerResponse(StatusCodes.Status200OK, "Тип роли найден", typeof(RolesTypesResponseDto))]
		[ResponseExample(StatusCodes.Status200OK, typeof(RolesTypesResponseDto))]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Тип роли не найден", typeof(ApiError))]
		[ApiErrorExample(StatusCodes.Status404NotFound, "1.2.3", "Тип роли не найден", "Тип роли с указанным UUID не найден", nameof(uuid))]
		[SwaggerOperation(Summary = "Получить тип роли по UUID")]
		public async Task<IActionResult> GetRoleType(
			[SwaggerParameter("UUID типа роли")]
			Guid uuid)
		{
			string functionName = ControllerContext.ActionDescriptor.ActionName;
			try
			{
				using Activity? activity = _activitySource.StartAndLog(_logger, this);
				_logger.LogInformation("{Function}: вызвано для uuid={Uuid}", functionName, uuid);

				RolesTypesResponseDto? roleType = await _context.RolesTypes
					.Where(rt => rt.Uuid == uuid)
					.Select(rt => new RolesTypesResponseDto(rt))
					.FirstOrDefaultAsync();

				if (roleType == null)
				{
					_logger.LogInformation("{Function}: тип роли uuid={Uuid} не найден", functionName, uuid);
					return NotFound(new ApiError(
						"1.2.3",
						"Тип роли не найден",
						"Тип роли с указанным UUID не найден",
						nameof(uuid)));
				}

				_logger.LogInformation("{Function}: возвращён тип роли uuid={Uuid}", functionName, uuid);
				return Ok(roleType);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "{Function}: неожиданная ошибка при получении типа роли", functionName);
				return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
				{
					StatusCode = "1.0.0",
					Title = "Внутренняя ошибка сервера",
					Message = "Произошла ошибка на сервере",
				});
			}
		}

		[HttpPost]
		[SwaggerResponse(StatusCodes.Status201Created, "Тип роли создан", typeof(RolesTypesResponseDto))]
		[ResponseExample(StatusCodes.Status201Created, typeof(RolesTypesResponseDto))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Некорректные данные", typeof(ApiError))]
		[SwaggerResponse(StatusCodes.Status409Conflict, "Тип роли уже существует", typeof(ApiError))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.1.0", "Неверный запрос", "Тело запроса не может быть пустым", "BODY")]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.0", "Неверный запрос", "Поле Name обязательно для создания типа роли", "Name")]
		[ApiErrorExample(StatusCodes.Status409Conflict, "1.2.1", "Конфликт", "Тип роли с таким Name уже существует", nameof(RolesTypesCreateDto.Name))]
		[SwaggerOperation(Summary = "Создать новый тип роли")]
		public async Task<IActionResult> CreateRoleType(
			[FromBody, SwaggerParameter("Тело запроса: данные для создания типа роли")]
			RolesTypesCreateDto? createDto)
		{
			string functionName = ControllerContext.ActionDescriptor.ActionName;
			try
			{
				using Activity? activity = _activitySource.StartAndLog(_logger, this);
				_logger.LogInformation("{Function}: вызвано", functionName);

				if (createDto == null)
				{
					_logger.LogWarning("{Function}: пустой createDto", functionName);
					return BadRequest(new ApiError(
						"0.1.0",
						"Неверный запрос",
						"Неверный формат данных",
						"BODY"));
				}

				if (string.IsNullOrWhiteSpace(createDto.Name))
				{
					_logger.LogWarning("{Function}: Name отсутствует", functionName);
					return BadRequest(new ApiError(
						"0.2.0",
						"Неверный запрос",
						"Поле Name обязательно для создания типа роли",
						nameof(createDto.Name)));
				}

				bool exists = await _context.RolesTypes.AnyAsync(rt => rt.Name == createDto.Name);
				if (exists)
				{
					_logger.LogWarning("{Function}: тип роли с Name={Name} уже существует", functionName, createDto.Name);
					return Conflict(new ApiError(
						"1.2.1",
						"Конфликт",
						"Тип роли с таким Name уже существует",
						nameof(RolesTypesCreateDto.Name)));
				}

				RolesTypes roleType = new()
				{
					Uuid = Guid.NewGuid(),
					Name = createDto.Name.Trim(),
				};

				_context.RolesTypes.Add(roleType);
				await _context.SaveChangesAsync();

				_logger.LogInformation("{Function}: создан тип роли uuid={Uuid}", functionName, roleType.Uuid);
				return CreatedAtAction(
					nameof(GetRoleType),
					new { uuid = roleType.Uuid },
					new RolesTypesResponseDto(roleType));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "{Function}: неожиданная ошибка при создании типа роли", functionName);
				return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
				{
					StatusCode = "1.0.0",
					Title = "Внутренняя ошибка сервера",
					Message = "Произошла ошибка на сервере",
				});
			}
		}

		[HttpPatch("{uuid}")]
		[SwaggerResponse(StatusCodes.Status200OK, "Тип роли обновлён", typeof(RolesTypesResponseDto))]
		[ResponseExample(StatusCodes.Status200OK, typeof(RolesTypesResponseDto))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Тип роли не найден", typeof(ApiError))]
		[SwaggerResponse(StatusCodes.Status409Conflict, "Имя типа роли уже используется", typeof(ApiError))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.1.0", "Неверный запрос", "Тело запроса не может быть пустым", "BODY")]
		[ApiErrorExample(StatusCodes.Status404NotFound, "1.2.3", "Тип роли не найден", "Тип роли с указанным UUID не найден", nameof(uuid))]
		[ApiErrorExample(StatusCodes.Status409Conflict, "1.2.1", "Конфликт", "Name уже используется", nameof(RolesTypesUpdateDto.Name))]
		[SwaggerOperation(Summary = "Частично обновить тип роли по UUID")]
		public async Task<IActionResult> UpdateRoleType(
			[SwaggerParameter("UUID типа роли")]
			Guid uuid,
			[FromBody, SwaggerParameter("Тело запроса: частичные данные для обновления типа роли")]
			RolesTypesUpdateDto? request)
		{
			string functionName = ControllerContext.ActionDescriptor.ActionName;
			try
			{
				using Activity? activity = _activitySource.StartAndLog(_logger, this);
				_logger.LogInformation("{Function}: вызвано для uuid={Uuid}", functionName, uuid);

				if (request == null)
				{
					_logger.LogWarning("{Function}: пустой request", functionName);
					return BadRequest(new ApiError(
						"0.1.0",
						"Неверный запрос",
						"Тело запроса не может быть пустым",
						"BODY"));
				}

				RolesTypes? roleType = await _context.RolesTypes.FirstOrDefaultAsync(rt => rt.Uuid == uuid);
				if (roleType == null)
				{
					_logger.LogInformation("{Function}: тип роли uuid={Uuid} не найден", functionName, uuid);
					return NotFound(new ApiError(
						"1.2.3",
						"Тип роли не найден",
						"Тип роли с указанным UUID не найден",
						nameof(uuid)));
				}

				if (request.Name != null)
				{
					if (string.IsNullOrWhiteSpace(request.Name))
					{
						_logger.LogWarning("{Function}: пустой Name", functionName);
						return BadRequest(new ApiError(
							"0.2.0",
							"Неверный запрос",
							"Поле Name не может быть пустым",
							nameof(request.Name)));
					}

					bool exists = await _context.RolesTypes.AnyAsync(rt =>
						rt.Name == request.Name && rt.Uuid != uuid);
					if (exists)
					{
						_logger.LogWarning("{Function}: Name={Name} уже используется", functionName, request.Name);
						return Conflict(new ApiError(
							"1.2.1",
							"Конфликт",
							"Name уже используется",
							nameof(RolesTypesUpdateDto.Name)));
					}

					roleType.Name = request.Name.Trim();
				}

				await _context.SaveChangesAsync();

				_logger.LogInformation("{Function}: тип роли uuid={Uuid} обновлён", functionName, uuid);
				return Ok(new RolesTypesResponseDto(roleType));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "{Function}: неожиданная ошибка при обновлении типа роли", functionName);
				return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
				{
					StatusCode = "1.0.0",
					Title = "Внутренняя ошибка сервера",
					Message = "Произошла ошибка на сервере",
				});
			}
		}

		[HttpPut("{uuid}")]
		[SwaggerResponse(StatusCodes.Status200OK, "Тип роли заменён", typeof(RolesTypesResponseDto))]
		[ResponseExample(StatusCodes.Status200OK, typeof(RolesTypesResponseDto))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Тип роли не найден", typeof(ApiError))]
		[SwaggerResponse(StatusCodes.Status409Conflict, "Конфликт: имя типа роли уже используется", typeof(ApiError))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.1.0", "Неверный запрос", "Неверный формат данных", "BODY")]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.0", "Неверный запрос", "Name обязательна", "Name")]
		[ApiErrorExample(StatusCodes.Status404NotFound, "1.2.3", "Тип роли не найден", "Тип роли с указанным UUID не найден", nameof(uuid))]
		[ApiErrorExample(StatusCodes.Status409Conflict, "1.2.1", "Конфликт", "Name уже используется", nameof(RolesTypesCreateDto.Name))]
		[SwaggerOperation(Summary = "Полная замена типа роли по UUID")]
		public async Task<IActionResult> ReplaceRoleType(
			[SwaggerParameter("UUID типа роли")]
			Guid uuid,
			[FromBody, SwaggerParameter("Тело запроса: данные для полной замены типа роли")]
			RolesTypesCreateDto? replaceDto)
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
						"BODY"));
				}

				if (string.IsNullOrWhiteSpace(replaceDto.Name))
				{
					_logger.LogWarning("{Function}: Name отсутствует", functionName);
					return BadRequest(new ApiError(
						"0.2.0",
						"Неверный запрос",
						"Name обязательна",
						nameof(replaceDto.Name)));
				}

				RolesTypes? roleType = await _context.RolesTypes.FirstOrDefaultAsync(rt => rt.Uuid == uuid);
				if (roleType == null)
				{
					_logger.LogInformation("{Function}: тип роли uuid={Uuid} не найден", functionName, uuid);
					return NotFound(new ApiError(
						"1.2.3",
						"Тип роли не найден",
						"Тип роли с указанным UUID не найден",
						nameof(uuid)));
				}

				bool exists = await _context.RolesTypes.AnyAsync(rt =>
					rt.Name == replaceDto.Name && rt.Uuid != uuid);
				if (exists)
				{
					_logger.LogWarning("{Function}: Name={Name} уже используется", functionName, replaceDto.Name);
					return Conflict(new ApiError(
						"1.2.1",
						"Конфликт",
						"Name уже используется",
						nameof(RolesTypesCreateDto.Name)));
				}

				roleType.Name = replaceDto.Name.Trim();
				await _context.SaveChangesAsync();

				_logger.LogInformation("{Function}: тип роли uuid={Uuid} заменён", functionName, uuid);
				return Ok(new RolesTypesResponseDto(roleType));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "{Function}: неожиданная ошибка при замене типа роли", functionName);
				return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
				{
					StatusCode = "1.0.0",
					Title = "Внутренняя ошибка сервера",
					Message = "Произошла ошибка на сервере",
				});
			}
		}

		[HttpDelete("{uuid}")]
		[SwaggerResponse(StatusCodes.Status204NoContent, "Тип роли удалён")]
		[SwaggerOperation(Summary = "Удалить тип роли по UUID")]
		[ApiErrorExample(StatusCodes.Status404NotFound, "1.2.3", "Тип роли не найден", "Тип роли с указанным UUID не найден", nameof(uuid))]
		public async Task<IActionResult> DeleteRoleType(
			[SwaggerParameter("UUID типа роли")]
			Guid uuid)
		{
			string functionName = ControllerContext.ActionDescriptor.ActionName;
			try
			{
				using Activity? activity = _activitySource.StartAndLog(_logger, this);
				_logger.LogInformation("{Function}: вызвано для uuid={Uuid}", functionName, uuid);

				RolesTypes? roleType = await _context.RolesTypes.FirstOrDefaultAsync(rt => rt.Uuid == uuid);
				if (roleType == null)
				{
					_logger.LogInformation("{Function}: тип роли uuid={Uuid} не найден", functionName, uuid);
					return NotFound(new ApiError(
						"1.2.3",
						"Тип роли не найден",
						"Тип роли с указанным UUID не найден",
						nameof(uuid)));
				}

				_context.RolesTypes.Remove(roleType);
				await _context.SaveChangesAsync();

				_logger.LogInformation("{Function}: тип роли uuid={Uuid} удалён", functionName, uuid);
				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "{Function}: неожиданная ошибка при удалении типа роли", functionName);
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
