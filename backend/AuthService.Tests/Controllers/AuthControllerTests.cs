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
	private readonly IRegistrationCodeStore _registrationCodeStore;
	private readonly TokenService _tokenService;
	private readonly AuthController _controller;

	public AuthControllerTests()
	{
		_testContext = TestDbContextFactory.Create();
		_context = _testContext.Context;
		_accessTokenList = new InMemoryTokenStore();
		_accessTokenBlackList = new InMemoryTokenBlackListStore();
		_refreshTokenBlackList = new InMemoryTokenBlackListStore();
		_registrationCodeStore = new InMemoryRegistrationCodeStore();
		_tokenService = AuthTestHelper.CreateTokenService();
		_controller = new AuthController(
			NullLogger<AuthController>.Instance,
			_context,
			_refreshTokenBlackList,
			_accessTokenList,
			_accessTokenBlackList,
			_tokenService,
			_registrationCodeStore,
			new ActivitySource("AuthService.Tests"));
		TestControllerHelper.SetupContext(_controller, nameof(AuthController));
	}

	[Fact(DisplayName = "Вход при пустом логине или пароле")]
	public async Task Login_WhenCredentialsEmpty_ReturnsBadRequest()
	{
		IActionResult result = await _controller.Login(new LoginRequest
		{
			Login = "   ",
			Password = "password123"
		});

		BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
		ApiError error = Assert.IsType<ApiError>(badRequest.Value);
		Assert.Equal("0.2.0", error.StatusCode);
	}

	[Fact(DisplayName = "Вход при неверном пароле")]
	public async Task Login_WhenWrongPassword_ReturnsUnauthorized()
	{
		await TestDataMock.MockUserAsync(_context);

		IActionResult result = await _controller.Login(new LoginRequest
		{
			Login = "ivanov",
			Password = "wrong-password"
		});

		UnauthorizedObjectResult unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
		ApiError error = Assert.IsType<ApiError>(unauthorized.Value);
		Assert.Equal("1.2.3", error.StatusCode);
	}

	[Fact(DisplayName = "Вход при несуществующем пользователе")]
	public async Task Login_WhenUserNotFound_ReturnsUnauthorized()
	{
		IActionResult result = await _controller.Login(new LoginRequest
		{
			Login = "unknown",
			Password = "password123"
		});

		UnauthorizedObjectResult unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
		ApiError error = Assert.IsType<ApiError>(unauthorized.Value);
		Assert.Equal("1.2.3", error.StatusCode);
	}

	[Fact(DisplayName = "Вход при валидных данных")]
	public async Task Login_WhenValid_ReturnsOkWithAccessToken()
	{
		await TestDataMock.MockUserAsync(_context);

		IActionResult result = await _controller.Login(new LoginRequest
		{
			Login = "ivanov",
			Password = "password123"
		});

		Assert.IsType<OkResult>(result);
		string opaqueToken = AssertHasAccessTokenCookie(_controller);
		Assert.True(Guid.TryParse(opaqueToken, out Guid tokenUuid));
		Assert.NotNull(await _accessTokenList.GetAsync(tokenUuid));
	}

	[Fact(DisplayName = "Регистрация при пустом теле запроса")]
	public async Task Registration_WhenRequestNull_ReturnsBadRequest()
	{
		ActionResult<UsersResponseDto> result = await _controller.Registration(null);

		BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
		ApiError error = Assert.IsType<ApiError>(badRequest.Value);
		Assert.Equal("0.1.0", error.StatusCode);
		Assert.Equal("BODY", error.Field);
	}

	[Fact(DisplayName = "Регистрация при пустом логине или пароле")]
	public async Task Registration_WhenLoginOrPasswordEmpty_ReturnsBadRequest()
	{
		ActionResult<UsersResponseDto> result = await _controller.Registration(new UsersCreateDto
		{
			Login = "   ",
			Password = "password123",
			FirstName = "Иван",
			LastName = "Иванов"
		});

		BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
		ApiError error = Assert.IsType<ApiError>(badRequest.Value);
		Assert.Equal("0.2.0", error.StatusCode);
	}

	[Fact(DisplayName = "Регистрация при пустом имени")]
	public async Task Registration_WhenFirstNameEmpty_ReturnsBadRequest()
	{
		ActionResult<UsersResponseDto> result = await _controller.Registration(new UsersCreateDto
		{
			Login = "newuser",
			Password = "password123",
			FirstName = "   ",
			LastName = "Иванов"
		});

		BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
		ApiError error = Assert.IsType<ApiError>(badRequest.Value);
		Assert.Equal("0.2.1", error.StatusCode);
		Assert.Equal("FirstName", error.Field);
	}

	[Fact(DisplayName = "Регистрация при невалидном UUID роли")]
	public async Task Registration_WhenRoleUuidInvalid_ReturnsBadRequest()
	{
		ActionResult<UsersResponseDto> result = await _controller.Registration(new UsersCreateDto
		{
			Login = "newuser",
			Password = "password123",
			FirstName = "Иван",
			LastName = "Иванов",
			RolesUuid = ["СТУДЕНТ"],
		});

		BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
		ApiError error = Assert.IsType<ApiError>(badRequest.Value);
		Assert.Equal("0.2.2", error.StatusCode);
		Assert.Equal("RolesUuid", error.Field);
	}

	[Fact(DisplayName = "Регистрация при дублировании логина")]
	public async Task Registration_WhenDuplicateLogin_ReturnsConflict()
	{
		await TestDataMock.MockUserAsync(_context);

		ActionResult<UsersResponseDto> result = await _controller.Registration(new UsersCreateDto
		{
			Login = "ivanov",
			Password = "password123",
			FirstName = "Иван",
			LastName = "Иванов"
		});

		ConflictObjectResult conflict = Assert.IsType<ConflictObjectResult>(result.Result);
		ApiError error = Assert.IsType<ApiError>(conflict.Value);
		Assert.Equal("1.1.1", error.StatusCode);
	}

	[Fact(DisplayName = "Регистрация при валидных данных")]
	public async Task Registration_WhenValid_ReturnsCreated()
	{
		ActionResult<UsersResponseDto> result = await _controller.Registration(new UsersCreateDto
		{
			Login = "petrov",
			Password = "password123",
			FirstName = "Пётр",
			LastName = "Петров"
		});

		CreatedResult created = Assert.IsType<CreatedResult>(result.Result);
		UsersResponseDto dto = Assert.IsType<UsersResponseDto>(created.Value);
		Assert.Equal("petrov", dto.Login);
		string opaqueToken = AssertHasAccessTokenCookie(_controller);
		Assert.True(Guid.TryParse(opaqueToken, out Guid tokenUuid));
		Assert.NotNull(await _accessTokenList.GetAsync(tokenUuid));
	}

	[Fact(DisplayName = "Проверка токена при отсутствии cookie и заголовка")]
	public async Task CheckAuthtoken_WhenNoHeader_ReturnsBadRequest()
	{
		IActionResult result = await _controller.CheckAuthtoken();

		BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
		ApiError error = Assert.IsType<ApiError>(badRequest.Value);
		Assert.Equal("2.4.0", error.StatusCode);
		Assert.Equal("accessToken", error.Field);
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
		TestControllerHelper.SetAccessTokenCookie(_controller, opaqueToken);

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
		TestControllerHelper.SetAccessTokenCookie(_controller, opaqueToken);

		IActionResult result = await _controller.CheckAuthtoken();

		UnauthorizedObjectResult unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
		ApiError error = Assert.IsType<ApiError>(unauthorized.Value);
		Assert.Equal("2.2.2", error.StatusCode);
	}

	[Fact(DisplayName = "Выход при отсутствии access token")]
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

	private static string AssertHasAccessTokenCookie(ControllerBase controller)
	{
		string? setCookie = controller.Response.Headers.SetCookie
			.FirstOrDefault(value => value.StartsWith("accessToken=", StringComparison.Ordinal));
		Assert.False(string.IsNullOrWhiteSpace(setCookie));
		string opaqueToken = setCookie!.Split(';')[0]["accessToken=".Length..];
		Assert.False(string.IsNullOrWhiteSpace(opaqueToken));
		return opaqueToken;
	}
}
