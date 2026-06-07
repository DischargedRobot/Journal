using MainService;
using MainService.Controllers;
using MainService.Errors;
using MainService.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MainService.Tests.Controllers;

public class FacultiesControllerTests : IDisposable
{
    private readonly MainServiceContext _context;
    private readonly FacultiesController _controller;

    public FacultiesControllerTests()
    {
        _context = TestDbContextFactory.Create();
        _controller = new FacultiesController(_context);
    }

    [Fact(DisplayName = "Получение факультетов при пустой базе данных")]
    public async Task GetFaculties_WhenEmpty_ReturnsNotFound()
    {
		ActionResult<PagedResult<FacultiesResponseDto>> result = await _controller.GetFaculties();

		NotFoundObjectResult notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
		ApiError error = Assert.IsType<ApiError>(notFound.Value);
        Assert.Equal("1.0.3", error.StatusCode);
    }

    [Fact(DisplayName = "Получение факультетов при наличии")]
    public async Task GetFaculties_WhenDataExists_ReturnsOk()
    {
        await TestDataMock.MockFacultyAsync(_context);

		ActionResult<PagedResult<FacultiesResponseDto>> result = await _controller.GetFaculties();

		OkObjectResult ok = Assert.IsType<OkObjectResult>(result.Result);
		PagedResult<FacultiesResponseDto> paged = Assert.IsType<PagedResult<FacultiesResponseDto>>(ok.Value);
        Assert.Single(paged.Items);
        Assert.Equal("Факультет информатики", paged.Items.First().Name);
    }

    [Theory(DisplayName = "Получение факультетов при отрицательном смещении")]
    [InlineData(-1)]
    public async Task GetFaculties_WhenOffsetNegative_ReturnsBadRequest(int offset)
    {
		ActionResult<PagedResult<FacultiesResponseDto>> result = await _controller.GetFaculties(offset: offset);

		BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
		ApiError error = Assert.IsType<ApiError>(badRequest.Value);
        Assert.Equal("0.2.0", error.StatusCode);
        Assert.Equal("offset", error.Field);
    }

    [Fact(DisplayName = "Получение факультета при пустом UUID")]
    public async Task GetFaculty_WhenUuidEmpty_ReturnsBadRequest()
    {
		ActionResult<FacultiesResponseDto> result = await _controller.GetFaculty(Guid.Empty);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact(DisplayName = "Получение факультета при несуществующем UUID")]
    public async Task GetFaculty_WhenNotFound_ReturnsNotFound()
    {
		ActionResult<FacultiesResponseDto> result = await _controller.GetFaculty(Guid.NewGuid());

		NotFoundObjectResult notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
		ApiError error = Assert.IsType<ApiError>(notFound.Value);
        Assert.Equal("1.2.3", error.StatusCode);
    }

    [Fact(DisplayName = "Создание факультета при пустом названии")]
    public async Task CreateFaculty_WhenNameEmpty_ReturnsBadRequest()
    {
		ActionResult<FacultiesResponseDto> result = await _controller.CreateFaculty(
            new FacultiesCreateDto { Name = "   " });

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact(DisplayName = "Создание факультета при валидных данных")]
    public async Task CreateFaculty_WhenValid_ReturnsCreated()
    {
		ActionResult<FacultiesResponseDto> result = await _controller.CreateFaculty(
            new FacultiesCreateDto { Name = "Факультет информатики" });

		CreatedAtActionResult created = Assert.IsType<CreatedAtActionResult>(result.Result);
		FacultiesResponseDto dto = Assert.IsType<FacultiesResponseDto>(created.Value);
        Assert.Equal("Факультет информатики", dto.Name);
    }

    [Fact(DisplayName = "Создание факультета при дублировании названия")]
    public async Task CreateFaculty_WhenDuplicateName_ReturnsConflict()
    {
        _context.Faculties.Add(new Faculties
        {
            Name = "Факультет информатики",
            ShortName = "ФИ"
        });
        await _context.SaveChangesAsync();

		ActionResult<FacultiesResponseDto> result = await _controller.CreateFaculty(
            new FacultiesCreateDto { Name = "Факультет информатики" });

        Assert.IsType<ConflictObjectResult>(result.Result);
    }

	public void Dispose()
	{
		_context.Dispose();
	}
}