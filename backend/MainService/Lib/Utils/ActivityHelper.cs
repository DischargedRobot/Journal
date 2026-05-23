using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;

namespace MainService.Lib.Utils
{
    public static class ActivityHelper
    {
        public static Activity? StartAndLog(
            this ActivitySource? source,
            ILogger logger,
            ControllerBase controller
        )
        {
            string serviceName = source?.Name ?? "main-service";
            string functionName = controller.ControllerContext.ActionDescriptor.ActionName;
            Activity? activity = source?.StartActivity($"{serviceName}.{functionName}", ActivityKind.Server);
            logger.LogInformation("Начало операции {Operation} {Path}", functionName, controller.Request.Path);
            return activity;
        }
    }
}
