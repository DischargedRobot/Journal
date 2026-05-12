using MainService.Errors;

using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;


namespace MainService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DisciplinesController : ControllerBase
    {
        private readonly MainServiceContext _context;

        public DisciplinesController(MainServiceContext context)
        {
            _context = context;
        }


        [HttpGet]
        [SwaggerResponse(200, "Дисциплины найдены", typeof(IEnumerable<DisciplinesDto>))]
        [SwaggerOperation(
            Summary = "Получить список всех дисциплин",
            Description = "Возвращает список всех дисциплин в системе"
        )]
        public async Task<ActionResult<IEnumerable<DisciplinesDto>>> GetDisciplines()
        {
            List<Disciplines> disciplinesList = await _context.Disciplines.ToListAsync();
            List<DisciplinesDto> disciplinesDtoList = disciplinesList
                .Select(d => new DisciplinesDto(d))
                .ToList();
            return Ok(disciplinesDtoList);
        }


        [HttpGet("{uuid}")]
        [SwaggerResponse(200, "Дисциплина найдена", typeof(DisciplinesDto))]
        [SwaggerResponse(404, "Дисциплина не найдена", typeof(ApiError))]
        [SwaggerResponseExample(404, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить дисциплину по идентификатору",
            Description = "Возвращает одну дисциплину по её uuid"
        )]
        public async Task<ActionResult<DisciplinesDto>> GetDiscipline(
            [SwaggerParameter("UUID дисциплины")]
            Guid uuid)
        {
            Disciplines? discipline = await _context.Disciplines.FindAsync(uuid);

            if (discipline == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "0.0",
                    Title = "Дисциплина не найдена",
                    Message = $"Дисциплина с UUID \"{uuid}\" не найдена"
                });
            }

            return Ok(new DisciplinesDto(discipline));
        }


        [HttpPost]
        [SwaggerResponse(201, "Дисциплина успешно создана", typeof(DisciplinesDto))]
        [SwaggerResponse(400, "Некорректные данные для создания дисциплины", typeof(ApiError))]
        [SwaggerResponseExample(400, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Создать новую дисциплину"
        )]
        public async Task<ActionResult<DisciplinesDto>> CreateDiscipline(
            [FromBody, SwaggerParameter("Данные новой дисциплины")]
            DisciplinesDto disciplinesDto
        )
        {
            if (disciplinesDto == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.0",
                    Title = "Пустой объект",
                    Message = "Данные дисциплины не предоставлены"
                });
            }

            if (string.IsNullOrWhiteSpace(disciplinesDto.Name))
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "1.0",
                    Title = "Некорректные данные",
                    Message = "Дисциплина должна иметь имя"
                });
            }


            if (disciplinesDto.SemesterUuid == Guid.Empty)
            {
                return BadRequest(new ApiError { StatusCode = "1.1", Title = "Некорректные данные", Message = "SemesterUuid обязателен" });
            }
            Semesters? semester = await _context.Semesters.FirstOrDefaultAsync(s => s.Uuid == disciplinesDto.SemesterUuid);
            if (semester == null)
            {
                return BadRequest(new ApiError { StatusCode = "1.2", Title = "Некорректные данные", Message = "Семестр с указанным UUID не найден" });
            }

            if (disciplinesDto.AcademicYearUuid == Guid.Empty)
            {
                return BadRequest(new ApiError { StatusCode = "1.3", Title = "Некорректные данные", Message = "AcademicYearUuid обязателен" });
            }
            AcademicYears? academicYear = await _context.AcademicYears.FirstOrDefaultAsync(a => a.Uuid == disciplinesDto.AcademicYearUuid);
            if (academicYear == null)
            {
                return BadRequest(new ApiError { StatusCode = "1.4", Title = "Некорректные данные", Message = "Учебный год с указанным UUID не найден" });
            }

            DisciplinesRegisters? disciplineRegister = null;
            if (disciplinesDto.DisciplineRegisterUuid != Guid.Empty)
            {
                disciplineRegister = await _context.DisciplinesRegisters.FirstOrDefaultAsync(r => r.Uuid == disciplinesDto.DisciplineRegisterUuid);
                if (disciplineRegister == null)
                {
                    return BadRequest(new ApiError { StatusCode = "1.5", Title = "Некорректные данные", Message = "Реестр дисциплин с указанным UUID не найден" });
                }
            }

            Disciplines newDiscipline = new()
            {
                Uuid = Guid.NewGuid(),
                Name = disciplinesDto.Name.Trim(),
                ShortName = string.IsNullOrWhiteSpace(disciplinesDto.ShortName) ? disciplinesDto.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(w => w[0]).Aggregate("", (a, c) => a + c) : disciplinesDto.ShortName.Trim(),
                IsArchived = disciplinesDto.IsArchived,
                DisciplineRegisterId = disciplineRegister?.DisciplineRegisterId,
                SemesterId = semester.SemesterId,
                AcademicYearId = academicYear.AcademicYearId
            };

            _context.Disciplines.Add(newDiscipline);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetDiscipline),
                new { uuid = newDiscipline.Uuid },
                new DisciplinesDto(newDiscipline)
            );
        }
    }

}