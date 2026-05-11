using Swashbuckle.AspNetCore.Filters;

using MainService.Errors;

namespace MainService.Errors
{
    public class ApiError400BadRequestExample : IExamplesProvider<ApiError>
    {
        public ApiError GetExamples()
        {
            return new ApiError
            {
                StatusCode = "400",
                Title = "Неверный запрос",
                Message = "Неправильный или некорректный запрос"
            };
        }
    }
}
