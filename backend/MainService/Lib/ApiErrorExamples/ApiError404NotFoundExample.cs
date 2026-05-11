using Swashbuckle.AspNetCore.Filters;
using MainService.Errors;

namespace MainService.Errors.ApiErrorExamples
{
    public class ApiError404NotFoundExample : IExamplesProvider<ApiError>
    {
        public ApiError GetExamples()
        {
            return new ApiError
            {
                StatusCode = "404",
                Title = "Не найдено",
                Message = "Ресурс не найден"
            };
        }
    }
}
