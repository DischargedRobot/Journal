using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;

namespace AuthService.Tests.Helpers;

public static class TestControllerHelper
{
    // для того чтобы логи не падали из-за активити
    public static void SetupContext(ControllerBase controller, string actionName)
    {
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext(),
            RouteData = new RouteData(),
            ActionDescriptor = new ControllerActionDescriptor
            {
                ActionName = actionName
            }
        };
    }

    public static void SetAuthorizationHeader(ControllerBase controller, string token)
    {
        controller.Request.Headers.Authorization = $"Bearer {token}";
    }
}
