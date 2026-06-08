using System.Diagnostics;
using AuthService;
using AuthService.Controller;
using AuthService.Errors;
using AuthService.Model;
using AuthService.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace AuthService.Tests.Controllers;

public class RolesControllerTests : IDisposable
{
    private readonly AuthTestContext _testContext;
    private readonly AuthServiceContext _context;
    private readonly RolesController _controller;

    public RolesControllerTests()
    {
        _testContext = TestDbContextFactory.Create();
        _context = _testContext.Context;
        _controller = new RolesController(
            _context,
            NullLogger<RolesController>.Instance,
            new ActivitySource("AuthService.Tests"));
        TestControllerHelper.SetupContext(_controller, nameof(RolesController));
    }

    [Fact(DisplayName = "Получение ролей при пустой базе данных")]
    public async Task GetRoles_WhenEmpty_ReturnsNotFound()
    {
        ActionResult<PagedResult<RolesResponseDto>> result = await _controller.GetRoles();

        NotFoundObjectResult notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        ApiError error = Assert.IsType<ApiError>(notFound.Value);
        Assert.Equal("1.0.3", error.StatusCode);
    }

    [Fact(DisplayName = "Получение ролей при наличии")]
    public async Task GetRoles_WhenDataExists_ReturnsOk()
    {
        await TestDataMock.MockRoleAsync(_context);

        ActionResult<PagedResult<RolesResponseDto>> result = await _controller.GetRoles();

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result.Result);
        PagedResult<RolesResponseDto> paged = Assert.IsType<PagedResult<RolesResponseDto>>(ok.Value);
        Assert.Single(paged.Items);
        Assert.Equal("Студент", paged.Items.First().Name);
    }

    [Theory(DisplayName = "Получение ролей при отрицательном смещении")]
    [InlineData(-1)]
    public async Task GetRoles_WhenOffsetNegative_ReturnsBadRequest(int offset)
    {
        ActionResult<PagedResult<RolesResponseDto>> result = await _controller.GetRoles(offset: offset);

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        ApiError error = Assert.IsType<ApiError>(badRequest.Value);
        Assert.Equal("0.2.1", error.StatusCode);
        Assert.Equal("offset", error.Field);
    }

    [Fact(DisplayName = "Получение роли при несуществующем UUID")]
    public async Task GetRole_WhenNotFound_ReturnsNotFound()
    {
        IActionResult result = await _controller.GetRole(Guid.NewGuid());

        NotFoundObjectResult notFound = Assert.IsType<NotFoundObjectResult>(result);
        ApiError error = Assert.IsType<ApiError>(notFound.Value);
        Assert.Equal("1.2.3", error.StatusCode);
    }

    [Fact(DisplayName = "Получение роли при существующем UUID")]
    public async Task GetRole_WhenExists_ReturnsOk()
    {
        Roles role = await TestDataMock.MockRoleAsync(_context);

        IActionResult result = await _controller.GetRole(role.Uuid);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        RolesResponseDto dto = Assert.IsType<RolesResponseDto>(ok.Value);
        Assert.Equal(role.Uuid, dto.Uuid);
        Assert.Equal("Студент", dto.Name);
    }

    [Fact(DisplayName = "Создание роли при пустом названии")]
    public async Task CreateRole_WhenNameEmpty_ReturnsBadRequest()
    {
        RolesTypes roleType = await TestDataMock.MockRoleTypeAsync(_context);

        IActionResult result = await _controller.CreateRole(new RolesCreateDto
        {
            Name = "   ",
            RightsUuids = [],
            RoleTypesUuids = [roleType.Uuid]
        });

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
        ApiError error = Assert.IsType<ApiError>(badRequest.Value);
        Assert.Equal("0.2.0", error.StatusCode);
        Assert.Equal("Name", error.Field);
    }

    [Fact(DisplayName = "Создание роли при валидных данных")]
    public async Task CreateRole_WhenValid_ReturnsCreated()
    {
        RolesTypes roleType = await TestDataMock.MockRoleTypeAsync(_context);

        IActionResult result = await _controller.CreateRole(new RolesCreateDto
        {
            Name = "Преподаватель",
            RightsUuids = [],
            RoleTypesUuids = [roleType.Uuid]
        });

        CreatedAtActionResult created = Assert.IsType<CreatedAtActionResult>(result);
        RolesResponseDto dto = Assert.IsType<RolesResponseDto>(created.Value);
        Assert.Equal("Преподаватель", dto.Name);
    }

    [Fact(DisplayName = "Создание роли при дублировании названия")]
    public async Task CreateRole_WhenDuplicateName_ReturnsConflict()
    {
        RolesTypes roleType = await TestDataMock.MockRoleTypeAsync(_context);
        await TestDataMock.MockRoleAsync(_context);

        IActionResult result = await _controller.CreateRole(new RolesCreateDto
        {
            Name = "Студент",
            RightsUuids = [],
            RoleTypesUuids = [roleType.Uuid]
        });

        ConflictObjectResult conflict = Assert.IsType<ConflictObjectResult>(result);
        ApiError error = Assert.IsType<ApiError>(conflict.Value);
        Assert.Equal("1.2.1", error.StatusCode);
    }

    public void Dispose()
    {
        _testContext.Dispose();
    }
}
