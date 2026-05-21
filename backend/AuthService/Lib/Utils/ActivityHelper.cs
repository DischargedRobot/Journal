using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;

namespace AuthService.Lib.Utils
{
    public static class ActivityHelper
    {
        public static Activity? StartAndLog(
            this ActivitySource? source,
            ILogger logger,
            ControllerBase controller
        )
        {
            string serviceName = source?.Name ?? "auth-service";
            string functionName = controller.ControllerContext.ActionDescriptor.ActionName;
            Activity? activity = source?.StartActivity($"{serviceName}.{functionName}", ActivityKind.Server);
            logger.LogInformation("Начало операции {Operation} {Path}", functionName, controller.Request.Path);
            return activity;
        }
    }
}
