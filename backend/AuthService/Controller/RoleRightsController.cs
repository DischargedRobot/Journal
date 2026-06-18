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
	[Route("auth-service/v1/[controller]")]
	[Produces("application/json")]
	public class RoleRightsController : ControllerBase
	{
		private readonly ILogger<RoleRightsController> _logger;
		private readonly AuthServiceContext _context;
		private readonly ActivitySource _activitySource;

		public RoleRightsController(
			AuthServiceContext context,
			ILogger<RoleRightsController> logger,
			ActivitySource activitySource)
		{
			_context = context;
			_logger = logger;
			_activitySource = activitySource;
		}

		[HttpGet]
		[SwaggerResponse(StatusCodes.Status200OK, "Права ролей найдены", typeof(PagedResult<RoleRightsResponseDto>))]
		[ResponseExample(StatusCodes.Status200OK, typeof(PagedResult<RoleRightsResponseDto>))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "Параметр offset не может быть отрицательным", nameof(offset))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.1", "Неверный запрос", "Параметр size не может быть отрицательным", nameof(size))]
		[ApiErrorExample(StatusCodes.Status404NotFound, "1.0.3", "Права ролей не найдены", "В системе не найдено ни одного права роли", "")]
		[ApiErrorExample(StatusCodes.Status404NotFound, "1.1.3", "Права ролей не найдены", "В системе не найдено ни одного права роли для указанных параметров запроса", "BODY")]
		[SwaggerOperation(Summary = "Получить список прав ролей")]
		public async Task<ActionResult<PagedResult<RoleRightsResponseDto>>> GetRoleRights(
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

				IQueryable<RoleRights> baseQuery = _context
					.RoleRights.Where(rr => filterName == null || rr.Name.Contains(filterName))
					.AsNoTracking();

				int total = await baseQuery.CountAsync();

				List<RoleRightsResponseDto> items = await baseQuery
					.SortByKey(rr => rr.Name, sortOrder)
					.TakeWithOffset(offset, size)
					.Select(rr => new RoleRightsResponseDto(rr))
					.ToListAsync();

				_logger.LogInformation("{Function}: найдено записей = {Total}", functionName, total);

				if (total == 0)
				{
					_logger.LogInformation("{Function}: права ролей не найдены (total=0)", functionName);
					return NotFound(new ApiError(
						"1.0.3",
						"Права ролей не найдены",
						"В системе не найдено ни одного права роли",
						string.Empty));
				}

				if (items.Count == 0)
				{
					_logger.LogInformation(
						"{Function}: нет прав ролей по фильтру (total={Total}, offset={Offset})",
						functionName, total, offset);
					return NotFound(new ApiError(
						"1.1.3",
						"Права ролей не найдены",
						"В системе не найдено ни одного права роли для указанных параметров запроса",
						"BODY"));
				}

				PagedResult<RoleRightsResponseDto> result = new(
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
		[SwaggerResponse(StatusCodes.Status200OK, "Право роли найдено", typeof(RoleRightsResponseDto))]
		[ResponseExample(StatusCodes.Status200OK, typeof(RoleRightsResponseDto))]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Право роли не найдено", typeof(ApiError))]
		[ApiErrorExample(StatusCodes.Status404NotFound, "1.2.3", "Право роли не найдено", "Право роли с указанным UUID не найдено", nameof(uuid))]
		[SwaggerOperation(Summary = "Получить право роли по UUID")]
		public async Task<IActionResult> GetRoleRight(
			[SwaggerParameter("UUID права роли")]
			Guid uuid)
		{
			string functionName = ControllerContext.ActionDescriptor.ActionName;
			try
			{
				using Activity? activity = _activitySource.StartAndLog(_logger, this);
				_logger.LogInformation("{Function}: вызвано для uuid={Uuid}", functionName, uuid);

				RoleRightsResponseDto? roleRight = await _context.RoleRights
					.Where(rr => rr.Uuid == uuid)
					.Select(rr => new RoleRightsResponseDto(rr))
					.FirstOrDefaultAsync();

				if (roleRight == null)
				{
					_logger.LogInformation("{Function}: право роли uuid={Uuid} не найдено", functionName, uuid);
					return NotFound(new ApiError(
						"1.2.3",
						"Право роли не найдено",
						"Право роли с указанным UUID не найдено",
						nameof(uuid)));
				}

				_logger.LogInformation("{Function}: возвращено право роли uuid={Uuid}", functionName, uuid);
				return Ok(roleRight);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "{Function}: неожиданная ошибка при получении права роли", functionName);
				return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
				{
					StatusCode = "1.0.0",
					Title = "Внутренняя ошибка сервера",
					Message = "Произошла ошибка на сервере",
				});
			}
		}

		[HttpPost]
		[SwaggerResponse(StatusCodes.Status201Created, "Право роли создано", typeof(RoleRightsResponseDto))]
		[ResponseExample(StatusCodes.Status201Created, typeof(RoleRightsResponseDto))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Некорректные данные", typeof(ApiError))]
		[SwaggerResponse(StatusCodes.Status409Conflict, "Право роли уже существует", typeof(ApiError))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.1.0", "Неверный запрос", "Тело запроса не может быть пустым", "BODY")]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.0", "Неверный запрос", "Поле Name обязательно для создания права роли", "Name")]
		[ApiErrorExample(StatusCodes.Status409Conflict, "1.2.1", "Конфликт", "Право роли с таким Name уже существует", nameof(RoleRightsCreateDto.Name))]
		[SwaggerOperation(Summary = "Создать новое право роли")]
		public async Task<IActionResult> CreateRoleRight(
			[FromBody, SwaggerParameter("Тело запроса: данные для создания права роли")]
			RoleRightsCreateDto? createDto)
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
						"Поле Name обязательно для создания права роли",
						nameof(createDto.Name)));
				}

				bool exists = await _context.RoleRights.AnyAsync(rr => rr.Name == createDto.Name);
				if (exists)
				{
					_logger.LogWarning("{Function}: право роли с Name={Name} уже существует", functionName, createDto.Name);
					return Conflict(new ApiError(
						"1.2.1",
						"Конфликт",
						"Право роли с таким Name уже существует",
						nameof(RoleRightsCreateDto.Name)));
				}

				RoleRights roleRight = new()
				{
					Uuid = Guid.NewGuid(),
					Name = createDto.Name.Trim(),
				};

				_context.RoleRights.Add(roleRight);
				await _context.SaveChangesAsync();

				_logger.LogInformation("{Function}: создано право роли uuid={Uuid}", functionName, roleRight.Uuid);
				return CreatedAtAction(
					nameof(GetRoleRight),
					new { uuid = roleRight.Uuid },
					new RoleRightsResponseDto(roleRight));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "{Function}: неожиданная ошибка при создании права роли", functionName);
				return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
				{
					StatusCode = "1.0.0",
					Title = "Внутренняя ошибка сервера",
					Message = "Произошла ошибка на сервере",
				});
			}
		}

		[HttpPatch("{uuid}")]
		[SwaggerResponse(StatusCodes.Status200OK, "Право роли обновлено", typeof(RoleRightsResponseDto))]
		[ResponseExample(StatusCodes.Status200OK, typeof(RoleRightsResponseDto))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Право роли не найдено", typeof(ApiError))]
		[SwaggerResponse(StatusCodes.Status409Conflict, "Имя права роли уже используется", typeof(ApiError))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.1.0", "Неверный запрос", "Тело запроса не может быть пустым", "BODY")]
		[ApiErrorExample(StatusCodes.Status404NotFound, "1.2.3", "Право роли не найдено", "Право роли с указанным UUID не найдено", nameof(uuid))]
		[ApiErrorExample(StatusCodes.Status409Conflict, "1.2.1", "Конфликт", "Name уже используется", nameof(RoleRightsUpdateDto.Name))]
		[SwaggerOperation(Summary = "Частично обновить право роли по UUID")]
		public async Task<IActionResult> UpdateRoleRight(
			[SwaggerParameter("UUID права роли")]
			Guid uuid,
			[FromBody, SwaggerParameter("Тело запроса: частичные данные для обновления права роли")]
			RoleRightsUpdateDto? request)
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

				RoleRights? roleRight = await _context.RoleRights.FirstOrDefaultAsync(rr => rr.Uuid == uuid);
				if (roleRight == null)
				{
					_logger.LogInformation("{Function}: право роли uuid={Uuid} не найдено", functionName, uuid);
					return NotFound(new ApiError(
						"1.2.3",
						"Право роли не найдено",
						"Право роли с указанным UUID не найдено",
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

					bool exists = await _context.RoleRights.AnyAsync(rr =>
						rr.Name == request.Name && rr.Uuid != uuid);
					if (exists)
					{
						_logger.LogWarning("{Function}: Name={Name} уже используется", functionName, request.Name);
						return Conflict(new ApiError(
							"1.2.1",
							"Конфликт",
							"Name уже используется",
							nameof(RoleRightsUpdateDto.Name)));
					}

					roleRight.Name = request.Name.Trim();
				}

				await _context.SaveChangesAsync();

				_logger.LogInformation("{Function}: право роли uuid={Uuid} обновлено", functionName, uuid);
				return Ok(new RoleRightsResponseDto(roleRight));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "{Function}: неожиданная ошибка при обновлении права роли", functionName);
				return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
				{
					StatusCode = "1.0.0",
					Title = "Внутренняя ошибка сервера",
					Message = "Произошла ошибка на сервере",
				});
			}
		}

		[HttpPut("{uuid}")]
		[SwaggerResponse(StatusCodes.Status200OK, "Право роли заменено", typeof(RoleRightsResponseDto))]
		[ResponseExample(StatusCodes.Status200OK, typeof(RoleRightsResponseDto))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Право роли не найдено", typeof(ApiError))]
		[SwaggerResponse(StatusCodes.Status409Conflict, "Конфликт: имя права роли уже используется", typeof(ApiError))]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.1.0", "Неверный запрос", "Неверный формат данных", "BODY")]
		[ApiErrorExample(StatusCodes.Status400BadRequest, "0.2.0", "Неверный запрос", "Name обязательна", "Name")]
		[ApiErrorExample(StatusCodes.Status404NotFound, "1.2.3", "Право роли не найдено", "Право роли с указанным UUID не найдено", nameof(uuid))]
		[ApiErrorExample(StatusCodes.Status409Conflict, "1.2.1", "Конфликт", "Name уже используется", nameof(RoleRightsCreateDto.Name))]
		[SwaggerOperation(Summary = "Полная замена права роли по UUID")]
		public async Task<IActionResult> ReplaceRoleRight(
			[SwaggerParameter("UUID права роли")]
			Guid uuid,
			[FromBody, SwaggerParameter("Тело запроса: данные для полной замены права роли")]
			RoleRightsCreateDto? replaceDto)
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

				RoleRights? roleRight = await _context.RoleRights.FirstOrDefaultAsync(rr => rr.Uuid == uuid);
				if (roleRight == null)
				{
					_logger.LogInformation("{Function}: право роли uuid={Uuid} не найдено", functionName, uuid);
					return NotFound(new ApiError(
						"1.2.3",
						"Право роли не найдено",
						"Право роли с указанным UUID не найдено",
						nameof(uuid)));
				}

				bool exists = await _context.RoleRights.AnyAsync(rr =>
					rr.Name == replaceDto.Name && rr.Uuid != uuid);
				if (exists)
				{
					_logger.LogWarning("{Function}: Name={Name} уже используется", functionName, replaceDto.Name);
					return Conflict(new ApiError(
						"1.2.1",
						"Конфликт",
						"Name уже используется",
						nameof(RoleRightsCreateDto.Name)));
				}

				roleRight.Name = replaceDto.Name.Trim();
				await _context.SaveChangesAsync();

				_logger.LogInformation("{Function}: право роли uuid={Uuid} заменено", functionName, uuid);
				return Ok(new RoleRightsResponseDto(roleRight));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "{Function}: неожиданная ошибка при замене права роли", functionName);
				return StatusCode(StatusCodes.Status500InternalServerError, new ApiError
				{
					StatusCode = "1.0.0",
					Title = "Внутренняя ошибка сервера",
					Message = "Произошла ошибка на сервере",
				});
			}
		}

		[HttpDelete("{uuid}")]
		[SwaggerResponse(StatusCodes.Status204NoContent, "Право роли удалено")]
		[SwaggerOperation(Summary = "Удалить право роли по UUID")]
		[ApiErrorExample(StatusCodes.Status404NotFound, "1.2.3", "Право роли не найдено", "Право роли с указанным UUID не найдено", nameof(uuid))]
		public async Task<IActionResult> DeleteRoleRight(
			[SwaggerParameter("UUID права роли")]
			Guid uuid)
		{
			string functionName = ControllerContext.ActionDescriptor.ActionName;
			try
			{
				using Activity? activity = _activitySource.StartAndLog(_logger, this);
				_logger.LogInformation("{Function}: вызвано для uuid={Uuid}", functionName, uuid);

				RoleRights? roleRight = await _context.RoleRights.FirstOrDefaultAsync(rr => rr.Uuid == uuid);
				if (roleRight == null)
				{
					_logger.LogInformation("{Function}: право роли uuid={Uuid} не найдено", functionName, uuid);
					return NotFound(new ApiError(
						"1.2.3",
						"Право роли не найдено",
						"Право роли с указанным UUID не найдено",
						nameof(uuid)));
				}

				_context.RoleRights.Remove(roleRight);
				await _context.SaveChangesAsync();

				_logger.LogInformation("{Function}: право роли uuid={Uuid} удалено", functionName, uuid);
				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "{Function}: неожиданная ошибка при удалении права роли", functionName);
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
