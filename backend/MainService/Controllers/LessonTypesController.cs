using MainService.Errors;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace MainService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LessonTypesController : ControllerBase
    {
        private readonly MainServiceContext _context;

        public LessonTypesController(MainServiceContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Типы занятий найдены", typeof(IEnumerable<LessonTypesResponseDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Типы занятий не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить список всех типов занятий",
            Description = "Возвращает список всех типов занятий в системе"
        )]
        public async Task<ActionResult<IEnumerable<LessonTypesResponseDto>>> GetLessonTypes()
        {
            List<LessonTypes> lessonTypes = await _context.LessonTypes
                .Include(lt => lt.Lessons)
                .AsNoTracking()
                .ToListAsync();

            if (lessonTypes.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Типы занятий не найдены",
                    Message = "В системе не найдено ни одного типа занятий",
                    Field = string.Empty
                });
            }

            return Ok(lessonTypes.Select(lt => new LessonTypesResponseDto(lt)));
        }

        [HttpGet("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Тип занятия найден", typeof(LessonTypesResponseDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Тип занятия не найден", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Получить тип занятия по UUID",
            Description = "Возвращает тип занятия по указанному UUID"
        )]
        public async Task<ActionResult<LessonTypesResponseDto>> GetLessonType(
            [SwaggerParameter("UUID типа занятия")]
            Guid uuid
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID типа занятия не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            LessonTypes? lessonType = await _context.LessonTypes
                .Include(lt => lt.Lessons)
                .AsNoTracking()
                .FirstOrDefaultAsync(lt => lt.Uuid == uuid);

            if (lessonType == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "1.0.3",
                    Title = "Тип занятия не найден",
                    Message = $"Тип занятия с UUID \"{uuid}\" не найден",
                    Field = nameof(uuid)
                });
            }

            return Ok(new LessonTypesResponseDto(lessonType));
        }
    }
}
