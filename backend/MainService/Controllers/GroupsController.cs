using MainService.Errors;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace MainService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly MainServiceContext _context;

        public GroupsController(MainServiceContext context)
        {
            _context = context;
        }


        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Группы найдены", typeof(IEnumerable<GroupsResponseDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Группы не найдены", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Получить список всех групп",
            Description = "Возвращает список всех групп в системе"
        )]
        public async Task<ActionResult<IEnumerable<GroupsResponseDto>>> GetGroups()
        {
            List<Groups> groups = await _context.Groups
                .Include(g => g.TrainingDirection)
                .Include(g => g.Faculty)
                .Include(g => g.Curators)
                .ToListAsync();

            if (groups.Count == 0)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "0.0.3",
                    Title = "Группы не найдены",
                    Message = "В системе не найдено ни одной группы",
                    Field = string.Empty
                });
            }

            return Ok(groups.Select(g => new GroupsResponseDto(g)));
        }


        [HttpGet("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Группа успешно найдена", typeof(GroupsResponseDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Группа не найдена", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Получить группу по UUID",
            Description = "Возвращает группу с указанным UUID. Если группа не найдена, возвращает 404 Not Found."
        )]
        public async Task<ActionResult<GroupsResponseDto>> GetGroupByUuid(
            [SwaggerParameter("UUID группы")]
            Guid uuid
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID группы не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            Groups? group = await _context.Groups
                .Include(g => g.TrainingDirection)
                .Include(g => g.Faculty)
                .Include(g => g.Curators)
                .FirstOrDefaultAsync(g => g.Uuid == uuid);

            if (group == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "0.0.3",
                    Title = "Группа не найдена",
                    Message = $"Группа с UUID \"{uuid}\" не найдена",
                    Field = nameof(uuid)
                });
            }

            return Ok(new GroupsResponseDto(group));
        }


        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, "Группа успешно создана", typeof(GroupsResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerOperation(
            Summary = "Создать новую группу",
            Description = "Создает новую группу с указанными данными"
        )]
        public async Task<ActionResult<GroupsResponseDto>> CreateGroup(
            [FromBody, SwaggerParameter("Данные новой группы")]
            GroupsCreateDto createDto
        )
        {
            // проверка перед запросом к бд
            if (string.IsNullOrWhiteSpace(createDto.Code))
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "Код группы не может быть пустым",
                    Field = nameof(createDto.Code)
                });
            }

            if (createDto.TrainingDirectionUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "TrainingDirectionUuid не может быть пустым",
                    Field = nameof(createDto.TrainingDirectionUuid)
                });
            }

            if (createDto.FacultyUuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "FacultyUuid не может быть пустым",
                    Field = nameof(createDto.FacultyUuid)
                });
            }

            // Загрузка связанных сущностей из бд
            TrainingDirections? trainingDirection = await _context.TrainingDirections
                .FirstOrDefaultAsync(t => t.Uuid == createDto.TrainingDirectionUuid);
            if (trainingDirection == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.3",
                    Title = "Некорректные данные",
                    Message = "Направление подготовки с указанным UUID не найдено",
                    Field = nameof(createDto.TrainingDirectionUuid)
                });
            }

            Faculties? faculty = await _context.Faculties
                .FirstOrDefaultAsync(f => f.Uuid == createDto.FacultyUuid);
            if (faculty == null)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.3",
                    Title = "Некорректные данные",
                    Message = "Факультет с указанным UUID не найден",
                    Field = nameof(createDto.FacultyUuid)
                });
            }

            List<Professors> curators = [];
            if (createDto.CuratorsUuids != null && createDto.CuratorsUuids.Length > 0)
            {
                curators = await _context.Professors
                    .Where(p => createDto.CuratorsUuids.Contains(p.Uuid))
                    .ToListAsync();
                if (curators.Count != createDto.CuratorsUuids.Length)
                {
                    Guid[] notFound = createDto.CuratorsUuids.Except(curators.Select(p => p.Uuid)).ToArray();
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.3",
                        Title = "Некорректные данные",
                        Message = "Один или несколько кураторов с указанными UUID не найдены",
                        Details = string.Join(", ", notFound),
                        Field = nameof(createDto.CuratorsUuids)
                    });
                }
            }

            // создание сущности (инициализируем кураторов сразу в инициализаторе)
            Groups newGroup = new()
            {
                Uuid = Guid.NewGuid(),
                Code = createDto.Code.Trim(),
                AdmissionDate = createDto.AdmissionDate,
                TrainingDirectionId = trainingDirection.TrainingDirectionId,
                FacultyId = faculty.FacultyId,
                Curators = curators
            };

            _context.Groups.Add(newGroup);
            await _context.SaveChangesAsync();

            newGroup.TrainingDirection = trainingDirection;
            newGroup.Faculty = faculty;

            return CreatedAtAction(
                nameof(GetGroupByUuid),
                new { uuid = newGroup.Uuid },
                new GroupsResponseDto(newGroup)
            );
        }


        [HttpDelete("{uuid}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Группа удалена")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Группа не найдена", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Удалить группу",
            Description = "Удаляет группу по её UUID"
        )]
        public async Task<IActionResult> DeleteGroup(
            [SwaggerParameter("UUID группы")]
            Guid uuid
        )
        {
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID группы не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            Groups? group = await _context.Groups.FirstOrDefaultAsync(g => g.Uuid == uuid);
            if (group == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "0.0.3",
                    Title = "Группа не найдена",
                    Message = $"Группа с UUID \"{uuid}\" не найдена",
                    Field = nameof(uuid)
                });
            }

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPatch("{uuid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Группа обновлена", typeof(GroupsResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Неверный запрос", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ApiError400BadRequestExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Группа не найдена", typeof(ApiError))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ApiError404NotFoundExample))]
        [SwaggerOperation(
            Summary = "Обновить группу",
            Description = "Обновляет данные группы по её UUID. Все поля необязательны"
        )]
        public async Task<ActionResult<GroupsResponseDto>> UpdateGroup(
            [SwaggerParameter("UUID группы")]
            Guid uuid,
            [FromBody, SwaggerParameter("Данные для обновления группы")]
            GroupsUpdateDto updateDto
        )
        {
            // Предварительная валидация без обращения к БД
            if (uuid == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "UUID группы не может быть пустым",
                    Field = nameof(uuid)
                });
            }

            if (updateDto.Code != null && updateDto.Code.Trim() == string.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "Код группы не может быть пустым",
                    Field = nameof(updateDto.Code)
                });
            }

            if (updateDto.TrainingDirectionUuid != null && updateDto.TrainingDirectionUuid.Value == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "TrainingDirectionUuid не может быть пустым",
                    Field = nameof(updateDto.TrainingDirectionUuid)
                });
            }

            if (updateDto.FacultyUuid != null && updateDto.FacultyUuid.Value == Guid.Empty)
            {
                return BadRequest(new ApiError
                {
                    StatusCode = "0.2.0",
                    Title = "Неверный запрос",
                    Message = "FacultyUuid не может быть пустым",
                    Field = nameof(updateDto.FacultyUuid)
                });
            }

            // Загрузка сущности и проверка ссылок в БД после проверки
            Groups? group = await _context.Groups
                .Include(g => g.TrainingDirection)
                .Include(g => g.Faculty)
                .Include(g => g.Curators)
                .FirstOrDefaultAsync(g => g.Uuid == uuid);

            if (group == null)
            {
                return NotFound(new ApiError
                {
                    StatusCode = "0.0.3",
                    Title = "Группа не найдена",
                    Message = $"Группа с UUID \"{uuid}\" не найдена",
                    Field = nameof(uuid)
                });
            }

            if (updateDto.Code != null)
            {
                group.Code = updateDto.Code.Trim();
            }

            if (updateDto.AdmissionDate != null)
            {
                group.AdmissionDate = updateDto.AdmissionDate.Value;
            }

            if (updateDto.TrainingDirectionUuid != null)
            {
                TrainingDirections? trainingDirection = await _context.TrainingDirections
                    .FirstOrDefaultAsync(t => t.Uuid == updateDto.TrainingDirectionUuid.Value);
                if (trainingDirection == null)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.3",
                        Title = "Некорректные данные",
                        Message = "Направление подготовки с указанным UUID не найдено",
                        Field = nameof(updateDto.TrainingDirectionUuid)
                    });
                }

                group.TrainingDirectionId = trainingDirection.TrainingDirectionId;
                group.TrainingDirection = trainingDirection;
            }

            if (updateDto.FacultyUuid != null)
            {
                Faculties? faculty = await _context.Faculties
                    .FirstOrDefaultAsync(f => f.Uuid == updateDto.FacultyUuid.Value);
                if (faculty == null)
                {
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.3",
                        Title = "Некорректные данные",
                        Message = $"Факультет с указанным UUID \"{updateDto.FacultyUuid}\" не найден",
                        Field = nameof(updateDto.FacultyUuid)
                    });
                }
                group.FacultyId = faculty.FacultyId;
                group.Faculty = faculty;
            }

            if (updateDto.CuratorsUuids != null)
            {
                List<Professors> curators = await _context.Professors
                    .Where(p => updateDto.CuratorsUuids.Contains(p.Uuid))
                    .ToListAsync();
                if (curators.Count != updateDto.CuratorsUuids.Length)
                {
                    Guid[] notFound = updateDto.CuratorsUuids.Except(curators.Select(p => p.Uuid)).ToArray();
                    return BadRequest(new ApiError
                    {
                        StatusCode = "0.2.3",
                        Title = "Некорректные данные",
                        Message = $"Один или несколько кураторов с указанными UUID \"{string.Join(", ", notFound)}\" не найдены",
                        Details = string.Join(", ", notFound),
                        Field = nameof(updateDto.CuratorsUuids)
                    });
                }
                group.Curators = curators;
            }

            await _context.SaveChangesAsync();

            return Ok(new GroupsResponseDto(group));
        }
    }
}