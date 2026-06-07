using MainService;
using MainService.Controllers;
using MainService.Errors;
using MainService.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace MainService.Tests.Controllers;

public class GroupsControllerTests : IDisposable
{
    private readonly MainServiceContext _context;
    private readonly GroupsController _controller;

    public GroupsControllerTests()
    {
        _context = TestDbContextFactory.Create();
        _controller = new GroupsController(_context);
    }

    [Fact(DisplayName = "Получение групп при пустой базе данных")]
    public async Task GetGroups_WhenEmpty_ReturnsNotFound()
    {
        ActionResult<IEnumerable<GroupsResponseDto>> result = await _controller.GetGroups();

        NotFoundObjectResult notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        ApiError error = Assert.IsType<ApiError>(notFound.Value);
        Assert.Equal("1.0.3", error.StatusCode);
    }

    [Fact(DisplayName = "Получение групп при наличии")]
    public async Task GetGroups_WhenDataExists_ReturnsOk()
    {
        await TestDataMock.MockGroupAsync(_context);

        ActionResult<IEnumerable<GroupsResponseDto>> result = await _controller.GetGroups();

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result.Result);
        IEnumerable<GroupsResponseDto> groups = Assert.IsAssignableFrom<IEnumerable<GroupsResponseDto>>(ok.Value);
        Assert.Single(groups);
        Assert.Equal("ИВТ-101", groups.First().Code);
    }

    [Fact(DisplayName = "Получение группы при пустом UUID")]
    public async Task GetGroupByUuid_WhenUuidEmpty_ReturnsBadRequest()
    {
        ActionResult<GroupsResponseDto> result = await _controller.GetGroupByUuid(Guid.Empty);

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        ApiError error = Assert.IsType<ApiError>(badRequest.Value);
        Assert.Equal("0.2.0", error.StatusCode);
    }

    [Fact(DisplayName = "Получение группы при несуществующем UUID")]
    public async Task GetGroupByUuid_WhenNotFound_ReturnsNotFound()
    {
        ActionResult<GroupsResponseDto> result = await _controller.GetGroupByUuid(Guid.NewGuid());

        NotFoundObjectResult notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        ApiError error = Assert.IsType<ApiError>(notFound.Value);
        Assert.Equal("1.2.3", error.StatusCode);
    }

    [Fact(DisplayName = "Получение группы при существующем UUID")]
    public async Task GetGroupByUuid_WhenExists_ReturnsOk()
    {
        Groups group = await TestDataMock.MockGroupAsync(_context);

        ActionResult<GroupsResponseDto> result = await _controller.GetGroupByUuid(group.Uuid);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result.Result);
        GroupsResponseDto dto = Assert.IsType<GroupsResponseDto>(ok.Value);
        Assert.Equal(group.Uuid, dto.Uuid);
        Assert.Equal("ИВТ-101", dto.Code);
    }

    [Fact(DisplayName = "Создание группы при пустом коде")]
    public async Task CreateGroup_WhenCodeEmpty_ReturnsBadRequest()
    {
        ActionResult<GroupsResponseDto> result = await _controller.CreateGroup(new GroupsCreateDto
        {
            Code = "   ",
            TrainingDirectionUuid = Guid.NewGuid(),
            FacultyUuid = Guid.NewGuid()
        });

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        ApiError error = Assert.IsType<ApiError>(badRequest.Value);
        Assert.Equal("0.2.0", error.StatusCode);
        Assert.Equal(nameof(GroupsCreateDto.Code), error.Field);
    }

    [Fact(DisplayName = "Создание группы при пустом UUID направления обучения")]
    public async Task CreateGroup_WhenTrainingDirectionUuidEmpty_ReturnsBadRequest()
    {
        ActionResult<GroupsResponseDto> result = await _controller.CreateGroup(new GroupsCreateDto
        {
            Code = "ИВТ-202",
            TrainingDirectionUuid = Guid.Empty,
            FacultyUuid = Guid.NewGuid()
        });

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        ApiError error = Assert.IsType<ApiError>(badRequest.Value);
        Assert.Equal(nameof(GroupsCreateDto.TrainingDirectionUuid), error.Field);
    }

    [Fact(DisplayName = "Создание группы при валидных данных")]
    public async Task CreateGroup_WhenValid_ReturnsCreated()
    {
        Faculties faculty = await TestDataMock.MockFacultyAsync(_context);
        TrainingDirections trainingDirection = await TestDataMock.MockTrainingDirectionAsync(_context);

        ActionResult<GroupsResponseDto> result = await _controller.CreateGroup(new GroupsCreateDto
        {
            Code = "ИВТ-202",
            TrainingDirectionUuid = trainingDirection.Uuid,
            FacultyUuid = faculty.Uuid
        });

        CreatedAtActionResult created = Assert.IsType<CreatedAtActionResult>(result.Result);
        GroupsResponseDto dto = Assert.IsType<GroupsResponseDto>(created.Value);
        Assert.Equal("ИВТ-202", dto.Code);
    }

	public void Dispose()
	{
		_context.Dispose();
	}
}
