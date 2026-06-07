using MainService;
using MainService.Controllers;
using MainService.Errors;
using MainService.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace MainService.Tests.Controllers;

public class StudentsControllerTests : IDisposable
{
    private readonly MainServiceContext _context;
    private readonly StudentsController _controller;

    public StudentsControllerTests()
    {
        _context = TestDbContextFactory.Create();
        _controller = new StudentsController(_context);
    }

    [Fact(DisplayName = "Получение студентов при пустой базе данных")]
    public async Task GetStudents_WhenEmpty_ReturnsNotFound()
    {
        ActionResult<PagedResult<StudentsResponseDto>> result = await _controller.GetStudents();

        NotFoundObjectResult notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        ApiError error = Assert.IsType<ApiError>(notFound.Value);
        Assert.Equal("1.0.3", error.StatusCode);
    }

    [Fact(DisplayName = "Получение студентов при наличии")]
    public async Task GetStudents_WhenDataExists_ReturnsOk()
    {
        await TestDataMock.MockStudentAsync(_context);

        ActionResult<PagedResult<StudentsResponseDto>> result = await _controller.GetStudents();

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result.Result);
        PagedResult<StudentsResponseDto> paged = Assert.IsType<PagedResult<StudentsResponseDto>>(ok.Value);
        Assert.Single(paged.Items);
        Assert.Equal("Иван", paged.Items.First().FirstName);
        Assert.Equal("Иванов", paged.Items.First().LastName);
    }

    [Theory(DisplayName = "Получение студентов при отрицательном смещении")]
    [InlineData(-1)]
    public async Task GetStudents_WhenOffsetNegative_ReturnsBadRequest(int offset)
    {
        ActionResult<PagedResult<StudentsResponseDto>> result = await _controller.GetStudents(offset: offset);

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        ApiError error = Assert.IsType<ApiError>(badRequest.Value);
        Assert.Equal("0.2.1", error.StatusCode);
        Assert.Equal("offset", error.Field);
    }

    [Fact(DisplayName = "Получение студента при пустом UUID")]
    public async Task GetStudent_WhenUuidEmpty_ReturnsBadRequest()
    {
        IActionResult result = await _controller.GetStudent(Guid.Empty);

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
        ApiError error = Assert.IsType<ApiError>(badRequest.Value);
        Assert.Equal("0.2.0", error.StatusCode);
    }

    [Fact(DisplayName = "Получение студентов при несуществующем UUID группы")]
    public async Task GetStudentsByGroup_WhenGroupNotFound_ReturnsNotFound()
    {
        ActionResult<PagedResult<StudentsResponseDto>> result =
            await _controller.GetStudentsByGroup(Guid.NewGuid());

        NotFoundObjectResult notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        ApiError error = Assert.IsType<ApiError>(notFound.Value);
        Assert.Equal("1.2.3", error.StatusCode);
    }

    [Fact(DisplayName = "Получение студентов при существующем UUID группы")]
    public async Task GetStudentsByGroup_WhenStudentsExist_ReturnsOk()
    {
        Students student = await TestDataMock.MockStudentAsync(_context);

        ActionResult<PagedResult<StudentsResponseDto>> result =
            await _controller.GetStudentsByGroup(student.Group!.Uuid);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result.Result);
        PagedResult<StudentsResponseDto> paged = Assert.IsType<PagedResult<StudentsResponseDto>>(ok.Value);
        Assert.Single(paged.Items);
        Assert.Equal(student.Uuid, paged.Items.First().Uuid);
    }

	public void Dispose()
	{
		_context.Dispose();
	}
}
