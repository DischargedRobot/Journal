using MainService.Errors;
using MainService.Enums;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using MainService.Lib.Utils;
using System.Diagnostics;

namespace MainService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class AttestationMarksController : ControllerBase
    {
        private readonly ILogger<AttestationMarksController> _logger;
        private readonly MainServiceContext _context;
        private readonly ActivitySource _activitySource;

        public AttestationMarksController(MainServiceContext context, ILogger<AttestationMarksController> logger, System.Diagnostics.ActivitySource activitySource)
        {
            _context = context;
            _logger = logger;
            _activitySource = activitySource;
        }


        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Оценки аттестаций найдены", typeof(PagedResult<AttestationMarksResponseDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Оценки аттестаций не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить список оценок аттестаций",
            Description = "Возвращает список оценок аттестаций в системе"
        )]
        public async Task<ActionResult<PagedResult<AttestationMarksResponseDto>>> GetAttestationMarks(
            [FromQuery, SwaggerParameter("Количество записей")]
            int size = 50,
            [FromQuery, SwaggerParameter("Смещение от начала списка")]
            int offset = 0,
            [FromQuery, SwaggerParameter("Фильтр по значению оценки")]
            string? filterMark = null,
            [FromQuery, SwaggerParameter("Порядок сортировки по значению оценки")]
            SortOrder sortOrder = SortOrder.Ascending
        )
        {
            string functionName = ControllerContext.ActionDescriptor.ActionName;
            try
            {
                using Activity? activity = _activitySource.StartAndLog(_logger, this);
                _logger.LogInformation("{Function} вызвано: size={Size}, offset={Offset}, filterMark={FilterMark}, sortOrder={SortOrder}", functionName, size, offset, filterMark, sortOrder);

                if (offset < 0)
                {
                    _logger.LogWarning("{Function}: неверный offset {Offset}", functionName, offset);
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.1",
                        Title = "Неверный запрос",
                        Message = "Параметр offset не может быть отрицательным",
                        Field = nameof(offset)
                    });
                }

                if (size < 0)
                {
                    _logger.LogWarning("{Function}: неверный size {Size}", functionName, size);
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.1",
                        Title = "Неверный запрос",
                        Message = "Параметр size не может быть отрицательным",
                        Field = nameof(size)
                    });
                }

                IQueryable<AttestationMarks> baseQuery = _context.AttestationMarks
                    .Where(am => filterMark == null || am.Mark.Contains(filterMark))
                    .Include(am => am.AttestationType)
                    .AsNoTracking();

                int total = await baseQuery.CountAsync();

                List<AttestationMarksResponseDto> AttestationMarks = await baseQuery
                    .SortByKey(am => am.Mark, sortOrder)
                    .TakeWithOffset(offset, size)
                    .Select(am => new AttestationMarksResponseDto(am))
                    .ToListAsync();

                _logger.LogInformation("{Function}: найдено записей = {Total}", functionName, total);

                if (total == 0)
                {
                    _logger.LogInformation("{Function}: не найдено записей (total=0)", functionName);
                    return NotFound(new ApiError
                    {
                        StatusCode = "1.0.3",
                        Title = "Оценки аттестаций не найдены",
                        Message = "В системе не найдено ни одной оценки аттестации",
                        Field = string.Empty
                    });
                }

                if (AttestationMarks.Count == 0)
                {
                    _logger.LogInformation("{Function}: нет записей по фильтру (total={Total}, offset={Offset})", functionName, total, offset);
                    return NotFound(new ApiError
                    {
                        StatusCode = "1.1.3",
                        Title = "Оценки аттестаций не найдены",
                        Message = "В системе не найдено ни одной оценки аттестации для указанных параметров запроса",
                        Field = "BODY"
                    });
                }

                _logger.LogInformation("{Function}: возвращает {Count} элементов (offset={Offset}, total={Total})", functionName, AttestationMarks.Count, offset, total);

                return Ok(new PagedResult<AttestationMarksResponseDto>(
                    Total: total,
                    Offset: offset,
                    Size: AttestationMarks.Count,
                    Items: AttestationMarks
                ));
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
        [SwaggerResponse(StatusCodes.Status200OK, "Оценка аттестации найдена", typeof(AttestationMarksResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [ApiErrorExample(400,
           "0.2.0",
            "Неверный запрос",
            "UUID оценки аттестации не может быть пустым",
            "uuid"
        )]
        [ApiErrorExample(400,
           "0.2.0",
            "Неверный запрос",
            "UUID оценки аттестации не может быть пустым",
            "uuidы"
        )]
        [SwaggerOperation(
            Summary = "Получить оценку аттестации по UUID",
            Description = "Возвращает оценку аттестации по указанному UUID"
        )]
        public async Task<ActionResult<AttestationMarksResponseDto>> GetAttestationMark(
            [SwaggerParameter("UUID оценки аттестации")]
            Guid uuid
        )
        {
            string functionName = ControllerContext.ActionDescriptor.ActionName;
            try
            {
                using Activity? activity = _activitySource.StartAndLog(_logger, this);
                _logger.LogInformation("{Function}: вызвано для uuid={Uuid}", functionName, uuid);

                if (uuid == Guid.Empty)
                {
                    _logger.LogWarning("{Function}: пустой uuid", functionName);
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.0",
                        Title = "Неверный запрос",
                        Message = "UUID оценки аттестации не может быть пустым",
                        Field = nameof(uuid)
                    });
                }

                AttestationMarks? attestationMark = await _context.AttestationMarks
                    .Include(am => am.AttestationType)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(am => am.Uuid == uuid);

                if (attestationMark == null)
                {
                    _logger.LogInformation("{Function}: запись с uuid={Uuid} не найдена", functionName, uuid);
                    return NotFound(new ApiError
                    {
                        StatusCode = "1.2.3",
                        Title = "Оценка аттестации не найдена",
                        Message = $"Оценка аттестации с UUID \"{uuid}\" не найдена",
                        Field = nameof(uuid)
                    });
                }

                _logger.LogInformation("{Function}: возвращена запись uuid={Uuid}", functionName, uuid);
                return Ok(new AttestationMarksResponseDto(attestationMark));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Function}: неожиданная ошибка при получении оценки", functionName);
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
