using System.Diagnostics;
using AuthService;
using AuthService.Controller;
using AuthService.Errors;
using AuthService.Model;
using AuthService.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace AuthService.Tests.Controllers;

public class UsersControllerTests : IDisposable
{
    private readonly AuthTestContext _testContext;
    private readonly AuthServiceContext _context;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _testContext = TestDbContextFactory.Create();
        _context = _testContext.Context;
        _controller = new UsersController(
            _context,
            NullLogger<UsersController>.Instance,
            new ActivitySource("AuthService.Tests"));
        TestControllerHelper.SetupContext(_controller, nameof(UsersController));
    }

    [Fact(DisplayName = "Получение пользователей при пустой базе данных")]
    public async Task GetUsers_WhenEmpty_ReturnsNotFound()
    {
        ActionResult<PagedResult<UsersResponseDto>> result = await _controller.GetUsers();

        NotFoundObjectResult notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        ApiError error = Assert.IsType<ApiError>(notFound.Value);
        Assert.Equal("1.0.3", error.StatusCode);
    }

    [Fact(DisplayName = "Получение пользователей при наличии")]
    public async Task GetUsers_WhenDataExists_ReturnsOk()
    {
        await TestDataMock.MockUserAsync(_context);

        ActionResult<PagedResult<UsersResponseDto>> result = await _controller.GetUsers();

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result.Result);
        PagedResult<UsersResponseDto> paged = Assert.IsType<PagedResult<UsersResponseDto>>(ok.Value);
        Assert.Single(paged.Items);
        Assert.Equal("ivanov", paged.Items.First().Login);
    }

    [Theory(DisplayName = "Получение пользователей при отрицательном смещении")]
    [InlineData(-1)]
    public async Task GetUsers_WhenOffsetNegative_ReturnsBadRequest(int offset)
    {
        ActionResult<PagedResult<UsersResponseDto>> result = await _controller.GetUsers(offset: offset);

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        ApiError error = Assert.IsType<ApiError>(badRequest.Value);
        Assert.Equal("0.2.1", error.StatusCode);
        Assert.Equal("offset", error.Field);
    }

    [Fact(DisplayName = "Получение пользователя при несуществующем UUID")]
    public async Task GetUser_WhenNotFound_ReturnsNotFound()
    {
        ActionResult<UsersResponseDto> result = await _controller.GetUser(Guid.NewGuid());

        NotFoundObjectResult notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        ApiError error = Assert.IsType<ApiError>(notFound.Value);
        Assert.Equal("1.2.3", error.StatusCode);
    }

    [Fact(DisplayName = "Получение пользователя при существующем UUID")]
    public async Task GetUser_WhenExists_ReturnsOk()
    {
        Users user = await TestDataMock.MockUserAsync(_context);

        ActionResult<UsersResponseDto> result = await _controller.GetUser(user.Uuid);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result.Result);
        UsersResponseDto dto = Assert.IsType<UsersResponseDto>(ok.Value);
        Assert.Equal(user.Uuid, dto.Uuid);
        Assert.Equal("ivanov", dto.Login);
    }

    [Fact(DisplayName = "Создание пользователя при пустом логине")]
    public async Task CreateUser_WhenLoginEmpty_ReturnsBadRequest()
    {
        ActionResult<UsersResponseDto> result = await _controller.CreateUser(new UsersCreateDto
        {
            Login = "   ",
            Password = "password123",
            FirstName = "Иван",
            LastName = "Иванов"
        });

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact(DisplayName = "Создание пользователя при валидных данных")]
    public async Task CreateUser_WhenValid_ReturnsCreated()
    {
        ActionResult<UsersResponseDto> result = await _controller.CreateUser(new UsersCreateDto
        {
            Login = "petrov",
            Password = "password123",
            FirstName = "Пётр",
            LastName = "Петров"
        });

        CreatedAtActionResult created = Assert.IsType<CreatedAtActionResult>(result.Result);
        UsersResponseDto dto = Assert.IsType<UsersResponseDto>(created.Value);
        Assert.Equal("petrov", dto.Login);
    }

    [Fact(DisplayName = "Создание пользователя при дублировании логина")]
    public async Task CreateUser_WhenDuplicateLogin_ReturnsConflict()
    {
        await TestDataMock.MockUserAsync(_context);

        ActionResult<UsersResponseDto> result = await _controller.CreateUser(new UsersCreateDto
        {
            Login = "ivanov",
            Password = "password123",
            FirstName = "Иван",
            LastName = "Иванов"
        });

        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    public void Dispose()
    {
        _testContext.Dispose();
    }
}
