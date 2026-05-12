using MainService.Errors;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

using Z.EntityFramework.Plus;

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
        [SwaggerResponse(200, "Студент найден", typeof(StudentsDto))]
        [SwaggerResponse(404, "Студенты не найдены", typeof(ApiError))]
        [SwaggerResponseExample(404, typeof(ApiError404NotFoundExample))]
        [SwaggerResponse(400, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(400, typeof(ApiError400BadRequestExample))]
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
                    StatusCode = "0.0",
                    Title = "Неверный запрос",
                    Message = "UUID студента не может быть пустым"
                });
            }
            var student = await _context.Students.FindAsync(uuid);
            if (student == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "0.0",
                    Title = "Студент не найден",
                    Message = $"Студент с UUID \"{uuid}\" не найден"
                });
            }
            return Ok(new StudentsDto(student));
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
        [SwaggerResponse(200, "Студенты найдены", typeof(PagedResult<StudentsDto>))]
        [SwaggerResponse(404, "Студенты не найдены", typeof(ApiError))]
        [SwaggerResponseExample(404, typeof(ApiError404NotFoundExample))]
        [SwaggerResponse(400, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(400, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Получить студентов по группе",
            Description = "Возвращает список студентов по указанной группе"
        )]
        public async Task<ActionResult<PagedResult<StudentsDto>>> GetStudentsByGroup(
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
                    StatusCode = "0.0",
                    Title = "Неверный запрос",
                    Message = "UUID группы не может быть пустым"
                });
            }
            var group = await _context.Groups
            .FirstOrDefaultAsync(g => g.Uuid == groupUuid);
            if (group == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "0.1",
                    Title = "Группа не найдена",
                    Message = $"Группа с UUID \"{groupUuid}\" не найдена"
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

            var futureCount = baseQuery.DeferredCount().FutureValue();
            var futureItems = baseQuery
                .OrderBy(s => s.StudentId)
                .Skip(offset)
                .Take(size)
                .Future();

            List<Students> studentsList = futureItems.ToList();
            int total = futureCount.Value;

            if (studentsList.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "0.0",
                    Title = "Студенты не найдены",
                    Message = $"Студенты в группе с UUID \"{groupUuid}\" не найдены"
                });
            }

            List<StudentsDto> studentsDtoList = studentsList.Select(s => new StudentsDto(s)).ToList();

            return Ok(new PagedResult<StudentsDto>(total, offset, studentsDtoList.Count, studentsDtoList));
        }


    };



}