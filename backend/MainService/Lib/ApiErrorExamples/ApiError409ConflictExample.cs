using Swashbuckle.AspNetCore.Filters;

using MainService.Errors;

namespace MainService.Errors
{
    public class ApiError409ConflictExample : IExamplesProvider<ApiError>
    {
        public ApiError GetExamples()
        {
            return new ApiError
            {
                StatusCode = "0.2.1",
                Title = "Конфликт",
                Message = "Запрос не может быть выполнен из-за конфликта с текущим состоянием ресурса",
                Field = string.Empty
            };
        }
    }
}
