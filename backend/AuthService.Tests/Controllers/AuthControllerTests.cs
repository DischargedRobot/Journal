using System.Diagnostics;
using AuthService;
using AuthService.Controller;
using AuthService.Errors;
using AuthService.Lib.Utils;
using AuthService.Model;
using AuthService.Model.Auth.Dto;
using AuthService.Redis;
using AuthService.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace AuthService.Tests.Controllers;

public class AuthControllerTests : IDisposable
{
    private readonly AuthTestContext _testContext;
    private readonly AuthServiceContext _context;
    private readonly InMemoryTokenStore _accessTokenList;
    private readonly InMemoryTokenBlackListStore _accessTokenBlackList;
    private readonly InMemoryTokenBlackListStore _refreshTokenBlackList;
    private readonly TokenService _tokenService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _testContext = TestDbContextFactory.Create();
        _context = _testContext.Context;
        _accessTokenList = new InMemoryTokenStore();
        _accessTokenBlackList = new InMemoryTokenBlackListStore();
        _refreshTokenBlackList = new InMemoryTokenBlackListStore();
        _tokenService = AuthTestHelper.CreateTokenService();
        _controller = new AuthController(
            NullLogger<AuthController>.Instance,
            _context,
            _refreshTokenBlackList,
            _accessTokenList,
            _accessTokenBlackList,
            _tokenService,
            new ActivitySource("AuthService.Tests"));
        TestControllerHelper.SetupContext(_controller, nameof(AuthController));
    }

    [Fact(DisplayName = "Вход при пустом логине или пароле")]
    public async Task Login_WhenCredentialsEmpty_ReturnsBadRequest()
    {
        ActionResult<LoginResponse> result = await _controller.Login(new LoginRequest
        {
            Login = "   ",
            Password = "password123"
        });

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        ApiError error = Assert.IsType<ApiError>(badRequest.Value);
        Assert.Equal("0.2.0", error.StatusCode);
    }

    [Fact(DisplayName = "Вход при неверном пароле")]
    public async Task Login_WhenWrongPassword_ReturnsUnauthorized()
    {
        await TestDataMock.MockUserAsync(_context);

        ActionResult<LoginResponse> result = await _controller.Login(new LoginRequest
        {
            Login = "ivanov",
            Password = "wrong-password"
        });

        UnauthorizedObjectResult unauthorized = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        ApiError error = Assert.IsType<ApiError>(unauthorized.Value);
        Assert.Equal("1.2.3", error.StatusCode);
    }

    [Fact(DisplayName = "Вход при несуществующем пользователе")]
    public async Task Login_WhenUserNotFound_ReturnsUnauthorized()
    {
        ActionResult<LoginResponse> result = await _controller.Login(new LoginRequest
        {
            Login = "unknown",
            Password = "password123"
        });

        UnauthorizedObjectResult unauthorized = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        ApiError error = Assert.IsType<ApiError>(unauthorized.Value);
        Assert.Equal("1.2.3", error.StatusCode);
    }

    [Fact(DisplayName = "Вход при валидных данных")]
    public async Task Login_WhenValid_ReturnsOkWithAccessToken()
    {
        await TestDataMock.MockUserAsync(_context);

        ActionResult<LoginResponse> result = await _controller.Login(new LoginRequest
        {
            Login = "ivanov",
            Password = "password123"
        });

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result.Result);
        LoginResponse response = Assert.IsType<LoginResponse>(ok.Value);
        Assert.False(string.IsNullOrWhiteSpace(response.AccessToken));
        Assert.True(Guid.TryParse(response.AccessToken, out Guid tokenUuid));
        Assert.NotNull(await _accessTokenList.GetAsync(tokenUuid));
    }

    [Fact(DisplayName = "Регистрация при пустом теле запроса")]
    public async Task Register_WhenRequestNull_ReturnsBadRequest()
    {
        IActionResult result = await _controller.Register(null);

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
        ApiError error = Assert.IsType<ApiError>(badRequest.Value);
        Assert.Equal("0.1.0", error.StatusCode);
        Assert.Equal("BODY", error.Field);
    }

    [Fact(DisplayName = "Регистрация при пустом логине или пароле")]
    public async Task Register_WhenLoginOrPasswordEmpty_ReturnsBadRequest()
    {
        IActionResult result = await _controller.Register(new UsersCreateDto
        {
            Login = "   ",
            Password = "password123",
            FirstName = "Иван",
            LastName = "Иванов"
        });

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
        ApiError error = Assert.IsType<ApiError>(badRequest.Value);
        Assert.Equal("0.2.0", error.StatusCode);
    }

    [Fact(DisplayName = "Регистрация при пустом имени")]
    public async Task Register_WhenFirstNameEmpty_ReturnsBadRequest()
    {
        IActionResult result = await _controller.Register(new UsersCreateDto
        {
            Login = "newuser",
            Password = "password123",
            FirstName = "   ",
            LastName = "Иванов"
        });

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
        ApiError error = Assert.IsType<ApiError>(badRequest.Value);
        Assert.Equal("0.2.1", error.StatusCode);
        Assert.Equal("FirstName", error.Field);
    }

    [Fact(DisplayName = "Регистрация при дублировании логина")]
    public async Task Register_WhenDuplicateLogin_ReturnsConflict()
    {
        await TestDataMock.MockUserAsync(_context);

        IActionResult result = await _controller.Register(new UsersCreateDto
        {
            Login = "ivanov",
            Password = "password123",
            FirstName = "Иван",
            LastName = "Иванов"
        });

        ConflictObjectResult conflict = Assert.IsType<ConflictObjectResult>(result);
        ApiError error = Assert.IsType<ApiError>(conflict.Value);
        Assert.Equal("1.1.1", error.StatusCode);
    }

    [Fact(DisplayName = "Регистрация при валидных данных")]
    public async Task Register_WhenValid_ReturnsCreated()
    {
        IActionResult result = await _controller.Register(new UsersCreateDto
        {
            Login = "petrov",
            Password = "password123",
            FirstName = "Пётр",
            LastName = "Петров"
        });

        CreatedResult created = Assert.IsType<CreatedResult>(result);
        UsersResponseDto dto = Assert.IsType<UsersResponseDto>(created.Value);
        Assert.Equal("petrov", dto.Login);
        Assert.False(string.IsNullOrWhiteSpace(_controller.Response.Headers.Authorization.ToString()));
    }

    [Fact(DisplayName = "Проверка токена при отсутствии заголовка Authorization")]
    public async Task CheckAuthtoken_WhenNoHeader_ReturnsBadRequest()
    {
        IActionResult result = await _controller.CheckAuthtoken();

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
        ApiError error = Assert.IsType<ApiError>(badRequest.Value);
        Assert.Equal("2.4.0", error.StatusCode);
        Assert.Equal("Authorization", error.Field);
    }

    [Fact(DisplayName = "Проверка токена при неверном формате заголовка")]
    public async Task CheckAuthtoken_WhenInvalidHeaderFormat_ReturnsBadRequest()
    {
        _controller.Request.Headers.Authorization = "Token abc";

        IActionResult result = await _controller.CheckAuthtoken();

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
        ApiError error = Assert.IsType<ApiError>(badRequest.Value);
        Assert.Equal("2.4.2", error.StatusCode);
    }

    [Fact(DisplayName = "Проверка токена при недействительном токене")]
    public async Task CheckAuthtoken_WhenTokenInvalid_ReturnsUnauthorized()
    {
        TestControllerHelper.SetAuthorizationHeader(_controller, Guid.NewGuid().ToString());

        IActionResult result = await _controller.CheckAuthtoken();

        UnauthorizedObjectResult unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        ApiError error = Assert.IsType<ApiError>(unauthorized.Value);
        Assert.Equal("2.2.2", error.StatusCode);
    }

    [Fact(DisplayName = "Проверка токена при валидном токене")]
    public async Task CheckAuthtoken_WhenTokenValid_ReturnsOk()
    {
        Users user = await TestDataMock.MockUserAsync(_context);
        string opaqueToken = await AuthTestHelper.IssueOpaqueTokenAsync(_tokenService, _accessTokenList, user.Uuid);
        TestControllerHelper.SetAuthorizationHeader(_controller, opaqueToken);

        IActionResult result = await _controller.CheckAuthtoken();

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    [Fact(DisplayName = "Проверка токена при отозванном токене")]
    public async Task CheckAuthtoken_WhenTokenRevoked_ReturnsUnauthorized()
    {
        Users user = await TestDataMock.MockUserAsync(_context);
        Guid tokenUuid = Guid.NewGuid();
        string accessToken = _tokenService.GenerateAccessToken(tokenUuid, user.Uuid, []);
        string opaqueToken = _tokenService.GenerateOpaqueToken(tokenUuid);
        await _accessTokenList.SaveAsync(tokenUuid, accessToken, TimeSpan.FromMinutes(30));
        await _accessTokenBlackList.SaveAsync(tokenUuid, user.Uuid, TimeSpan.FromMinutes(30));
        TestControllerHelper.SetAuthorizationHeader(_controller, opaqueToken);

        IActionResult result = await _controller.CheckAuthtoken();

        UnauthorizedObjectResult unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        ApiError error = Assert.IsType<ApiError>(unauthorized.Value);
        Assert.Equal("2.2.2", error.StatusCode);
    }

    [Fact(DisplayName = "Выход при отсутствии заголовка Authorization")]
    public async Task Logout_WhenNoAuthHeader_ReturnsBadRequest()
    {
        IActionResult result = await _controller.Logout();

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
        ApiError error = Assert.IsType<ApiError>(badRequest.Value);
        Assert.Equal("2.3.0", error.StatusCode);
    }

    [Fact(DisplayName = "Обновление токена при отсутствии refresh cookie")]
    public async Task Refresh_WhenNoCookie_ReturnsUnauthorized()
    {
        IActionResult result = await _controller.Refresh();

        UnauthorizedObjectResult unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        ApiError error = Assert.IsType<ApiError>(unauthorized.Value);
        Assert.Equal("2.4.0", error.StatusCode);
    }

    public void Dispose()
    {
        _testContext.Dispose();
    }
}
