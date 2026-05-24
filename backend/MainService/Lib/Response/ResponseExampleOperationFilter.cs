using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace AuthService.ResponseExample
{
    public class ResponseExampleOperationFilter : IOperationFilter
    {

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            ResponseExampleAttribute[] attrs = context.MethodInfo.GetCustomAttributes(true)
                .OfType<ResponseExampleAttribute>()
                .ToArray();

            if (attrs.Length == 0) return;

            foreach (ResponseExampleAttribute attr in attrs)
            {
                string statusKey = attr.HttpStatus.ToString();
                if (!operation.Responses.TryGetValue(statusKey, out OpenApiResponse? response))
                {
                    response = operation.Responses[statusKey] = new OpenApiResponse
                    {
                        Content = new Dictionary<string, OpenApiMediaType>(),
                        Description = $"HTTP {statusKey} response"
                    };
                    response.Content["application/json"] = new OpenApiMediaType();
                }

                if (!response.Content.TryGetValue("application/json", out OpenApiMediaType? mediaType))
                {
                    continue;
                }

                if (attrs.Count(a => a.HttpStatus == attr.HttpStatus) > 1)
                {
                    mediaType.Examples ??= new Dictionary<string, OpenApiExample>();
                    string exampleKey = $"Пример для HTTP {statusKey}";
                    mediaType.Examples[exampleKey] = new OpenApiExample
                    {
                        Value = OpenApiAnyFactory.CreateFromJson(System.Text.Json.JsonSerializer.Serialize(attr.ExampleType.GetProperty("Example")?.GetValue(null)))
                    };
                }
                else
                {
                    mediaType.Example ??= OpenApiAnyFactory.CreateFromJson(System.Text.Json.JsonSerializer.Serialize(attr.ExampleType.GetProperty("Example")?.GetValue(null)));
                }
            }
        }
    }
}