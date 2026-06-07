using MainService;
using MainService.Controllers;
using MainService.Errors;
using MainService.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace MainService.Tests.Controllers;

public class DisciplinesControllerTests : IDisposable
{
    private readonly MainServiceContext _context;
    private readonly DisciplinesController _controller;

    public DisciplinesControllerTests()
    {
        _context = TestDbContextFactory.Create();
        _controller = new DisciplinesController(_context);
    }

    [Fact(DisplayName = "Получение дисциплин при пустой базе данных")]
    public async Task GetDisciplines_WhenEmpty_ReturnsNotFound()
    {
        ActionResult<PagedResult<DisciplinesResponseDto>> result = await _controller.GetDisciplines();

        NotFoundObjectResult notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        ApiError error = Assert.IsType<ApiError>(notFound.Value);
        Assert.Equal("1.0.3", error.StatusCode);
    }

    [Fact(DisplayName = "Получение дисциплин при наличии")]
    public async Task GetDisciplines_WhenDataExists_ReturnsOk()
    {
        await TestDataMock.MockDisciplineAsync(_context);

        ActionResult<PagedResult<DisciplinesResponseDto>> result = await _controller.GetDisciplines();

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result.Result);
        PagedResult<DisciplinesResponseDto> paged = Assert.IsType<PagedResult<DisciplinesResponseDto>>(ok.Value);
        Assert.Single(paged.Items);
        Assert.Equal("Математический анализ", paged.Items.First().Name);
    }

    [Theory(DisplayName = "Получение дисциплин при отрицательном смещении")]
    [InlineData(-1)]
    public async Task GetDisciplines_WhenOffsetNegative_ReturnsBadRequest(int offset)
    {
        ActionResult<PagedResult<DisciplinesResponseDto>> result =
            await _controller.GetDisciplines(offset: offset);

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        ApiError error = Assert.IsType<ApiError>(badRequest.Value);
        Assert.Equal("0.2.0", error.StatusCode);
        Assert.Equal("offset", error.Field);
    }

    [Fact(DisplayName = "Получение дисциплины при пустом UUID")]
    public async Task GetDiscipline_WhenUuidEmpty_ReturnsBadRequest()
    {
        ActionResult<DisciplinesResponseDto> result = await _controller.GetDiscipline(Guid.Empty);

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        ApiError error = Assert.IsType<ApiError>(badRequest.Value);
        Assert.Equal("0.2.0", error.StatusCode);
    }

    [Fact(DisplayName = "Получение дисциплины при несуществующем UUID")]
    public async Task GetDiscipline_WhenNotFound_ReturnsNotFound()
    {
        ActionResult<DisciplinesResponseDto> result = await _controller.GetDiscipline(Guid.NewGuid());

        NotFoundObjectResult notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        ApiError error = Assert.IsType<ApiError>(notFound.Value);
        Assert.Equal("1.2.3", error.StatusCode);
    }

    [Fact(DisplayName = "Получение дисциплины при существующем UUID")]
    public async Task GetDiscipline_WhenExists_ReturnsOk()
    {
        Disciplines discipline = await TestDataMock.MockDisciplineAsync(_context);

        ActionResult<DisciplinesResponseDto> result = await _controller.GetDiscipline(discipline.Uuid);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result.Result);
        DisciplinesResponseDto dto = Assert.IsType<DisciplinesResponseDto>(ok.Value);
        Assert.Equal(discipline.Uuid, dto.Uuid);
        Assert.Equal("Математический анализ", dto.Name);
    }

    public void Dispose() => _context.Dispose();
}
