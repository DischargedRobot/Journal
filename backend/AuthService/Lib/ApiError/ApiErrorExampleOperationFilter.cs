using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace AuthService.Errors
{
    public class ApiErrorExampleOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            ApiErrorExampleAttribute[] attrs = context.MethodInfo.GetCustomAttributes(true)
                .OfType<ApiErrorExampleAttribute>()
                .ToArray();

            if (attrs.Length == 0) return;

            foreach (ApiErrorExampleAttribute attr in attrs)
            {

                string statusKey = attr.HttpStatus.ToString();
                // установили ли код, если нет, то берём из статуса ответа
                if (!operation.Responses.TryGetValue(statusKey, out OpenApiResponse? response))
                {
                    response = operation.Responses[statusKey] = new OpenApiResponse
                    {
                        Content = new Dictionary<string, OpenApiMediaType>(),
                        Description = $"HTTP {statusKey} response"
                    };
                    response.Content["application/json"] = new OpenApiMediaType();
                }

                // на случай если укажем другой медиатип
                if (!response.Content.TryGetValue("application/json", out OpenApiMediaType? mediaType))
                {
                    continue;
                }

                ApiError err = attr.Error;

                OpenApiObject openApiObj = new()
                {
                    [nameof(err.StatusCode)] = new OpenApiString(err.StatusCode),
                    [nameof(err.Title)] = new OpenApiString(err.Title),
                    [nameof(err.Message)] = new OpenApiString(err.Message),
                    [nameof(err.Field)] = new OpenApiString(err.Field ?? string.Empty)
                };

                if (!string.IsNullOrEmpty(err.Details))
                {
                    openApiObj[nameof(err.Details)] = new OpenApiString(err.Details);
                }

                if (attrs.Count(a => a.HttpStatus == attr.HttpStatus) > 1)
                {
                    mediaType.Examples ??= new Dictionary<string, OpenApiExample>();
                    string exampleKey = $"{attr.StatusCode} - {attr.Field}";
                    mediaType.Examples[exampleKey] = new OpenApiExample { Value = openApiObj };
                }
                else
                {
                    mediaType.Example = openApiObj;
                }
            }
        }
    }
}
