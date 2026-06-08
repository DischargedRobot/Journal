using System.Diagnostics;
using AuthService;
using AuthService.Controller;
using AuthService.Errors;
using AuthService.Model;
using AuthService.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace AuthService.Tests.Controllers;

public class SessionsControllerTests : IDisposable
{
    private readonly AuthTestContext _testContext;
    private readonly AuthServiceContext _context;
    private readonly SessionsController _controller;

    public SessionsControllerTests()
    {
        _testContext = TestDbContextFactory.Create();
        _context = _testContext.Context;
        _controller = new SessionsController(
            _context,
            NullLogger<SessionsController>.Instance,
            new ActivitySource("AuthService.Tests"));
        TestControllerHelper.SetupContext(_controller, nameof(SessionsController));
    }

    [Fact(DisplayName = "Получение сессий при пустой базе данных")]
    public async Task GetSessions_WhenEmpty_ReturnsNotFound()
    {
        ActionResult<PagedResult<SessionsResponseDto>> result = await _controller.GetSessions();

        NotFoundObjectResult notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        ApiError error = Assert.IsType<ApiError>(notFound.Value);
        Assert.Equal("1.0.3", error.StatusCode);
    }

    [Fact(DisplayName = "Получение сессий при наличии")]
    public async Task GetSessions_WhenDataExists_ReturnsOk()
    {
        await TestDataMock.MockSessionAsync(_context);

        ActionResult<PagedResult<SessionsResponseDto>> result = await _controller.GetSessions();

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result.Result);
        PagedResult<SessionsResponseDto> paged = Assert.IsType<PagedResult<SessionsResponseDto>>(ok.Value);
        Assert.Single(paged.Items);
        Assert.Equal("Chrome", paged.Items.First().BrowserName);
    }

    [Theory(DisplayName = "Получение сессий при отрицательном смещении")]
    [InlineData(-1)]
    public async Task GetSessions_WhenOffsetNegative_ReturnsBadRequest(int offset)
    {
        ActionResult<PagedResult<SessionsResponseDto>> result = await _controller.GetSessions(offset: offset);

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        ApiError error = Assert.IsType<ApiError>(badRequest.Value);
        Assert.Equal("0.2.1", error.StatusCode);
        Assert.Equal("offset", error.Field);
    }

    [Fact(DisplayName = "Получение сессии при пустом UUID")]
    public async Task GetSession_WhenUuidEmpty_ReturnsBadRequest()
    {
        IActionResult result = await _controller.GetSession(Guid.Empty);

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
        ApiError error = Assert.IsType<ApiError>(badRequest.Value);
        Assert.Equal("0.2.1", error.StatusCode);
        Assert.Equal("refreshTokenUuid", error.Field);
    }

    [Fact(DisplayName = "Получение сессии при несуществующем UUID")]
    public async Task GetSession_WhenNotFound_ReturnsNotFound()
    {
        IActionResult result = await _controller.GetSession(Guid.NewGuid());

        NotFoundObjectResult notFound = Assert.IsType<NotFoundObjectResult>(result);
        ApiError error = Assert.IsType<ApiError>(notFound.Value);
        Assert.Equal("1.2.3", error.StatusCode);
    }

    [Fact(DisplayName = "Создание сессии при несуществующем пользователе")]
    public async Task CreateSession_WhenUserNotFound_ReturnsBadRequest()
    {
        IActionResult result = await _controller.CreateSession(new SessionsCreateDto
        {
            UserUuid = Guid.NewGuid(),
            RefreshTokenUuid = Guid.NewGuid(),
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
        ApiError error = Assert.IsType<ApiError>(badRequest.Value);
        Assert.Equal("0.2.1", error.StatusCode);
        Assert.Equal("UserUuid", error.Field);
    }

    [Fact(DisplayName = "Создание сессии при валидных данных")]
    public async Task CreateSession_WhenValid_ReturnsCreated()
    {
        Users user = await TestDataMock.MockUserAsync(_context);
        Guid refreshTokenUuid = Guid.NewGuid();

        IActionResult result = await _controller.CreateSession(new SessionsCreateDto
        {
            UserUuid = user.Uuid,
            RefreshTokenUuid = refreshTokenUuid,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            BrowserName = "Firefox"
        });

        CreatedAtActionResult created = Assert.IsType<CreatedAtActionResult>(result);
        SessionsResponseDto dto = Assert.IsType<SessionsResponseDto>(created.Value);
        Assert.Equal(refreshTokenUuid, dto.RefreshTokenUuid);
        Assert.Equal(user.Uuid, dto.UserUuid);
        Assert.Equal("Firefox", dto.BrowserName);
    }

    public void Dispose()
    {
        _testContext.Dispose();
    }
}
