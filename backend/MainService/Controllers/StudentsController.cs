using MainService.Errors;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

using System.Text.Json.Serialization;

namespace MainService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly MainServiceContext _context;

        public StudentsController(MainServiceContext context)
        {
            _context = context;
        }

        [HttpGet("{Uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Студент найден", typeof(StudentsResponseDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Студенты не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Получить студента по ID",
            Description = "Возвращает студента по указанному ID"
        )]
        public async Task<IActionResult> GetStudent(Guid uuid)
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID студента не может быть пустым",
                    Field = nameof(uuid)
                });
            }
            var student = await _context.Students.FindAsync(uuid);
            if (student == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "0.0.3",
                    Title = "Студент не найден",
                    Message = $"Студент с UUID \"{uuid}\" не найден",
                    Field = nameof(uuid)
                });
            }
            return Ok(new StudentsResponseDto(student));
        }

        // [JsonConverter(typeof(JsonStringEnumConverter))]
        // public enum SortOrder
        // {
        //     Ascending,
        //     Descending
        // }

        // [JsonConverter(typeof(JsonStringEnumConverter))]
        // public enum SortBy
        // {
        //     Group,
        //     FullName,

        // }

        [HttpGet("group/{groupUuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Студенты найдены", typeof(PagedResult<StudentsResponseDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Студенты не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Получить студентов по группе",
            Description = "Возвращает список студентов по указанной группе"
        )]
        public async Task<ActionResult<PagedResult<StudentsResponseDto>>> GetStudentsByGroup(
            Guid groupUuid,
            [FromQuery] int offset = 0,
            [FromQuery] int size = 50,
            [FromQuery, SwaggerParameter("ФИО")] string? filterFullName = null
        // [FromQuery] SortBy[]? sortBy = null,
        // [FromQuery] SortOrder[]? sortOrder = null
        )
        {
            if (groupUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID группы не может быть пустым",
                    Field = nameof(groupUuid)
                });
            }
            var group = await _context.Groups
            .FirstOrDefaultAsync(g => g.Uuid == groupUuid);
            if (group == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "0.0.3",
                    Title = "Группа не найдена",
                    Message = $"Группа с UUID \"{groupUuid}\" не найдена",
                    Field = nameof(groupUuid)
                });
            }

            if (offset < 0)
            {
                offset = 0;
            }

            if (size <= 0)
            {
                size = 50;
            }

            var baseQuery = _context.Students
                .Where(s => s.GroupId == group.GroupId
                // TODO: подумать нужен ли тут фильтр и как именно будет идти фильтраци на клиенте
                // (каждый ввод = запрос или по кнопке)
                && (filterFullName == null
                || s.StudentPerson!.User!.FirstName.Contains(filterFullName)
                || s.StudentPerson.User.LastName.Contains(filterFullName)
                || (s.StudentPerson.User.Patronymic != null && s.StudentPerson.User.Patronymic.Contains(filterFullName))))
                .Include(s => s.Group)
                .AsNoTracking();

            Task<int> totalTask = baseQuery.CountAsync();
            var itemsQuery = baseQuery
                .OrderBy(s => s.StudentId)
                .Skip(offset)
                .Take(size);

            Task<List<Students>> itemsTask = itemsQuery.ToListAsync();


            List<Students> studentsList = await itemsTask;
            int total = await totalTask;

            if (studentsList.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "0.0.3",
                    Title = "Студенты не найдены",
                    Message = $"Студенты в группе с UUID \"{groupUuid}\" не найдены",
                    Field = nameof(groupUuid)
                });
            }

            List<StudentsResponseDto> studentsDtoList = studentsList.Select(s => new StudentsResponseDto(s)).ToList();

            return Ok(new PagedResult<StudentsResponseDto>(total, offset, studentsDtoList.Count, studentsDtoList));
        }


    };



}
